
#region "                   名稱空間"
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.PLMList;
using BCS.CADs.Synchronization.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#endregion

namespace BCS.CADs.Synchronization.Entities
{
    /// <summary>
    /// 事件參數
    /// </summary>
    public class SyncReachedEventArgs : EventArgs, ISyncReachedEventArgs
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"
        /// <summary>
        /// 名稱
        /// </summary>
        public SyncEvents Name { get; set; }


        public bool IsResolveAllLightweightSuppres { get; set; } = true;

        public bool IsResolveAllSuppres { get; set; } = true;

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 功能名稱
        /// </summary>
        public string FunctionName { get; set; } = "";

        /// <summary>
        /// 型態
        /// </summary>
        public SyncType  Type { get; set; }

        public bool IsActiveCAD { get; set; } = true;

        /// <summary>
        /// 查詢PLM物件
        /// </summary>
        public List<SearchItem> SearchItems { get; set; }


        public List<StructuralChange> StructureChanges { get; set; }

        /// <summary>
        /// 查詢PLM目前物件
        /// </summary>
        public SearchItem SearchItem { get; set; } = null;

        /// <summary>
        /// 列表
        /// </summary>
        public Lists ListItem { get; set; }

        /// <summary>
        /// 同步事件
        /// </summary>
        public IntegrationEvents IntegrationEvent { get; set; } = null;

        /// <summary>
        /// Aras.IOM.Innovator
        /// </summary>
        //public Aras.IOM.Innovator AsInnovator { get; set; } = null;
        public Aras.IOM.Innovator AsInnovator { get; set; } = null;
        /// <summary>
        /// Aras.IOM.Item
        /// </summary>
        public Aras.IOM.Item AsItem { get; set; } = null;


        /// <summary>
        /// 類別名稱
        /// </summary>
        public List<ClassItem> ClassItems { get; set; } = null;


        /// <summary>
        /// 訊息
        /// </summary>
        public SyncMessages SyncMessages { get; set; }


        public CommonPartsLibrary PartsLibrary { get; set; }


        /// <summary>
        /// 取得目前語系資源
        /// </summary>
        public ResourceDictionary LanguageResources { get; set; }


        public string GetLanguageByKeyName(string key)
        {
            try
            {
                var value = LanguageResources[key];
                return value.ToString();
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return "";
            }
        }





        //public List<IntegrationEvents> integrationEvent { get; set; } = null;
        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
