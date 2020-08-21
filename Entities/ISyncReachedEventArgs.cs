

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
    internal interface ISyncReachedEventArgs
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"
        /// <summary>
        /// 名稱
        /// </summary>
        SyncEvents Name { get; set; }


        bool IsResolveAllLightweightSuppres { get; set; }
        bool IsResolveAllSuppres { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        string Url { get; set; }


        /// <summary>
        /// 功能名稱
        /// </summary>
        string FunctionName { get; set; }

        /// <summary>
        /// 型態
        /// </summary>
        SyncType Type { get; set; }

        bool IsActiveCAD { get; set; }

        /// <summary>
        /// 查詢PLM物件
        /// </summary>
        List<SearchItem> SearchItems { get; set; }

        /// <summary>
        /// 查詢PLM目前物件
        /// </summary>
        SearchItem SearchItem { get; set; }

        /// <summary>
        /// 同步事件
        /// </summary>
        IntegrationEvents IntegrationEvent { get; set; }

        /// <summary>
        /// 列表
        /// </summary>
        Lists ListItem { get; set; }

        /// <summary>
        /// Aras.IOM.Innovator
        /// </summary>
        Aras.IOM.Innovator AsInnovator { get; set; }
        /// <summary>
        /// Aras.IOM.Item
        /// </summary>
        Aras.IOM.Item AsItem { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        SyncMessages SyncMessages { get; set; }


        ResourceDictionary LanguageResources { get; set; }


        CommonPartsLibrary PartsLibrary { get; set; }

        //List<IntegrationEvents> integrationEvent { get; set; }
        //BCS.CADs.Synchronization.Classes.IntegrationEvents



        /// <summary>
        /// 類別名稱
        /// </summary>
        List<ClassItem> ClassItems { get; set; }

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion


    }
}
