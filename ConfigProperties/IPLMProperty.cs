
#region "                   名稱空間"
using BCS.CADs.Synchronization.PLMList;
using BCS.CADs.Synchronization.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
#endregion

namespace BCS.CADs.Synchronization.ConfigProperties
{
    /// <summary>
    /// 系統欄位和CAD屬性
    /// </summary>
    interface IPLMProperty: IDefaultElements
    {
        #region "                   宣告區"

        #endregion


        #region "                   屬性"

        /// <summary>
        /// 來源的SearchItem
        /// </summary>
        SearchItem SoruceSearchItem { get; set; }

        /// <summary>
        /// 同步PLM
        /// </summary>
        bool IsSyncPLM { get; set; }

        /// <summary>
        /// 同步CAD
        /// </summary>
        bool IsSyncCAD { get; set; }

        /// <summary>
        /// CAD屬性分類
        /// </summary>
        string CadClass { get; set; }

        /// <summary>
        /// CAD預設屬性名稱
        /// </summary>
        string CadName { get; set; }

        /// <summary>
        /// CAD預設屬性
        /// </summary>
        string CadDefaultName { get; set; }

        /// <summary>
        /// CAD資料類型
        /// </summary>
        string CadDataType { get; set; }

        /// <summary>
        /// CAD資料來源
        /// </summary>
        string CadDataSource { get; set; }


        /// <summary>
        /// 參數
        /// </summary>
        string Parameters { get; set; }


        /// <summary>
        /// PLM屬性名稱 (因為Name無法在Grid Header Content,所以新增PropertyName)
        /// </summary>
        string PropertyName { get; set; }


        /// <summary>
        /// PLM資料類型
        /// </summary>
        string DataType { get; set; }

        /// <summary>
        /// PLM資料來源
        /// </summary>
        string DataSource { get; set; }

        /// <summary>
        /// 樣式
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// 鍵名
        /// </summary>
        string KeyedName { get; set; }

        /// <summary>
        /// 鍵值
        /// </summary>
        string KeyedId { get; set; }

        /// <summary>
        /// PLM參考資料來源
        /// </summary>
        string RefDataSource { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        string DataValue { get; set; }

        /// <summary>
        /// 顯示值
        /// </summary>
        string DisplayValue { get; set; }
        /// <summary>
        /// 同步對方的值
        /// </summary>
        string SyncValue { get; set; }

        /// <summary>
        /// 同步對方的顯示值
        /// </summary>
        string SyncDisplayValue { get; set; }

        /// <summary>
        /// 同步顏色型態值
        /// </summary>
        int SyncColorTypeValue { get; set; }

        /// <summary>
        ///必填
        /// </summary>
        bool IsRequired { get; set; }

        /// <summary>
        ///是否修改
        /// </summary>
        bool IsModify { get; set; }

        /// <summary>
        ///是否執行過更新旗標
        /// </summary>
        bool IsUpdateFlag { get; set; }


        /// <summary>
        ///是否初始
        /// </summary>
        bool IsInitial { get; set; }

        /// <summary>
        ///是否存在
        /// </summary>
        bool IsExist { get; set; }

        /// <summary>
        ///是否為系統程式新增屬性
        /// </summary>
        bool IsSystemAdd { get; set; }


        /// <summary>
        /// PLM屬性標籤名稱
        /// </summary>
        string Label { get; set; }


        /// <summary>
        /// 欄位寬度
        /// </summary>
        int ColumnWidth { get; set; }


        /// <summary>
        /// 標籤其他語系
        /// </summary>
        Dictionary<string, string> Labels { get; set; }

        /// <summary>
        /// List
        /// </summary>
        ObservableCollection<PLMListItem> ListItems { get; set; }


        /// <summary>
        /// 目前選到版本的ItemId
        /// </summary>
        string PLMRevisionItemId { get; set; } 

        /// <summary>
        /// 系統所有版本+版次
        /// </summary>
        ObservableCollection<PLMRevision> PlmRevisions { get; set; }

        /// <summary>
        /// 被選到的項目
        /// </summary>
        PLMListItem SelectedListItem { get; set; }



        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
