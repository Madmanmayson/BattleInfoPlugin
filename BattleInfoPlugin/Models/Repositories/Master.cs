using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using System.IO;
using System.Reactive.Linq;
using BattleInfoPlugin.Models.Raw;
using BattleInfoPlugin.Models.Settings;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;

namespace BattleInfoPlugin.Models.Repositories
{
    [DataContract]
    public class Master
    {
        private static readonly object mergeLock = new object();

        public static Master Current { get; } = new Master(PluginSettings.Paths.MasterDataFileName);

        /// <summary>
        /// すべての海域の定義を取得します。
        /// </summary>
        [DataMember]
        public Dictionary<int, MapArea> MapAreas { get; private set; }

        /// <summary>
        /// すべてのマップの定義を取得します。
        /// </summary>
        [DataMember]
        public Dictionary<int, MapInfo> MapInfos { get; private set; }

        /// <summary>
        /// すべてのセルの定義を取得します。
        /// </summary>
        [DataMember]
        public Dictionary<int, MapCell> MapCells { get; private set; }

        private readonly string filePath;

        private Master(string filePath)
        {
            this.filePath = filePath;

            var obj = this.filePath.Deserialize<Master>();

            this.MapAreas = (obj?.MapAreas).ValueOrNew();
            this.MapInfos = (obj?.MapInfos).ValueOrNew();
            this.MapCells = (obj?.MapCells).ValueOrNew();

            var proxy = KanColleClient.Current.Proxy;
            proxy.Observe<kcsapi_start2>("/kcsapi/api_start2/getData")
                .Subscribe(x => this.Update(x.Data));
            proxy.Observe<map_start_next>("/kcsapi/api_req_map/start")
                .Subscribe(this.Update);
            proxy.Observe<mapinfo>("/kcsapi/api_get_member/mapinfo")
                .Subscribe(x => this.UpdateMapRank(x.Data.api_map_info));
            proxy.ApiSessionSource
                .Where(x => x.Request.PathAndQuery == "/kcsapi/api_req_map/select_eventmap_rank")
                .TryParse()
                .Subscribe(x =>
                    this.UpdateMapRank(
                        int.Parse(x.Request["api_maparea_id"]),
                        int.Parse(x.Request["api_map_no"]),
                        int.Parse(x.Request["api_rank"]))
                );
        }

        public void Init()
        {
            // Do nothing
        }

        public void Update(kcsapi_start2 start2)
        {
            var mapAreas = start2.api_mst_maparea.Select(x => new MapArea(x)).ToDictionary(x => x.Id, x => x);
            var mapInfos = start2.api_mst_mapinfo.Select(x => new MapInfo(x)).ToDictionary(x => x.Id, x => x);
            this.MapAreas = this.MapAreas.Merge(mapAreas);
            this.MapInfos = this.MapInfos.Merge(mapInfos);

            this.Serialize(this.filePath);
        }

        public void Update(SvData<map_start_next> start)
        {
            int mapArea = start.Data.api_maparea_id;
            var mapNo = start.Data.api_mapinfo_no;
            var infoId = this.MapAreas[mapArea][mapNo].Id;

            var cells = start.Data.api_cell_data
                .Select(cell => new MapCell(cell, mapArea, mapNo, infoId))
                .ToDictionary(x => x.Id, x => x);

            this.MapCells = this.MapCells.Merge(cells);
            Debug.WriteLine(this.MapCells.Values.Count(c => c.MapAreaId == 36 && c.MapInfoId == 361));
            this.Serialize(this.filePath);
        }

        public void UpdateMapRank(member_mapinfo[] mapinfos)
        {
            foreach (var mapinfo in mapinfos)
            {
                this.MapInfos[mapinfo.api_id].Rank = mapinfo.api_eventmap?.api_selected_rank ?? 0;
            }
        }

        public void UpdateMapRank(int areaId, int mapId, int rank)
        {
            this.MapAreas[areaId][mapId].Rank = rank;
        }

        public bool Merge(string path)
        {
            if (!File.Exists(path)) return false;

            lock (mergeLock)
            {
                var obj = path.Deserialize<Master>();
                if (obj == null) return false;

                this.Merge(obj);

                this.Serialize(this.filePath);
            }

            return true;
        }

        private void Merge(Master master)
        {
            this.MapAreas = this.MapAreas.Merge(master.MapAreas);
            this.MapInfos = this.MapInfos.Merge(master.MapInfos);
            this.MapCells = this.MapCells.Merge(master.MapCells);
        }
    }
}
