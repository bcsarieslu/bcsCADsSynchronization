using BCS.CADs.Synchronization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.Search
{
    /// <summary>
    /// 插入分件或替換分件紀錄
    /// </summary>
    public class StructuralChange
    {
        /// <summary>
        /// 異動型態:(1)插入分件(2)替換分件(3)複製轉新增(4)插入另存分件
        /// </summary>
        public ChangeType Type { get; set; }


        ///// <summary>
        ///// 父親的SearchItem
        ///// </summary>
        //public SearchItem ParentSearchItem { get; set; }

        /// <summary>
        /// 來源圖檔名稱
        /// </summary>
        public string SourceFileName { get; set; } = "";

        /// <summary>
        /// 來源圖檔路徑
        /// </summary>
        public string SourceFilePath { get; set; } = "";

        /// <summary>
        /// 來源CAD ItemId
        /// </summary>
        public string SourceItemId { get; set; } = "";

        /// <summary>
        /// 來源CAD Config Id
        /// </summary>
        public string SourceItemConfigId { get; set; } = "";

        /// <summary>
        /// 目標異動後的圖檔名稱
        /// </summary>
        public string TargetFileName { get; set; } = "";

        /// <summary>
        /// 目標異動後的圖檔路徑
        /// </summary>
        public string TargetFilePath { get; set; } = "";

        /// <summary>
        /// 目標異動後的CAD ItemId
        /// </summary>
        public string TargetItemId { get; set; } = "";

        /// <summary>
        /// 目標異動後的CAD Config Id
        /// </summary>
        public string TargetItemConfigId { get; set; } = "";

        /// <summary>
        /// 是否為全面替換
        /// </summary>
        public bool IsReplaceAll { get; set; } = false;

        /// <summary>
        /// 副本Id
        /// </summary>
        public string InstanceId { get; set; } = "";

        /// <summary>
        /// 順序位置
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否存在目前CAD圖檔
        /// </summary>
        public bool IsExist { get; set; } = false;

    }
}
