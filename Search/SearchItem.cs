
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Search
{
    #region "                   宣告區"

    #endregion

    #region "                   屬性"

    /// <summary>
    /// 查詢PLM物件
    /// </summary>
    public class SearchItem : NotifyPropertyBase, ISearchItem
    {
        /// <summary>
        /// CAD類別名稱
        /// </summary>
        public string ClassName { get; set; } = "";

        /// <summary>
        /// CAD類別名稱
        /// </summary>
        public string ItemType { get; set; } = "";

        /// <summary>
        /// 檔名
        /// </summary>
        public string FileName { get; set; } = "";
        /// <summary>
        /// 檔案路徑
        /// </summary>
        public string FilePath { get; set; } = "";
        /// <summary>
        /// CADId
        /// </summary>
        public string ItemId { get; set; } = "";
        /// <summary>
        /// CAD Config Id
        /// </summary>
        public string ItemConfigId { get; set; } = "";

        /// <summary>
        /// 鍵名
        /// </summary>
        public string KeyedName { get; set; } = "";
        

        /// <summary>
        /// 縮圖
        /// </summary>
        public string Thumbnail { get; set; } = "";

        /// <summary>
        /// CAD圖檔讀取Id
        /// </summary>
        public string CadItemId { get; set; } = "";
        /// <summary>
        /// CAD圖檔讀取 Config Id
        /// </summary>
        public string CadItemConfigId { get; set; } = "";


        ///// <summary>
        ///// New Version Id
        ///// </summary>
        //public string newVersionId { get; set; } = "";
        /// <summary>
        /// CAD File Id
        /// </summary>
        public string FileId { get; set; } = "";

        /// <summary>
        /// CAD是否存在PLM (圖檔存在,CAD物件存在) bcs_added_filename 有值
        /// </summary>
        public bool IsExist { get; set; }
        /// <summary>
        /// CAD是否是新增 (圖檔存在,CAD物件存在) : bcs_added_filename 是空值
        /// </summary>
        public bool IsAdded { get; set; } = false;
        /// <summary>
        /// 條件規則屬性
        /// </summary>
        public Dictionary<string, string> RuleProperties { get; set; } = null;

        /// <summary>
        /// 系統欄位和CAD屬性
        /// </summary>
        //public ObservableCollection<PLMProperties> plmProperties { get; set; } = new ObservableCollection<PLMProperties>();
        private ObservableCollection<PLMProperty> _plmProperties = new ObservableCollection<PLMProperty>();
        public ObservableCollection<PLMProperty> PlmProperties
        {
            get { return _plmProperties; }
            //set { SetProperty(ref _plmProperties, value); }//Modify by kenny 2020/08/04
            set { SetProperty(ref _plmProperties, value, nameof(PLMProperty)); }
        }

        /// <summary>
        /// 系統所有版本+版次
        /// </summary>
        //private ObservableCollection<PLMRevision> _plmRevisions = new ObservableCollection<PLMRevision>();
        //public ObservableCollection<PLMRevision> PlmRevisions
        //{
        //    get { return _plmRevisions; }
        //    set { SetProperty(ref _plmRevisions, value, nameof(PlmRevisions)); }
        //}

        /// <summary>
        /// 子階資訊 <子階config_id,List<sort_order>>
        /// </summary>
        private ObservableCollection<CADStructure> _cadStructure = new ObservableCollection<CADStructure>();
        public ObservableCollection<CADStructure> CadStructure
        {
            get { return _cadStructure; }
            set
            {
                SetProperty(ref _cadStructure, value);

            }
        }
        //public Dictionary<string, List<int>> structure { get; set; } = new Dictionary<string, List<int>>();

        /// <summary>
        /// CAD關聯檔案
        /// </summary>
        public List<PLMPropertyFile> PropertyFile { get; set; } = new List<PLMPropertyFile>();

        /// <summary>
        /// View的編輯動作 (當Client CAD整合程式,取得Client的屬性後,會變為true,表示有取過,可以紀錄第二次再取時,不執行)
        /// </summary>
        public bool IsViewEdited { get; set; } = false;
        /// <summary>
        /// View的加入選取動作
        /// </summary>
        public bool IsViewSelected { get; set; } = false;

        /// <summary>
        /// 目前現行的圖檔
        /// </summary>
        public bool IsActiveCAD { get; set; } = false;

        /// <summary>
        /// 是否產生新版本
        /// </summary>
        public bool IsNewVersion { get; set; } = false;

        /// <summary>
        /// 是否是最新版本
        /// </summary>
        public bool IsCurrent { get; set; } = false;

        /// <summary>
        /// 是否有版本屬性
        /// </summary>
        public bool IsVersion { get; set; } = false;

        /// <summary>
        /// 共用圖檔
        /// </summary>
        public bool IsCommonPart { get; set; } = false;

        /// <summary>
        /// 標準圖檔
        /// </summary>
        public bool IsStandardPart { get; set; } = false;

        /// <summary>
        /// 限制狀態
        /// </summary>
        public string RestrictedStatus { get; set; } = SyncRestrictedStatus.None.ToString();
        /// <summary>
        /// 版本狀態
        /// </summary>
        public string VersionStatus { get; set; } = SyncVersionStatus.Uncompared.ToString();

        /// <summary>
        /// 存取權
        /// </summary>
        public string AccessRights { get; set; } = SyncAccessRights.None.ToString();


        /// <summary>
        /// 縮圖圖檔完整名稱
        /// </summary>
        public string ClassThumbnail { get; set; } = "";




        /// <summary>
        /// 副檔名
        /// </summary>
        //public string extension { get; set; } = "";
        #endregion
    }
}
