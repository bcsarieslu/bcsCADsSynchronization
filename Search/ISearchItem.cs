
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Search
{
    /// <summary>
    /// 查詢PLM物件
    /// </summary>
    interface ISearchItem
    {


        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// CAD類別名稱
        /// </summary>
        string ClassName { get; set; }

        /// <summary>
        /// CAD類別名稱
        /// </summary>
        string ItemType { get; set; }

        /// <summary>
        /// 檔名
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// 檔案路徑
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// 零件庫路徑
        /// </summary>
        string LibraryPath { get; set; }

        /// <summary>
        /// CADId
        /// </summary>
        string ItemId { get; set; }
        /// <summary>
        /// CAD Config Id
        /// </summary>
        string ItemConfigId { get; set; }

        /// <summary>
        /// 鍵名
        /// </summary>
        string KeyedName { get; set; }

        string Thumbnail { get; set; }

        /// <summary>
        /// CAD圖檔讀取Id
        /// </summary>
        string CadItemId { get; set; }
        /// <summary>
        /// CAD圖檔讀取 Config Id
        /// </summary>
        string CadItemConfigId { get; set; }

        /// <summary>
        /// CAD File Id
        /// </summary>
        string FileId { get; set; }

        ///// <summary>
        ///// New Version Id
        ///// </summary>
        //string newVersionId { get; set; }

        /// <summary>
        /// CAD是否存在PLM (圖檔存在,CAD物件存在) bcs_added_filename 有值
        /// </summary>
        bool IsExist { get; set; }
        /// <summary>
        /// CAD是否是新增 (圖檔存在,CAD物件存在) : bcs_added_filename 是空值
        /// </summary>
        bool IsAdded { get; set; }
        /// <summary>
        /// 條件規則屬性
        /// </summary>
        Dictionary<string, string> RuleProperties { get; set; }

        /// <summary>
        /// 系統欄位和CAD屬性
        /// </summary>
        ObservableCollection<PLMProperty> PlmProperties { get; set; }

        /// <summary>
        /// 系統所有版本+版次
        /// </summary>
        //ObservableCollection<PLMRevision> PlmRevisions { get; set; }


        /// <summary>
        /// 子階資訊 <子階config_id,List<sort_order>>
        /// </summary>
        ObservableCollection<CADStructure> CadStructure { get; set;}
        //Dictionary<string, List<int>> structure {get; set;}

        /// <summary>
        /// CAD關聯檔案
        /// </summary>
        List<PLMPropertyFile> PropertyFile { get; set; }

        /// <summary>
        /// View的編輯動作(當Client CAD整合程式,取得Client的屬性後,會變為true,表示有取過,可以紀錄第二次再取時,不執行)
        /// </summary>
        bool IsViewEdited { get; set; }
        /// <summary>
        /// View的加入選取動作
        /// </summary>
        bool IsViewSelected { get; set; }

        /// <summary>
        /// 目前現行的圖檔
        /// </summary>
        bool IsActiveCAD { get; set; }

        /// <summary>
        /// 是否產生新版本
        /// </summary>
        bool IsNewVersion { get; set; }

        /// <summary>
        /// 是否是最上層
        /// </summary>
        bool IsRoot { get; set; }

        /// <summary>
        /// 是否是最新版本
        /// </summary>
        bool IsCurrent { get; set; }

        /// <summary>
        /// 是否有版本屬性
        /// </summary>
        bool IsVersion { get; set; }

        /// <summary>
        /// 共用圖檔
        /// </summary>
        bool IsCommonPart { get; set; }

        /// <summary>
        /// 標準圖檔
        /// </summary>
        bool IsStandardPart { get; set; }

        /// <summary>
        /// 限制狀態
        /// </summary>
        string RestrictedStatus { get; set; }
        /// <summary>
        /// 版本狀態
        /// </summary>
        string VersionStatus { get; set; }

        /// <summary>
        /// 存取權
        /// </summary>
        string AccessRights { get; set; }


        /// <summary>
        /// 縮圖圖檔完整名稱
        /// </summary>
        string ClassThumbnail { get; set; }




        /// <summary>
        /// 副檔名
        /// </summary>
        //string extension { get; set; }

        #endregion

        #region "                   事件"


        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"


        #endregion

    }
}
