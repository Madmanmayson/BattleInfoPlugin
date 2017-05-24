﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BattleInfoPlugin.Properties;
using Livet;

namespace BattleInfoPlugin.Models
{
    /// <summary>
    /// 多言語化されたリソースへのアクセスを提供します。
    /// </summary>
    public class ResourceService : NotificationObject
    {
        #region static members

        public static ResourceService Current { get; } = new ResourceService();

        #endregion

        /// <summary>
        /// サポートされているカルチャの名前。
        /// </summary>
        private readonly string[] supportedCultureNames =
        {
            "ja", // Resources.resx
			"en",
        };

        /// <summary>
        /// 多言語化されたリソースを取得します。
        /// </summary>
        public Resources Resources { get; }

        /// <summary>
        /// サポートされているカルチャを取得します。
        /// </summary>
        public IReadOnlyCollection<CultureInfo> SupportedCultures { get; }

        private ResourceService()
        {
            this.Resources = new Resources();
            this.SupportedCultures = this.supportedCultureNames
                .Select(x =>
                {
                    try
                    {
                        return CultureInfo.GetCultureInfo(x);
                    }
                    catch (CultureNotFoundException)
                    {
                        return null;
                    }
                })
                .Where(x => x != null)
                .ToList();
        }

        /// <summary>
        /// 指定されたカルチャ名を使用して、リソースのカルチャを変更します。
        /// </summary>
        /// <param name="name">カルチャの名前。</param>
        public void ChangeCulture(string name)
        {
            Resources.Culture = this.SupportedCultures.SingleOrDefault(x => x.Name == name);

            this.RaisePropertyChanged(nameof(this.Resources));
        }
    }
}
