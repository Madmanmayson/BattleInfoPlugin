﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BattleInfoPlugin.Models;
using BattleInfoPlugin.Models.Notifiers;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;

namespace BattleInfoPlugin.ViewModels
{
    public class BattleViewModel : ViewModel
    {
        public static BattleViewModel Current { get; } = new BattleViewModel();

        private BattleData BattleData { get; } = BattleData.Current;

        public bool IsInBattle
            => this.BattleData.IsInBattle;

        public string BattleResultRank
            => this.BattleData.BattleResult != Models.BattleResult.なし
                ? this.BattleData.BattleResult.ToString()
                : "";

        public string BattleName
            => this.BattleData?.Name ?? "";

        public string UpdatedTime
            => this.BattleData != null && this.BattleData.UpdatedTime != default(DateTimeOffset)
                ? this.BattleData.UpdatedTime.ToString("yyyy/MM/dd HH:mm:ss")
                : "No Data";

        public string BattleSituation
            => this.BattleData != null && this.BattleData.BattleSituation != Models.BattleSituation.なし
                ? this.BattleData.BattleSituation.ToString()
                : "";

        public string FriendAirSupremacy
            => this.BattleData?.FriendAirSupremacy.ToString();

        public string DropShipName
            => this.BattleData?.DropShipName;

        public AirCombatResult[] AirCombatResults
            => this.BattleData?.AirCombatResults ?? new AirCombatResult[0];


        #region FirstFleet変更通知プロパティ
        private FleetViewModel _FirstFleet;

        public FleetViewModel FirstFleet
        {
            get
            { return this._FirstFleet; }
            set
            {
                if (this._FirstFleet == value)
                    return;
                this._FirstFleet = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region SecondFleet変更通知プロパティ
        private FleetViewModel _SecondFleet;

        public FleetViewModel SecondFleet
        {
            get
            { return this._SecondFleet; }
            set
            {
                if (this._SecondFleet == value)
                    return;
                this._SecondFleet = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region Enemies変更通知プロパティ
        private FleetViewModel _Enemies;

        public FleetViewModel Enemies
        {
            get
            { return this._Enemies; }
            set
            {
                if (this._Enemies == value)
                    return;
                this._Enemies = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region NextPointInfo

        private NextPointInfoViewModel _NextPointInfo = new NextPointInfoViewModel { IsInSortie = false };

        public NextPointInfoViewModel NextPointInfo
        {
            get { return this._NextPointInfo; }
            set
            {
                if (this._NextPointInfo != value)
                {
                    this._NextPointInfo = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion


        #region LandBaseVisibility

        private Visibility _LandBaseVisibility;

        public Visibility LandBaseVisibility
        {
            get { return this._LandBaseVisibility; }
            set
            {
                if (this._LandBaseVisibility != value)
                {
                    this._LandBaseVisibility = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion


        #region FriendLandBaseAirCombatResults

        private LandBaseAirCombatResultViewModel[] _FriendLandBaseAirCombatResults;

        public LandBaseAirCombatResultViewModel[] FriendLandBaseAirCombatResults
        {
            get { return this._FriendLandBaseAirCombatResults; }
            set
            {
                if (this._FriendLandBaseAirCombatResults != value)
                {
                    this._FriendLandBaseAirCombatResults = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion


        #region EnemyLandBaseAirCombatResults

        private LandBaseAirCombatResultViewModel[] _EnemyLandBaseAirCombatResults;

        public LandBaseAirCombatResultViewModel[] EnemyLandBaseAirCombatResults
        {
            get { return this._EnemyLandBaseAirCombatResults; }
            set
            {
                if (this._EnemyLandBaseAirCombatResults != value)
                {
                    this._EnemyLandBaseAirCombatResults = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion


        #region FriendAirCombatResults

        private AirCombatResultViewModel[] _FriendAirCombatResults;

        public AirCombatResultViewModel[] FriendAirCombatResults
        {
            get { return this._FriendAirCombatResults; }
            set
            {
                if (this._FriendAirCombatResults != value)
                {
                    this._FriendAirCombatResults = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion


        #region EnemyAirCombatResults

        private AirCombatResultViewModel[] _EnemyAirCombatResults;

        public AirCombatResultViewModel[] EnemyAirCombatResults
        {
            get { return this._EnemyAirCombatResults; }
            set
            {
                if (this._EnemyAirCombatResults != value)
                {
                    this._EnemyAirCombatResults = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        private BattleViewModel()
        {
            this._FirstFleet = new FleetViewModel("自艦隊");
            this._SecondFleet = new FleetViewModel("護衛艦隊");
            this._Enemies = new FleetViewModel("敵艦隊");

            this.CompositeDisposable.Add(new PropertyChangedEventListener(this.BattleData)
            {
                {
                    () => this.BattleData.IsInBattle,
                    (_, __) => this.RaisePropertyChanged(() => this.IsInBattle)
                },
                {
                    () => this.BattleData.BattleResult,
                    (_, __) => this.RaisePropertyChanged(() => this.BattleResultRank)
                },
                {
                    () => this.BattleData.Name,
                    (_, __) => this.RaisePropertyChanged(() => this.BattleName)
                },
                {
                    () => this.BattleData.UpdatedTime,
                    (_, __) => this.RaisePropertyChanged(() => this.UpdatedTime)
                },
                {
                    () => this.BattleData.BattleSituation,
                    (_, __) => this.RaisePropertyChanged(() => this.BattleSituation)
                },
                {
                    () => this.BattleData.FriendAirSupremacy,
                    (_, __) => this.RaisePropertyChanged(() => this.FriendAirSupremacy)
                },
                {
                    () => this.BattleData.LandBaseAirCombatResults,
                    (_, __) =>
                    {
                        var landbase = this.BattleData.LandBaseAirCombatResults;
                        this.UpdateLandBaseVisibility(landbase.Length > 0);
                        this.FriendLandBaseAirCombatResults = landbase.Select(x => new LandBaseAirCombatResultViewModel(x, FleetType.First)).ToArray();
                        this.EnemyLandBaseAirCombatResults = landbase.Select(x => new LandBaseAirCombatResultViewModel(x, FleetType.Enemy)).ToArray();
                    }
                },
                {
                    () => this.BattleData.AirCombatResults,
                    (_, __) =>
                    {
                        this.RaisePropertyChanged(() => this.AirCombatResults);
                        this.FriendAirCombatResults = this.AirCombatResults.Select(x => new AirCombatResultViewModel(x, FleetType.First)).ToArray();
                        this.EnemyAirCombatResults = this.AirCombatResults.Select(x => new AirCombatResultViewModel(x, FleetType.Enemy)).ToArray();
                    }
                },
                {
                    () => this.BattleData.DropShipName,
                    (_, __) => this.RaisePropertyChanged(() => this.DropShipName)
                },
                {
                    () => this.BattleData.FirstFleet,
                    (_, __) => this.FirstFleet.Fleet = this.BattleData.FirstFleet
                },
                {
                    () => this.BattleData.SecondFleet,
                    (_, __) => this.SecondFleet.Fleet = this.BattleData.SecondFleet
                },
                {
                    () => this.BattleData.Enemies,
                    (_, __) => this.Enemies.Fleet = this.BattleData.Enemies
                },
                {
                    () => this.BattleData.NextCell,
                    (_, __) =>
                    {
                        var nextCell = this.BattleData.NextCell;

                        var getLostItems = new List<GetLostItemViewModel>();
                        if (nextCell.GetItem != null)
                            getLostItems.AddRange(nextCell.GetItem.Select(item => new GetLostItemViewModel(item, true)));
                        if (nextCell.LostItem != null)
                            getLostItems.Add(new GetLostItemViewModel(nextCell.LostItem, false));

                        this.NextPointInfo = new NextPointInfoViewModel
                        {
                            MapId = nextCell.MapId.ToString(),
                            Id = ((char) (nextCell.Id - 1 + 'A')).ToString(),
                            CellType = nextCell.Type,
                            IsInSortie = true,
                            GetLostItems = getLostItems.ToArray(),
                        };
                    }
                },
                {
                    () => this.BattleData.State,
                    (_, __) => this.NextPointInfo = new NextPointInfoViewModel { IsInSortie = false }
                }
            });

            this.BattleData
                .Subscribe(nameof(BattleData.IsShowLandBaseAirStage),
                    () => this.UpdateLandBaseVisibility(this.BattleData.IsShowLandBaseAirStage))
                .AddTo(this);
        }

        private void UpdateLandBaseVisibility(bool visible)
        {
            this.LandBaseVisibility = visible
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}
