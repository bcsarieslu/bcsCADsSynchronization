
#region "                   名稱空間"
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.PLMList;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
#endregion


namespace BCS.CADs.Synchronization.ConfigProperties
{
    /// <summary>
    /// 系統欄位和CAD屬性
    /// </summary>
    public class PLMProperty : NotifyPropertyBase, IPLMProperty, IValueConverter, ICloneable
    {
        #region "                   宣告區"

        #endregion

        //related_id,sort_order,bcs_ref_cad_classification,bcs_sync_plm,bcs_sync_cad,bcs_cad_property_classification,bcs_cad_property,bcs_cad_default_property,data_type,data_source,pattern

        //sort_order 序號  integer X       left 	70 	0 		
        //bcs_ref_cad_classification 產品分類    foreign left 	200 	30 	Pattern: bcs_cad_classification
        //bcs_sync_plm    同步PLM boolean                 left 	100 	50 		
        //bcs_sync_cad 同步CAD   boolean left 	100 	60 			
        //bcs_cad_property_classification CAD屬性分類     filter list                 left 	100 	80 	Pattern: bcs_ref_cad_classification
        //bcs_cad_property    CAD屬性 string (512) 				left 	150 	90 		
        //bcs_cad_default_property CAD預設屬性     filter list                 left 	150 	100 	Pattern: bcs_ref_cad_classification
        //data_type   CAD資料類型 list(Data Types)               left 		120 		
        //data_source CAD資料來源     item(ItemType)                 left 		125 		
        //bcs_parameters 參數
        //value 值:取得物件時,寫入內容
        #region "                   屬性"
        /// <summary>
        /// 序號
        /// </summary>
        [XmlSettingTagNameAttribute("sort_order")]
        public int Order { get; set; } = 0;


        /// <summary>
        /// 來源的SearchItem
        /// </summary>
        public SearchItem SoruceSearchItem { get; set; }

        /// <summary>
        /// 同步PLM
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_sync_plm")]
        public bool IsSyncPLM { get; set; } = false;

        /// <summary>
        /// 同步CAD
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_sync_cad")]
        public bool IsSyncCAD { get; set; } = false;

        /// <summary>
        /// CAD屬性分類
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_cad_property_classification")]
        public string CadClass { get; set; } = "";

        /// <summary>
        /// CAD預設屬性名稱
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_cad_property")]
        public string CadName { get; set; } = "";

        /// <summary>
        /// CAD預設屬性
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_cad_default_property")]
        public string CadDefaultName { get; set; } = "";

        /// <summary>
        /// CAD資料類型
        /// </summary>
        [XmlSettingTagNameAttribute("data_type")]
        public string CadDataType { get; set; } = "";

        /// <summary>
        /// CAD資料來源
        /// </summary>
        [XmlSettingAttributeName("data_source")]
        public string CadDataSource { get; set; } = "";


        /// <summary>
        /// 參數
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_parameters")]
        public string Parameters { get; set; } = "";

        /// <summary>
        /// PLM屬性名稱
        /// </summary>
        [XmlSettingAttributeName("property.name")]
        //public string Name { get; set; } = "";
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                this.PropertyName = value;
                SetProperty(ref _name, value, nameof(Name));
            }
        }

        /// <summary>
        /// PLM屬性名稱 (因為Name無法在Grid Header Content,所以新增PropertyName)
        /// </summary>
        public string PropertyName { get; set; }
        

        /// <summary>
        /// PLM資料類型
        /// </summary>
        [XmlSettingTagNameAttribute("property.DataType")]
        public string DataType { get; set; } = "";

        /// <summary>
        /// PLM資料來源
        /// </summary>
        [XmlSettingAttributeName("property.data_source")]
        public string DataSource { get; set; } = "";


        /// <summary>
        /// 樣式
        /// </summary>
        [XmlSettingAttributeName("property.pattern")]
        public string Pattern { get; set; } = "";

        /// <summary>
        /// 鍵名
        /// </summary>
        [XmlSettingAttributeName("property.keyed_name")]
        public string KeyedName { get; set; } = "";

        /// <summary>
        /// 鍵值
        /// </summary>
        public string KeyedId { get; set; } = "";

        /// <summary>
        /// PLM參考資料來源
        /// </summary>
        public string RefDataSource { get; set; }="";

        /// <summary>
        /// 值
        /// </summary>
        private string _dataValue="";
        public string DataValue
        {
            get { return _dataValue; }
            set
            {
                SetProperty(ref _dataValue, value, nameof(DataValue));
                if (IsInitial) return;
                ResetTargetFilterListItems(this);
            }
        }

        /// <summary>
        /// 同步對方的值
        /// </summary>
        public string SyncValue { get; set; } = "";

        /// <summary>
        /// 同步對方的顯示值
        /// </summary>
        public string SyncDisplayValue { get; set; } = "";

        /// <summary>
        /// 同步顏色型態值
        /// </summary>
        private int _syncColorTypeValue = 0;
        public int SyncColorTypeValue
        {
            get { return _syncColorTypeValue; }
            set
            {
                if (IsInitial) return;
                SetProperty(ref _syncColorTypeValue, value, nameof(SyncColorTypeValue));
            }
        }

        public string Value { get; set; } = "";

        /// <summary>
        /// 顯示值
        /// </summary>
        private string _displayValue;
        public string DisplayValue
        {
            get { return _displayValue; }
            set
            {
                if (value != _displayValue)
                {
                    _displayValue = value;
                    SetProperty(ref _displayValue, value, nameof(DisplayValue));
                    OnPropertyChanged("DisplayValue");
                    if (IsInitial) return;

                    ChangeDataValueOnPropertyChange();
                    IsModify = (DataValue != Value);
                    SetProperty(ref _displayValue, value, nameof(DisplayValue));
                }

            }
        }

        /// <summary>
        ///必填
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        ///是否修改
        /// </summary>
        private bool _isModify = false;

        public bool IsModify
        {
            get { return _isModify; }
            set
            {
                if (IsInitial) return;
                SetProperty(ref _isModify, value, nameof(IsModify));


            }
        }

        /// <summary>
        ///是否執行過更新旗標
        /// </summary>
        public bool IsUpdateFlag { get; set; } = false;

        /// <summary>
        ///是否新增
        /// </summary>
        //public bool IsAdd { get; set; } = false;

        /// <summary>
        ///是否初始
        /// </summary>
        public bool IsInitial { get; set; } = true;

        /// <summary>
        ///是否存在
        /// </summary>
        public bool IsExist { get; set; } = false;


        /// <summary>
        ///是否為系統程式新增屬性
        /// </summary>
        public bool IsSystemAdd { get; set; } = false;

        /// <summary>
        /// PLM屬性標籤名稱
        /// </summary>
        public string Label { get; set; } = "";

        /// <summary>
        /// 欄位寬度
        /// </summary>
        public int ColumnWidth { get; set; } = 0;

        /// <summary>
        /// 標籤其他語系
        /// </summary>
        public Dictionary<string, string> Labels { get; set; } = new Dictionary<string, string>();

        ///// <summary>
        ///// 圖檔路徑 (同步檔案的實體路)
        ///// </summary>
        //public string file_path { get; set; } = "";


        /// <summary>
        /// List
        /// </summary>
        private ObservableCollection<PLMListItem> _listItems = new ObservableCollection<PLMListItem>();
        public ObservableCollection<PLMListItem> ListItems
        {
            get { return _listItems; }
            set {
                //System.Diagnostics.Debugger.Break();
                SetProperty(ref _listItems, value, nameof(ListItems));

            }
        }

        /// <summary>
        /// List
        /// </summary>
        private Lists _plmlist ;
        public Lists PLMList
        {
            get { return _plmlist; }
            set
            {
                SetProperty(ref _plmlist, value, nameof(PLMList));
            }
        }


        /// <summary>
        /// 被選到的項目
        /// </summary>
        private PLMListItem _selectedListItem;
        public PLMListItem SelectedListItem
        {
            get { return _selectedListItem; }
            //set { SetProperty(ref _selectedListItem, value); }
            set { SetProperty(ref _selectedListItem, value, nameof(SelectedListItem)); }
        }


        private ICommand _selectedListItemChanged;
        public ICommand SelectedListItemChanged
        {
            get
            {
                _selectedListItemChanged = _selectedListItemChanged ?? new RelayCommand((x) =>
                {
                    //var y = x;
                    ResetTargetFilterListItems(this);

                });
                return _selectedListItemChanged;
            }
        }

        /// <summary>
        /// 目前選到版本的ItemId
        /// </summary>
        public string PLMRevisionItemId { get; set; } = "";

        /// <summary>
        /// 系統所有版本+版次
        /// </summary>
        private ObservableCollection<PLMRevision> _plmRevisions = new ObservableCollection<PLMRevision>();
        public ObservableCollection<PLMRevision> PlmRevisions
        {
            get { return _plmRevisions; }
            set { SetProperty(ref _plmRevisions, value, nameof(PlmRevisions)); }
        }

        private ICommand _selectedPLMRevisionChanged;
        public ICommand SelectedPLMRevisionChanged
        {
            get
            {
                _selectedPLMRevisionChanged = _selectedPLMRevisionChanged ?? new RelayCommand((x) =>
                {
                    //var y = x;
                    //更新PLMRevisionItemId

                });
                return _selectedPLMRevisionChanged;
            }
        }


        #endregion

        #region "                   事件"


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
            }

            Type type = value.GetType();
            if (value is bool)
            {
                if ((bool)value)
                {
                    return (SolidColorBrush)Brushes.Blue;
                }
                //return (SolidColorBrush)Brushes.DeepSkyBlue;
                return (SolidColorBrush)Brushes.Black;
            } else if (value is int)
            {
               switch ((int)value)
                {
                    case (int)SyncColorType.SyncDifferentValues:
                        return (SolidColorBrush)Brushes.LightYellow;
                    case (int)SyncColorType.IsRequired:
                        return (SolidColorBrush)Brushes.LightPink;
                    default:
                        return (SolidColorBrush)Brushes.White;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            dynamic instance = Activator.CreateInstance(value.GetType());
            throw new NotImplementedException();
        }

        #endregion

        #region "                   方法"
        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void ResetSyncColorTypeValue()
        {
            ResetSyncColorTypeValue(DataValue);
        }

        /// <summary>
        /// 重新設定_syncColorTypeValue值
        /// </summary>
        /// <param name="value"></param>
        public void ResetSyncColorTypeValue(string value)
        {
            if (IsRequired) { _syncColorTypeValue = (int)SyncColorType.IsRequired; return; }
            _syncColorTypeValue = (value != SyncValue) ? (int)SyncColorType.SyncDifferentValues : (int)SyncColorType.Defalue;
        }


        #endregion

        #region "                   方法(內部)"

        /// <summary>
        /// 更新DataValue的值(下拉式選項畢動調整)
        /// </summary>
        private void ChangeDataValueOnPropertyChange()
        {
            if (IsInitial == false)
            {
                if (DataSource != "" && ListItems.Count() > 0 && (DataType == "list" || DataType == "filter list"))
                {
                    //System.Diagnostics.Debugger.Break();
                    ResetSyncColorTypeValue((ListItems.Where(x => x.Label == DisplayValue) != null) ? ListItems.Where(x => x.Label == DisplayValue).Select(x => x.Value).FirstOrDefault() : "");
                    DataValue = (ListItems.Where(x => x.Label == DisplayValue) != null) ? ListItems.Where(x => x.Label == DisplayValue).Select(x => x.Value).FirstOrDefault() : "";
                    ResetTargetFilterListItems(this);
                }
                else
                {
                    ResetSyncColorTypeValue(DisplayValue);
                    DataValue = DisplayValue;
                }       
            }
        }

        private void ResetTargetFilterListItems(PLMProperty currentProperty)
        {
            try
            {
                //System.Diagnostics.Debugger.Break();
                if ((currentProperty.DataType != "filter list" && currentProperty.DataType != "list") || currentProperty.SoruceSearchItem == null) return;

                PLMProperty targetProperty = currentProperty.SoruceSearchItem.PlmProperties.Where(x => x.Pattern == currentProperty.Name).FirstOrDefault();
                if (targetProperty == null) return;

                string value = (currentProperty.DataValue == null) ? "" : currentProperty.DataValue;
                List <PLMListItem> filterListItems = targetProperty.PLMList.ListItems.Where(x => x.Filter == value).ToList<PLMListItem>();
                NewCollectionPLMListItems(targetProperty, filterListItems);

                PLMListItem listItem = filterListItems.Where(x => x.Value == targetProperty.DataValue).FirstOrDefault();
                targetProperty.DataValue = (listItem == null)? "" : listItem.Value;
                targetProperty.DisplayValue = (listItem == null) ? "" : listItem.Label;
                targetProperty.SelectedListItem = listItem;

    }
            catch (Exception ex)
            {
                string strError = ex.Message;
                throw ex;
            }
        }

        /// <summary>
        /// 會發生錯誤,因此暫用Try Catch忽略
        /// </summary>
        /// <param name="currentProperty"></param>
        /// <param name="filterListItems"></param>
        private void NewCollectionPLMListItems(PLMProperty currentProperty,List<PLMListItem> filterListItems)
        {
            try
            {
                //currentProperty.ListItem.Clear();   
                currentProperty.ListItems  = (filterListItems.Count == 0) ? new ObservableCollection<PLMListItem>() : new ObservableCollection<PLMListItem>(filterListItems);
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        public static implicit operator List<object>(PLMProperty v)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

