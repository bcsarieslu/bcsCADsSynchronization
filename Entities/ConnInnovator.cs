
#region "                   名稱空間"
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.Windows;

using BCS.CADs.Synchronization;
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.PLMList;
using System.IO;
using System.Collections.ObjectModel;
using BCS.CADs.Synchronization.ViewModels;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Controls;

#endregion


namespace BCS.CADs.Synchronization.Entities
{
    
    /// <summary>
    /// Innovator 相關程式
    /// </summary>
    abstract public class ConnInnovator : NotifyPropertyBase,  IServerConnection, IValueConverter
    {

        #region "                   宣告區"

        /// <summary>
        /// Aras Innovator物件
        /// </summary>
        private Innovator _asInnovator = new Innovator();

        /// <summary>
        /// CAD產品定義
        /// </summary>
        private Product _product { get; set; } = null;

        //private List<PLMKeys> _classesKeys=null;
        Dictionary<string, Dictionary<string, string>> _classesKeys = null;
        Dictionary<string, List<string>> _allKeys = null;
        Dictionary<string, List<ClassTemplateFile>> _classesTemplates = null;
        private Dictionary<string, Lists> _lists = new Dictionary<string, Lists>();

        private Dictionary<string, ItemType> _itemTypes = new Dictionary<string, ItemType>();

        private ItemType _cadItemType = null;
        private ItemType _cadRevisionItemType = null;

        private SyncCADEvents _syncCADEvents = new SyncCADEvents();
        internal SyncCADEvents GetSyncCADEvents
        {
            get
            {
                return _syncCADEvents;
            }
        }
        #endregion

        #region "                   屬性"

        protected internal bool IsResolveAllLightweightSuppres { get; set; } = true;

        protected internal bool IsResolveAllSuppres { get; set; } = true;


        protected internal CommonPartsLibrary PartsLibrary { get; set; }

        /// <summary>
        /// 資料庫名稱下拉選項
        /// </summary>
        private ObservableCollection<PLMListItem> _listItems = new ObservableCollection<PLMListItem>();
		//private ObservableCollection<PLMListItem> _listItems = new ObservableCollection<PLMListItem>() { new PLMListItem {Value="test001" } };
        public ObservableCollection<PLMListItem> ListItems
        {
            get { return _listItems; }
            //set { SetProperty(ref _listItem, value); }
            set { SetProperty(ref _listItems, value, nameof(ListItems)); }
            
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



        /// <summary>
        /// 資料庫名稱下拉選項類型
        /// </summary>
        [XmlSettingTagNameAttribute("property.data_type")]
        public string DataType { get; set; } = "list";


        /// <summary>
        /// 資料庫名稱
        /// </summary>
        private string _Database = "";
        public string Database
        {
            get { return _Database; }
            set
            {
                SetProperty(ref _Database, value, nameof(Database));
            }
        }
        /// <summary>
        /// 是否為AD認證
        /// </summary>
        //public bool IsWinAuth { get; set; } = false;
        private bool _IsWinAuth = false;
        public bool IsWinAuth
        {
            get { return _IsWinAuth; }
            set
            {
                SetProperty(ref _IsWinAuth, value, nameof(IsWinAuth));
            }
        }
        /// <summary>
        /// 登錄使用者名稱
        /// </summary>
        //public string LoginName { get; set; }
        private string _LoginName = "";
		//private string _LoginName = "admin";
        public string LoginName
        {
            get { return _LoginName; }
            set
            {
                SetProperty(ref _LoginName, value, nameof(LoginName));
            }
        }
        /// <summary>
        /// 登錄使用者密碼
        /// </summary>
        //public string Password { get; set; }
        private string _Password = "";
        public string Password
        {
            get { return _Password; }
            set
            {
                SetProperty(ref _Password, value, nameof(Password));
            }
        }

        /// <summary>
        /// 登錄的網址
        /// </summary>
        //public string Url { get; set; }
        private string _Url = "";
		//private string _Url = "http://2017caddev01/115";
        public string Url
        {
            get { return _Url; }
            set
            {
                SetProperty(ref _Url, value, nameof(Url));
            }
        }

        /// <summary>
        /// 使用者是否登錄
        /// </summary>
        //public bool IsActiveLogin { get; set; } = false;
        private bool _IsActiveLogin = false;
        public bool IsActiveLogin
        {
            get { return _IsActiveLogin; }
            set
            {
                SetProperty(ref _IsActiveLogin, value, nameof(IsActiveLogin));
            }
        }


        /// <summary>
        /// 過濾條件
        /// </summary>
        //public string LoginName { get; set; }
        private string _FilterCondition = "";
        internal string FilterCondition
        {
            get { return _FilterCondition; }
            set
            {
                SetProperty(ref _FilterCondition, value, nameof(FilterCondition));
            }
        }


        /// <summary>
        /// 原始檔案屬性
        /// </summary>
        private string _nativeProperty = "native_file";
        internal string NativeProperty {
            get { return _nativeProperty; }
            set { _nativeProperty = value;
                _asInnovator.NativeProperty = _nativeProperty;
            }
        }

        /// <summary>
        /// 縮圖屬性
        /// </summary>
        //internal string ThumbnailProperty { get; set; } = "thumbnail";
        private string _thumbnailProperty = "thumbnail";
        internal string ThumbnailProperty
        {
            get { return _thumbnailProperty; }
            set
            {
                _thumbnailProperty = value;
                _asInnovator.ThumbnailProperty = _thumbnailProperty;
            }
        }



        /// <summary>
        /// CAD軟體名稱
        /// </summary>
        private string _cadSoftware = "";
        internal string CADSoftware {
            get
            {
                return _cadSoftware;
            }
            set
            {
                _cadSoftware = value;
                _asInnovator.CADSoftware = _cadSoftware;
                
            }
        } 

        private string _imageDefaultPath = "";

        /// <summary>
        /// 取得預設Images的位置
        /// </summary>
        internal string ImageDefaultPath {
            get{
                if (_imageDefaultPath =="") _imageDefaultPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + $@"/Broadway/{CADSoftware}/Images/";
                return _imageDefaultPath;
            }
        }


        /// <summary>
        /// Get Aras.IOM.Innovator
        /// </summary>
        internal Aras.IOM.Innovator AsInnovator
        {
            get
            {
                return _asInnovator.AsInnovator;
            }
        }

        /// <summary>
        /// Integration Events
        /// </summary>
        internal List<BCS.CADs.Synchronization.Classes.IntegrationEvents > IntegrationEvents {
            get
            {
                return _product.PdEvents;
            }
                
         }




        #endregion


        /// <summary>
        /// 初始化,載入事件
        /// </summary>
        internal ConnInnovator()
        {

        }

        #region "                   事件"

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value)
            {
                return null;
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
        /// 回收資源
        /// </summary>
        internal void Dispose()
        {

        }

        

        protected internal static string UserName { get; set; }
        protected internal static string Image_Filename { get; set; }
        protected internal Aras.IOM.Item File_Item
        {
            get
            {
                if (!string.IsNullOrEmpty(Image_ID))
                {
                    Aras.IOM.Item it = _asInnovator.AsInnovator.getItemById(ItemTypeName.File.ToString(), Image_ID);
                    Image_Filename = it.getProperty("filename", "");
                    return it;
                }
                return null;
            }
        }


        private string _language;
        private ResourceDictionary _languageResources = null;

        /// <summary>
        /// 取得目前語系資源
        /// </summary>
        protected internal ResourceDictionary LanguageResources
        {
            get
            {
                SetLanguageResources();
                return _languageResources;
            }
        }

        /// <summary>
        /// 設定語系資源
        /// </summary>
        protected internal void SetLanguageResources()
        {
            if (_language != ClsSynchronizer.Language || _languageResources == null)
            {
                _languageResources = new ResourceDictionary();
                _languageResources.Source = new Uri($"pack://application:,,,/BCS.CADs.Synchronization;Component/Lang/{ClsSynchronizer.Language}.xaml", UriKind.Absolute);
                _language = ClsSynchronizer.Language;
                _asInnovator.LanguageResources = _languageResources;
                _syncCADEvents.LanguageResources = _languageResources;
            }
        }

        //csv

        protected internal string GetLanguageByKeyName(string key)
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

        private static string _image_ID;

        protected internal string Image_ID
        {
            get
            {
                if (string.IsNullOrEmpty(_image_ID))
                {
                    Aras.IOM.Item it = _asInnovator.AsInnovator.newItem("User", "get");
                    it.setProperty("login_name", "admin");
                    it = it.apply();

                    UserName = it.getProperty("last_name", "") + " " + it.getProperty("first_name", "");
                    string str = it.getProperty("picture", "");
                    _image_ID = str.Substring(str.IndexOf('=') + 1);
                }
                return _image_ID;
            }
        }

        /// <summary>
        /// 取得圖檔
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        virtual protected internal string  GetImageFullName(string value)
        {
            try
            {
                
                //string imageID = (value!="")? (value.IndexOf("=")>0)? value.Substring(value.IndexOf('=') + 1): value: "";
                string imageID = (String.IsNullOrWhiteSpace(value) ==false) ? (value.IndexOf("=")>0)? value.Substring(value.IndexOf('=') + 1): value: "";
                //if (imageID == "") return "";
                if (String.IsNullOrWhiteSpace(imageID)) return "";
                string path= ImageDefaultPath + imageID ;
                if (Directory.Exists(path)){
                    string[] files = Directory.GetFiles(path);
                    if (files.Length > 0) return files[0];
                }

                string fileName = "";
                if (_asInnovator.DownloadFile(imageID, path, ref fileName)==false) return "" ;

                return Path.Combine(path, fileName);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// 取得圖檔
        /// </summary>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        virtual protected internal string GetImageFullName(SearchItem searchItem, SyncType type)
        {
            try
            {

                var value = searchItem.Thumbnail;
                PLMProperty plmProperty=  searchItem.PlmProperties.Where(x => x.DataType == "revision").FirstOrDefault();
                string itemType = ItemTypeName.CAD.ToString();
                if (plmProperty != null)
                {
                    searchItem.IsVersion = true;
                    
                    if (plmProperty.Value!= plmProperty.DisplayValue)
                    {
                        List<string> list = plmProperty.DisplayValue.Split('.').ToList();
                        Aras.IOM.Item item = _asInnovator.GetRevisionItem(itemType, searchItem.ItemConfigId, list[0], list[1]);
                        value = item.getProperty(ThumbnailProperty, "");
                        if (value == "") return "";
                    }else
                    {
                        Aras.IOM.Item item = AsInnovator.getItemById(itemType, searchItem.ItemId);
                        value = item.getProperty(ThumbnailProperty, "");
                    }
                }else if ( type == SyncType.SyncToPLM)
                {
                    value = "";
                    if (searchItem.CadItemConfigId== searchItem.ItemConfigId && searchItem.CadItemId != "")
                    {
                        Aras.IOM.Item item = AsInnovator.getItemById(itemType, searchItem.CadItemId);
                        value = item.getProperty(ThumbnailProperty, "");
                    }
                    if (value == "") return "";
                }
                else if (type == SyncType.SyncFromPLM)
                {
                    Aras.IOM.Item item = AsInnovator.getItemById(itemType, searchItem.ItemId);
                    value = item.getProperty(ThumbnailProperty, "");
                }

                return GetImageFullName(value);

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return "";
            }
        }

        /// <summary>
        /// 取得所選到版本的SearchItem
        /// </summary>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        virtual protected internal SearchItem GetVersionSearchItem(SearchItem searchItem)
        {
            try
            {

                var value = searchItem.Thumbnail;
                PLMProperty plmProperty = searchItem.PlmProperties.Where(x => x.DataType == "revision").FirstOrDefault();
                if (plmProperty != null)
                {
                    searchItem.IsVersion = true;
                    if (plmProperty.Value != plmProperty.DisplayValue)
                    {
                        List<string> list = plmProperty.DisplayValue.Split('.').ToList();
                        Aras.IOM.Item item = _asInnovator.GetRevisionItem(searchItem.ItemType, searchItem.ItemConfigId, list[0], list[1]);
                        SearchItem versionSearchItem = GetActiveSearchItem(GetPLMSearchItem(item));
                        return versionSearchItem;
                    }
                }

                return searchItem;

            }
            catch (Exception ex)
            {
                //string message = ex.Message;
                throw ex;
            }
        }

        /// <summary>
        /// Aras Innovator Login 
        /// </summary>
        virtual public void Login()
        {
            try
            {

                SetLanguageResources();
                IsActiveLogin = false;
                IsActiveLogin = (IsWinAuth) ? _asInnovator.WinAuthLogin(Url, Database) : _asInnovator.UserLogin(Url, Database, LoginName, Password);
                if (IsActiveLogin) GetConfigurations();

 
                //Events
                List<SearchItem> searchItems = null;
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemLogin.ToString(), searchItems,null, IntegrationEvents, SyncEvents.UserLogin, SyncType.Login);

                if (File_Item != null)
                {
                    //string path = $@"/Broadway/CADImage/LoginImage/{Image_ID}";
                    string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + $@"/Broadway/CADImage/LoginImage/{Image_ID}";
                    try
                    {
                        Directory.GetFiles(path, Image_Filename);
                    }
                    catch
                    {
                        File_Item.checkout(path);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Aras Innovator Logoff
        /// </summary>
        virtual public void  Logoff()
        {
            try
            {
                IsActiveLogin = false;
                bool ret = _asInnovator.UserLogoff();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得Classification
        /// </summary>
        /// <param name="itemType"></param>
        virtual protected internal XDocument GetClassification(string itemType)
        {
            try
            {
                return _asInnovator.GetClassification(itemType);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 被選到的項目
        /// </summary>
        private SearchItem _selectedSearchItem;
        virtual protected internal SearchItem SelectedSearchItem
        {
            get { return _selectedSearchItem; }
            //set { SetProperty(ref _selectedListItem, value); }
            set { SetProperty(ref _selectedSearchItem, value, nameof(SelectedSearchItem)); }
        }

        virtual protected internal Field GetFieldProperties(string keyed_name)
        {
            Aras.IOM.Item it_form = AsInnovator.getItemByKeyedName("Form", "CAD");
            Aras.IOM.Item it_body = AsInnovator.newItem("Body", "get");
            it_body.setProperty("source_id", it_form.getID());
            it_body = it_body.apply();
            Aras.IOM.Item it_field = AsInnovator.newItem("Field", "get");
            it_field.setProperty("source_id", it_body.getID());
            it_field.setProperty("keyed_name", keyed_name);
            it_field = it_field.apply();

            //var fieldProperties = xd.Descendants().Elements().Select(a =>
            //    new Field
            //    {
            //        KeyedName = a.Element("keyed_name").Value,
            //        X = System.Convert.ToDouble(a.Element("x").Value),
            //        Y = System.Convert.ToDouble(a.Element("y").Value)
            //    }
            //);
            var field = new Field
            {
                KeyedName = it_field.getProperty("keyed_name", ""),
                X = System.Convert.ToDouble(it_field.getProperty("x", "0")),
                Y = System.Convert.ToDouble(it_field.getProperty("y", "0"))
            };
            return field;
        }

        /// <summary>
        /// 同步查詢item Type物件
        /// </summary>
        /// <param name="searchItemType"></param>
        /// <param name="LoadFromPLM"></param>
        /// <returns></returns>
        virtual protected internal ObservableCollection<SearchItem> SyncItemTypeSearch(SearchItem searchItemType)
        {
            try
            {
                ObservableCollection<SearchItem> ObsSearchItems = new ObservableCollection<SearchItem>();

                //string propertyName = (LoadFromPLM) ? NativeProperty : "";
                string propertyName = (searchItemType.ItemType == ItemTypeName.CAD.ToString()) ? NativeProperty : "";
                Aras.IOM.Item qureyResult = _asInnovator.GetItemTypeSearch(searchItemType, propertyName);

                int count = qureyResult.getItemCount();

                for (var i = 0; i < qureyResult.getItemCount(); i++)
                {
                    SelectedSearchItem = new SearchItem();
                    //SearchItem searchItem = new SearchItem();
                    SelectedSearchItem.IsAdded = false;
                    SelectedSearchItem.ItemConfigId = qureyResult.getItemByIndex(i).getProperty("config_id", "");
                    SelectedSearchItem.ItemId = qureyResult.getItemByIndex(i).getProperty("id", "");
                    SelectedSearchItem.KeyedName = qureyResult.getItemByIndex(i).getProperty("keyed_name", "");
                    SelectedSearchItem.IsCurrent = (qureyResult.getItemByIndex(i).getProperty("is_current", "0") == "1") ? true : false;
                    
                    //if (propertyName != "") SelectedSearchItem.FileId = qureyResult.getItemByIndex(i).getProperty(propertyName, "");
                    if (String.IsNullOrWhiteSpace(propertyName) == false) SelectedSearchItem.FileId = qureyResult.getItemByIndex(i).getProperty(propertyName, "");

                    SelectedSearchItem.ItemType = searchItemType.ItemType; //Modify by kenny 2020/08/13
                    SelectedSearchItem.ClassThumbnail = GetClassThumbnail(SelectedSearchItem.ItemType);
                    ObservableCollection<PLMProperty> newProperties = new ObservableCollection<PLMProperty>();
                    foreach (PLMProperty property in searchItemType.PlmProperties)
                    {
                        PLMProperty newProperty = property.Clone() as PLMProperty;

                        //if (newProperty.Name != "")
                        if (String.IsNullOrWhiteSpace(newProperty.Name) == false)
                        {
                            newProperty.IsInitial = false;
                            newProperty.IsExist = true;
                            newProperty.DataValue = qureyResult.getItemByIndex(i).getProperty(newProperty.Name, "");
                            
                            newProperty.SoruceSearchItem = SelectedSearchItem;

                            //if (newProperty.DataType=="item" && newProperty.DataSource != "")
                            if (newProperty.DataType == "item" && String.IsNullOrWhiteSpace(newProperty.DataSource) == false)
                            {
                                newProperty.DisplayValue = qureyResult.getItemByIndex(i).getProperty(newProperty.Name + "_keyed_name", "");
                                newProperty.KeyedId = newProperty.DataValue;
                                newProperty.KeyedName = newProperty.DisplayValue;

                            }
                            else if (newProperty.DataType == "classification")
                            {
                                string[] arry = newProperty.DataValue.Split((char)47);
                                newProperty.DisplayValue = arry[arry.Length - 1];
                                newProperty.DataValue = qureyResult.getItemByIndex(i).getProperty(newProperty.Name, "");
                            }
                            else if(newProperty.DataType == "revision")//Modify by kenny 2020/08/05
                            {
                                AddRevisionItem(qureyResult.getItemByIndex(i), newProperty);
                                searchItemType.IsVersion = true;
                            }
                            AddPropertyListItem(newProperty);
                            SetDispalyValue(SelectedSearchItem, newProperty);

                        }


                        newProperties.Add(newProperty);
                    }
                    SelectedSearchItem.PlmProperties = newProperties;
                    ObsSearchItems.Add(SelectedSearchItem);
                }

                return ObsSearchItems;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 取得物件所有版本
        /// </summary>
        /// <param name="searchItemType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        virtual protected internal ObservableCollection<SearchItem> GetAllRevisions(ItemType searchItemType,string itemId)
        {
            try
            {
                ObservableCollection<SearchItem> ObsSearchItems = new ObservableCollection<SearchItem>();

                //string propertyName = (LoadFromPLM) ? NativeProperty : "";
                string propertyName = (searchItemType.Name == ItemTypeName.CAD.ToString()) ? NativeProperty : "";
                Aras.IOM.Item qureyResult = _asInnovator.GetAllRevisions(searchItemType, propertyName, itemId);

                int count = qureyResult.getItemCount();

                for (var i = 0; i < qureyResult.getItemCount(); i++)
                {
                    SelectedSearchItem = new SearchItem();
                    //SearchItem searchItem = new SearchItem();
                    SelectedSearchItem.IsAdded = false;
                    SelectedSearchItem.ItemConfigId = qureyResult.getItemByIndex(i).getProperty("config_id", "");
                    SelectedSearchItem.ItemId = qureyResult.getItemByIndex(i).getProperty("id", "");
                    SelectedSearchItem.KeyedName = qureyResult.getItemByIndex(i).getProperty("keyed_name", "");
                   
                    //if (propertyName != "") SelectedSearchItem.FileId = qureyResult.getItemByIndex(i).getProperty(propertyName, "");
                    if (String.IsNullOrWhiteSpace(propertyName) == false) SelectedSearchItem.FileId = qureyResult.getItemByIndex(i).getProperty(propertyName, "");

                    SelectedSearchItem.ItemType = searchItemType.Name ;//Modify by kenny 2020/08/13
                    SelectedSearchItem.ClassThumbnail = GetClassThumbnail(SelectedSearchItem.ItemType);
                    ObservableCollection<PLMProperty> newProperties = new ObservableCollection<PLMProperty>();
                    foreach (PLMProperty property in searchItemType.PlmProperties)
                    {
                        PLMProperty newProperty = property.Clone() as PLMProperty;

                        //if (newProperty.Name != "")
                        if (String.IsNullOrWhiteSpace(newProperty.Name) == false)
                        {
                            newProperty.IsInitial = false;
                            newProperty.IsExist = true;
                            newProperty.DataValue = qureyResult.getItemByIndex(i).getProperty(newProperty.Name, "");
                            newProperty.SoruceSearchItem = SelectedSearchItem;

                            //if (newProperty.DataType=="item" && newProperty.DataSource != "")
                            if (newProperty.DataType == "item" && String.IsNullOrWhiteSpace(newProperty.DataSource) == false)
                            {
                                newProperty.DisplayValue = qureyResult.getItemByIndex(i).getProperty(newProperty.Name + "_keyed_name", "");
                                newProperty.KeyedId = newProperty.DataValue;
                                newProperty.KeyedName = newProperty.DisplayValue;

                            }
                            else if (newProperty.DataType == "classification")
                            {
                                string[] arry = newProperty.DataValue.Split((char)47);
                                newProperty.DisplayValue = arry[arry.Length - 1];
                                newProperty.DataValue = qureyResult.getItemByIndex(i).getProperty(newProperty.Name, "");
                            }
                            else if (newProperty.DataType == "revision")
                            {
                                newProperty.DisplayValue = qureyResult.getItemByIndex(i).getProperty("major_rev", "") + "." + qureyResult.getItemByIndex(i).getProperty("generation", "");
                                newProperty.DataValue = newProperty.DisplayValue;
                                newProperty.Value = newProperty.DisplayValue;
                                SelectedSearchItem.IsVersion = true;
                            }
                            AddPropertyListItem(newProperty);
                            SetDispalyValue(SelectedSearchItem, newProperty);

                        }


                        newProperties.Add(newProperty);
                    }
                    SelectedSearchItem.PlmProperties = newProperties;
                    ObsSearchItems.Add(SelectedSearchItem);
                }

                return ObsSearchItems;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
        }


        /// <summary>
        /// 加入版本項目
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        private void AddRevisionItem(Aras.IOM.Item item, PLMProperty property)//2020/08/05
        {
            //select.Add("major_rev");
            //select.Add("generation");
            property.IsSystemAdd = true;
            //PLMRevision plmRevision = new PLMRevision();
            //plmRevision.IsSelected = true;
            //plmRevision.ItemId = item.getProperty("id", "");
            //plmRevision.ItemConfigId = item.getProperty("config_id", "");
            //plmRevision.MajorRevision = item.getProperty("major_rev", "");
            //plmRevision.Generation = item.getProperty("generation", "");
            //plmRevision.Revision = plmRevision.MajorRevision + "." + plmRevision.Generation;
            //property.PlmRevisions.Add(plmRevision);
            //property.DisplayValue = plmRevision.Revision;
            property.DisplayValue = item.getProperty("major_rev", "") + "." + item.getProperty("generation", "");
        }

        /// <summary>
        /// 取得版本
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        virtual protected internal string GetVersion(string itemType, string itemId)
        {
            try
            {
                Aras.IOM.Item item = AsInnovator.getItemById(itemType, itemId);

                return $"{item.getProperty("major_rev")}.{item.getProperty("generation")}";
                
            }
            catch (Exception ex)
            {
                //string strError = ex.Message;
                throw ex;
            }
        }


        /// <summary>
        /// 同步PLM屬性
        /// </summary>
        /// <param name="classItemName"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> SynToPLMItemProperties(List<SearchItem> searchItems, IsLock isLock)
        {
            try
            {

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItems,null, IntegrationEvents, SyncEvents.OnSyncToPLMBefore, SyncType.Properties);

                foreach (SearchItem searchItem in searchItems)
                {

                    if (_asInnovator.SynToPLMItemProperties(searchItem,IntegrationEvents, isLock) == false)
                    {
                        //events
                    }
                    //events
                }

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnSyncToPLMAfter, SyncType.Properties);
                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 鎖定CAD物件
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> LockItems(List<SearchItem> searchItems)
        {
            try
            {
                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(),  searchItems, null, IntegrationEvents, SyncEvents.OnLockItemsBefore, SyncType.Locked);

                foreach (SearchItem searchItem in searchItems)
                {

                    if (_asInnovator.LockItem(searchItem, IntegrationEvents,IsLock.True) == false)
                    {
                        //events
                    }
                    //events
                }

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(),  searchItems, null, IntegrationEvents, SyncEvents.OnLockItemsAfter, SyncType.Locked);

                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// CAD物件解鎖
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> UnlockItems(List<SearchItem> searchItems)
        {
            try
            {
                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemItemLocked.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnUnlockItemsBefore, SyncType.Unlocked);

                foreach (SearchItem searchItem in searchItems)
                {

                    //if (_asInnovator.UnlockItem(searchItem, IntegrationEvents) == false)
                    if (_asInnovator.LockItem(searchItem, IntegrationEvents,IsLock.False) == false)
                    {
                        //events
                    }
                    //events
                }

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemItemLocked.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnUnlockItemsAfter, SyncType.Unlocked);

                return null;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 取得CAD圖檔在PLM系統的基本查詢屬性建立(含是否存在)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> GetPLMSearchItems(Dictionary<string, List<string>> search)
        {
            try
            {

                List <SearchItem> searchItems = new List<SearchItem>();

                List<ConditionalRule> rules = _product.PdRules;
                string strRules = "";
                rules?.ForEach(property =>{strRules += ",a." + property.PrepertyName;});

                foreach (KeyValuePair<string, List<string>> classItem in search)
                {

                    string strConditions = AddClassItemKeysConditions(classItem.Key, "a.", ref strRules);

                    Aras.IOM.Item qureyResult = _asInnovator.GetPLMSearchItems(" a.id,a.config_id,a.is_current,a." + NativeProperty + ",bcs_added_filename as filename{0} from [innovator].[CAD] as a where a.is_current=1 and a.bcs_added_filename in ({1}){2}", strRules, "N'" + String.Join("',N'", classItem.Value) + "'", strConditions);
                    AddSearchItemByQureyResult(qureyResult, rules, classItem, ref searchItems);

                    if (classItem.Value?.Count>0)
                    {
                        qureyResult = _asInnovator.GetPLMSearchItems(" a.id,a.config_id,a.is_current,a." + NativeProperty + ", b.filename{0} from [innovator].[CAD] as a left join [innovator].[File] as b on a." + NativeProperty + "=b.id where a.is_current=1 and b.filename in ({1}){2}", strRules, "N'" + String.Join("',N'", classItem.Value) + "'", strConditions);
                        AddSearchItemByQureyResult(qureyResult, rules, classItem, ref searchItems);
                    }

                    classItem.Value?.ForEach(listItem => {
                        SearchItem searchItem = new SearchItem();
                        searchItem.IsAdded = false;
                        searchItem.IsExist = false;
                        searchItem.FileName = listItem;
                        searchItem.ItemConfigId = "";
                        searchItem.KeyedName = ""; 
                        searchItem.ItemId =  "";
                        searchItem.ClassName = (classItem.Key!="")? classItem.Key:"";
                        searchItem.ItemType = ItemTypeName.CAD.ToString();//Modify by kenny 2020/08/13
                        searchItem.ClassThumbnail = GetClassThumbnail(searchItem.ClassName);
                        searchItems.Add(searchItem);
                    });
                    
                }

                return searchItems;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// SearchItems不存在Ids時,則重新取得相關Ids
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="properties"></param>
        /// <param name="plmPropertyFile"></param>
        virtual protected internal void GetSearchItemsCADIds(SyncType type, List<SearchItem> searchItems)
        {

            try
            {
                
                //List<string> itemFileNames = searchItems.Where(x => x.ItemConfigId == "" && x.FileName != "").Select(x => x.FileName).ToList();
                List<string> itemFileNames = searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemConfigId) && String.IsNullOrWhiteSpace(x.FileName) ==false).Select(x => x.FileName).ToList();
                if (itemFileNames.Count() < 1) return;


                List<ConditionalRule> rules = _product.PdRules;
                string strRules = "";
                rules?.ForEach(property => { strRules += ",b." + property.PrepertyName; });

                List<string> select = new List<string>();
                _asInnovator.AddCADSelectDefaultProperties(rules, select);
                string selectValue = (select.Count()>0)? ",b." + String.Join(",b.", select):"";


                //foreach (ClassItem classItem in _product.PdClassItem?.Where(c => c.Name != ""))
                foreach (ClassItem classItem in _product.PdClassItems?.Where(c => String.IsNullOrWhiteSpace(c.Name) ==false))
                {

                    //Modify by kenny 2020/08/20 bcs_library_path
                    string strConditions = AddClassItemKeysConditions(classItem.Name, "b.",ref strRules);
                    string strSql = $" b.{ThumbnailProperty}{selectValue},b.modified_on,b.locked_by_id,b.config_id,b.keyed_name,b.id,b.is_current,b.bcs_added_filename,c.filename,c.id as native_file{"{0}"} from[innovator].[CAD] as b left join[innovator].[FILE] as c on b.{NativeProperty} =c.id and b.is_current=1";
                    strSql += " where ((c.filename in ({1}) and not c.filename is null) or b.BCS_ADDED_FILENAME in ({1})) and b.is_current=1 {2} order by modified_on desc";

                    Aras.IOM.Item qureyResult = _asInnovator.GetPLMSearchItems(strSql, strRules, "N'" + String.Join("',N'", itemFileNames) + "'", strConditions);


                    for (var i = 0; i < qureyResult.getItemCount(); i++)
                    {
                        SearchItem searchItem = searchItems.Where(x => x.FileName == qureyResult.getItemByIndex(i).getProperty("filename", "")).FirstOrDefault();
                        if (searchItem == null)
                        {
                            searchItem = searchItems.Where(x => x.FileName == qureyResult.getItemByIndex(i).getProperty("bcs_added_filename", "")).FirstOrDefault();
                            if (searchItem == null) continue;
                        }
                        
                        //if (searchItem.ItemId != "") continue;
                        if (String.IsNullOrWhiteSpace(searchItem.ItemId) ==false) continue;

                        searchItem.IsCurrent = (qureyResult.getItemByIndex(i).getProperty("is_current", "0") == "1") ? true : false;
                        searchItem.ItemConfigId = qureyResult.getItemByIndex(i).getProperty("config_id", "");
                        searchItem.KeyedName = qureyResult.getItemByIndex(i).getProperty("keyed_name", "");
                        searchItem.ItemId = qureyResult.getItemByIndex(i).getProperty("id", "");
                        searchItem.FileId = qureyResult.getItemByIndex(i).getProperty("native_file", "");

                        //Modify by kenny 2020/08/20
                        searchItem.LibraryPath = qureyResult.getItemByIndex(i).getProperty("bcs_library_path", "");
                        searchItem.IsCommonPart = System.Convert.ToBoolean(int.Parse(qureyResult.getItemByIndex(i).getProperty("bcs_is_common_part", "0")));
                        //searchItem.IsStandardPart = System.Convert.ToBoolean(int.Parse(qureyResult.getItemByIndex(i).getProperty("bcs_is_standard_part", "0")));
                        searchItem.IsStandardPart = System.Convert.ToBoolean(int.Parse(qureyResult.getItemByIndex(i).getProperty("is_standard", "0")));
                        if (searchItem.PropertyFile.Count() < 1)
                        {
                            if (searchItem.ClassName == "") searchItem.ClassName = GetClassName(Path.GetExtension(searchItem.FileName));
                            List<PLMPropertyFile> propertyFile = _product.PdClassItems?.Where(c => c.Name == searchItem.ClassName)?.First()?.CsPropertyFile;
                            _asInnovator.AddPLMPropertyFiles(searchItem, propertyFile);
                        }

                        searchItem.AccessRights = (qureyResult.getItemByIndex(i).getProperty("locked_by_id", "") == "") ? SyncAccessRights.None.ToString() : (qureyResult.getItemByIndex(i).getProperty("locked_by_id", "") == AsInnovator.getUserID()) ? SyncAccessRights.FlaggedByMe.ToString() : SyncAccessRights.FlaggedByOthers.ToString();
                        searchItem.RuleProperties = GetRuleProperties(rules, qureyResult.getItemByIndex(i));
                        searchItem.RestrictedStatus = GetRestrictedStatus(searchItem, rules, type).ToString();
                        searchItem.VersionStatus = GetVersionStatus(searchItem).ToString();

                        searchItem.Thumbnail = qureyResult.getItemByIndex(i).getProperty(ThumbnailProperty, "");
                        
                    }

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 重新設定版本狀態
        /// </summary>
        /// <param name="searchItems"></param>
        virtual protected internal void ResetVersionStatus(List<SearchItem> searchItems)
        {

            try
            {

                foreach (SearchItem searchItem in searchItems)
                {
                    searchItem.VersionStatus = GetVersionStatus(searchItem).ToString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得CAD圖檔在PLM系統的基本查詢屬性建立(含是否存在)AddNewSearchItem(searchItems, item, rules, "", addedFilename, itemId, fileId);
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> GetPLMSearchItem(Aras.IOM.Item item)
        {
            try
            {

                List<SearchItem> searchItems = new List<SearchItem>();
                List<ConditionalRule> rules = _product.PdRules;

                string addedFilename;
                string fileId = item.getProperty(NativeProperty, "");
                string itemId = item.getID();
                addedFilename = item.getProperty("bcs_added_filename", "");
                SearchItem searchItem = AddNewSearchItem(searchItems, item, rules, "", addedFilename, itemId, fileId);
                searchItem.FileName = "";
                Aras.IOM.Item fileItem = AsInnovator.getItemById(ItemTypeName.File.ToString(), fileId);
                if (fileItem.isError() == false) searchItem.FileName = fileItem.getProperty("filename","");
                
                //if (searchItem.ClassName == "") searchItem.ClassName = GetClassKey(item);
                if (String.IsNullOrWhiteSpace(searchItem.ClassName)) searchItem.ClassName = GetClassKey(item);
                searchItem.ClassThumbnail = GetClassThumbnail(searchItem.ClassName);
                searchItem.RuleProperties = GetRuleProperties(rules, item);

                SearchItem qurerySearchItem = searchItems.Where(x => x.ItemId == searchItem.ItemId).FirstOrDefault();
                if (qurerySearchItem==null) searchItems.Add(searchItem);

                return searchItems;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 共用圖檔的基本屬性
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        virtual protected internal SearchItem GetCommonPartSearchItem(string path, string fileName)
        {
            try
            {

                SearchItem searchItem = new SearchItem();
                searchItem.ItemType = ItemTypeName.CAD.ToString();
                searchItem.FileName = fileName;
                searchItem.LibraryPath = path;
                searchItem.FilePath = path;
                searchItem.IsCommonPart = true;
                return searchItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得PLM的CAD物件屬性定義
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        virtual protected internal List<PLMProperty> GetCADProperties(SearchItem searchItem ,string className)
        {
            try
            {
                if (_product == null) return null;

                List<PLMProperty> properties = _product.PdClassItems?.Where(c => c.Name == className)?.First()?.CsProperties;

                List<PLMProperty> newProperties = new List<PLMProperty>();
                foreach (PLMProperty property in properties)
                {
                    PLMProperty newProperty = property.Clone() as PLMProperty;
                    newProperty.SoruceSearchItem = searchItem;
                    newProperty.IsInitial = false;
                    newProperties.Add(newProperty);

                }

                return newProperties;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得PLM的CAD物件屬性值for SearchItem
        /// </summary>
        /// <param name="searchItem"></param>
        virtual protected internal void GetCADProperties(SearchItem searchItem)
        {
            try
            {
                if (_product == null) return;// null;
                if (searchItem.ClassName == "") searchItem.ClassName = GetClassName(Path.GetExtension(searchItem.FileName));
                List<PLMProperty> properties = _product.PdClassItems?.Where(c => c.Name == searchItem.ClassName)?.First()?.CsProperties;
                List<PLMPropertyFile> propertyFile = _product.PdClassItems?.Where(c => c.Name == searchItem.ClassName)?.First()?.CsPropertyFile;
                List<SearchItem> searchItems = new List<SearchItem>();
                searchItems.Add(searchItem);
                _asInnovator.GetCADProperties(searchItems, properties, propertyFile);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得PLM的CAD物件屬性值for List<SearchItem>
        /// </summary>
        /// <param name="searchItems"></param>
        virtual protected internal void GetCADProperties(List<SearchItem> searchItems,bool isSelected)
        {
            try
            {
                if (_product == null) return;
                if (searchItems == null) return;
                foreach (ClassItem classItem in _product.PdClassItems)
                {
                    List<PLMProperty> properties = _product.PdClassItems?.Where(c => c.Name == classItem.Name)?.First()?.CsProperties;
                    List<SearchItem> searchItemsFilter = searchItems.Where(c => c.ClassName == classItem.Name).ToList() as List<SearchItem>;
                    List<PLMPropertyFile> propertyFile = _product.PdClassItems?.Where(c => c.Name == classItem.Name)?.First()?.CsPropertyFile;
                    if (searchItemsFilter.Count < 1) continue;

                    if (isSelected==true) SetSearchItemsFilter(classItem, ref searchItemsFilter);
                    _asInnovator.GetCADProperties(searchItemsFilter, properties, propertyFile);
                     
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得範本屬性
        /// </summary>
        /// <param name="obslistTemplates"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        virtual protected internal ObservableCollection<SearchItem> GetTemplatesProperties(ObservableCollection<ClassTemplateFile> obslistTemplates,string filePath)
        {
            try
            {
                //Events
                ObservableCollection<SearchItem> ObsSearchItems = new ObservableCollection<SearchItem> ();
                List<ConditionalRule> rules = _product.PdRules;

                foreach (ClassTemplateFile calssTemplateFile in obslistTemplates)
                {

                    
                    for (int i=0;i< calssTemplateFile.Quantity; i++)
                    {
                        SearchItem searchItem = new SearchItem();
                        searchItem.IsAdded = true;
                        searchItem.IsExist = false;
                        searchItem.FileName = calssTemplateFile.FileName;
                        searchItem.ItemConfigId = "";
                        searchItem.KeyedName = ""; 
                        searchItem.ItemId = "";
                        searchItem.ItemType = ItemTypeName.CAD.ToString(); //Modify by kenny 2020/08/13
                        searchItem.ClassName = calssTemplateFile.ClassName;
                        searchItem.ClassThumbnail = GetClassThumbnail(searchItem.ClassName);
                        searchItem.FileId = calssTemplateFile.FileId;
                        Dictionary<string, string> ruleProperty = new Dictionary<string, string>();
                        List<PLMProperty> plmProperties = GetCADProperties(searchItem,calssTemplateFile.ClassName);
                        ObservableCollection<PLMProperty> newProperties = new ObservableCollection<PLMProperty>(plmProperties);

                        AddNewFileNameProperty(searchItem, newProperties, GetLanguageByKeyName("label_NewFileName"),"");
                        searchItem.FilePath = filePath;
                        searchItem.PlmProperties = newProperties;
                        ObsSearchItems.Add(searchItem);
                    }
                }
                return ObsSearchItems;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 複製轉新增的圖檔名稱
        /// </summary>
        /// <param name="searchItems"></param>
        virtual protected internal void SearchItemsCopyFileNameProperty(List<SearchItem> searchItems)
        {
            try
            {
                if (searchItems == null) return;
                foreach (SearchItem searchItem in searchItems.Where(x=>x.IsViewSelected==true))
                {
                    ObservableCollection<PLMProperty> plmProperties = (searchItem.PlmProperties.Count > 0) ? searchItem.PlmProperties : new ObservableCollection<PLMProperty>();
                    AddNewFileNameProperty(searchItem, plmProperties, GetLanguageByKeyName("label_CopyFileName"), "");
                    searchItem.PlmProperties = plmProperties;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得PLM的CAD所有結構
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> CADItemStructure(string itemId, SyncType type)
        {
            try
            {

                Aras.IOM.Item item = _asInnovator.AsInnovator.getItemById(ItemTypeName.CAD.ToString(), itemId);
                List<SearchItem> searchItems = GetPLMSearchItem(item);
                
                SearchItem searchItem = searchItems.First();
                searchItem.IsActiveCAD = true;

                return  CADItemsStructure(searchItems, type);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        virtual protected internal string GetFileNameByCADItemId(string itemId)
        {
            try
            {

                Aras.IOM.Item item = _asInnovator.AsInnovator.getItemById(ItemTypeName.CAD.ToString(), itemId);
                string fileId= item.getProperty(NativeProperty, "");
                if (fileId == "") return "";
                Aras.IOM.Item file =  _asInnovator.AsInnovator.getItemById(ItemTypeName.File.ToString(), fileId);
                return file.getProperty("filename", ""); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        virtual protected internal bool CopyToAdd(List<SearchItem> searchItems, string directory, SyncType type,bool isLastVersion)
        {
            try
            {
                //ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("CopyToAdd", "", "", "Start");

                string newFileName ="";

                //foreach (SearchItem searchItem in searchItems.Where(x => x.ItemId != "" && x.FileId != ""))
                foreach (SearchItem searchItem in searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemId) ==false && String.IsNullOrWhiteSpace(x.FileId) ==false))
                {

                    string fileName = searchItem.FileName;
                    if (searchItem.IsActiveCAD==true)
                    {
                        
                        //PLMProperties property = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename" && x.DisplayValue != "").SingleOrDefault();
                        PLMProperty property = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) ==false).SingleOrDefault();
                        fileName = (property!=null)? property.DisplayValue + Path.GetExtension(searchItem.FileName) : fileName;
                        newFileName = fileName;
                    }

                    Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);

                    if (isLastVersion)
                        item = (item.getProperty("is_current", "0") == "0") ? _asInnovator.GetLastItem(item) : item;

                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(fileName, "Download", "", "Start");
                    if (_asInnovator.DownloadFile(searchItem, IntegrationEvents, SyncEvents.OnLoadFromPLMDownloadBefore, item, searchItem.FileId, directory, fileName) == true)
                    {
                        //searchItem.FilePath = directory;
                        //fileMessage.Status = "Finish";
                    }
                    fileMessage.Status = "Finish";
                }

                //Modify by kenny 2020/08/23
                ////Events
                //_syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemCopyToAdd.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnCopyToAddBefore, SyncType.CopyToAdd);
                

                //Clinet CAD更新圖檔名稱
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.CopyToAdd.ToString(), ref searchItems, null, null, SyncEvents.None, type);

                //調整:不更新PLM系統 : 2020/07/23
                /*
                ////Events
                //_syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemCopyToAdd.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnCopyToAddBefore, SyncType.CopyToAdd);

                _asInnovator.CopyToAddItems(searchItems, IntegrationEvents);

                //Clinet CAD圖檔的itemid及configid
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.UpdateCADKeys.ToString(), ref searchItems, null, null, SyncEvents.None, type);

                //CAD圖檔的itemid及configid更新後,上傳圖檔
                _asInnovator.UploadFiles(searchItems);
                */

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemCopyToAdd.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnCopyToAddBefore, SyncType.CopyToAdd);
                //itemMessage.Status = "End";

                OpenFile(directory, newFileName);


                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 僅提供開啟檔案,不做作任何事
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        protected internal void OpenFile(string path, string fileName)
        {
            try
            {
                //Events
                SearchItem searchItem = new SearchItem();
                searchItem.FilePath = path;
                searchItem.FileName = fileName;

                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.OpenFile.ToString(), ref searchItem, null, null, SyncEvents.OpenFile, SyncType.None);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 取得PLM的CAD所有結構 :最新(後)版本 (1)CAD結構比較 (2)Load From PLM (for List<SearchItem>)
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> CADItemsStructure(List<SearchItem> searchItems, SyncType type)
        {
            try
            {

                List<ConditionalRule> rules = _product.PdRules;
                string strRules = "";
                rules?.ForEach(property => { strRules += ",b." + property.PrepertyName; });

                List<string> select = new List<string>();
                _asInnovator.AddCADSelectDefaultProperties(rules, select);
                string selectValue = (select.Count() > 0) ? ",b." + String.Join(",b.", select) : "";

                string strConditions = "";
                GetRuleKeys(ref strRules,ref strConditions);

                //List<string> compare = searchItems.Where(x => x.ItemId != "").Select(x => x.ItemId).ToList<string>();
                List<string> compare = searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemId) ==false).Select(x => x.ItemId).ToList<string>();

                //Modify by kenny 2010/08/12
                //檢查是否是下載指定版本(預設全部是最新版本)
                SearchItem activeSearchItem = GetActiveSearchItem(searchItems);
                bool isLastVersion = activeSearchItem.IsCurrent;

                if (compare.Count == 0)
                {
                    //if (activeSearchItem.CadItemId != "") { compare = new List<string>(); compare.Add(activeSearchItem.CadItemId); }
                    if (activeSearchItem.CadItemId != "") { compare = new List<string>(); compare.Add(activeSearchItem.CadItemId); }
                    else if (String.IsNullOrWhiteSpace(activeSearchItem.CadItemId) ==false) { compare = new List<string>(); compare.Add(activeSearchItem.CadItemConfigId); }
                    if (compare.Count > 0)
                    {
                        Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), compare[0]);
                        if (item != null && item.isError() == false)
                        {
                            activeSearchItem.ItemId = compare[0];
                            activeSearchItem.ItemConfigId = item.getProperty("config_id", "");
                            activeSearchItem.KeyedName = item.getProperty("keyed_name", ""); 
                            activeSearchItem.IsCurrent = (item.getProperty("is_current", "0") == "1") ? true : false;
                            
                            //activeSearchItem.IsAdded = (activeSearchItem.FileName == "") ? true : false;
                            activeSearchItem.IsAdded = (String.IsNullOrWhiteSpace(activeSearchItem.FileName)) ? true : false;
                            
                            activeSearchItem.IsExist = true;
                            activeSearchItem.ClassName = GetClassKey(item);
                            activeSearchItem.ClassThumbnail = GetClassThumbnail(activeSearchItem.ClassName);
                            activeSearchItem.FileId = item.getProperty(NativeProperty, "") ;
                            activeSearchItem.AccessRights = (item.getProperty("locked_by_id", "") == "") ? SyncAccessRights.None.ToString() : (item.getProperty("locked_by_id", "") == AsInnovator.getUserID()) ? SyncAccessRights.FlaggedByMe.ToString() : SyncAccessRights.FlaggedByOthers.ToString();
                            activeSearchItem.RuleProperties = GetRuleProperties(rules, item);
                            activeSearchItem.RestrictedStatus = GetRestrictedStatus(activeSearchItem, rules, type).ToString();
                            activeSearchItem.VersionStatus = GetVersionStatus(activeSearchItem).ToString();
                            activeSearchItem.Thumbnail = item.getProperty(ThumbnailProperty, "");

                            //Modify by kenny 2020/08/20 bcs_library_path
                            activeSearchItem.LibraryPath = item.getProperty("bcs_library_path", "");
                            activeSearchItem.IsCommonPart = System.Convert.ToBoolean(int.Parse(item.getProperty("bcs_is_common_part", "0")));
                            //activeSearchItem.IsStandardPart = System.Convert.ToBoolean(int.Parse(item.getProperty("bcs_is_standard_part", "0")));
                            activeSearchItem.IsStandardPart = System.Convert.ToBoolean(int.Parse(item.getProperty("is_standard", "0")));
                            //2d圖檔,取得關連
                            if (activeSearchItem.ClassName == "Drawing")
                            {
                                _asInnovator.Get2DDrawingCADSourceItem(searchItems, activeSearchItem);
                                
                                //compare = searchItems.Where(x => x.ItemId != "").Select(x => x.ItemId).ToList<string>();
                                compare = searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemId) ==false).Select(x => x.ItemId).ToList<string>();
                            }
                        }
                    }
                }

                
                //List<string> items = searchItems.Where(x => x.ItemId != "").Select(x => x.ItemId).ToList<string>();
                List<string> items = searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemId) ==false).Select(x => x.ItemId).ToList<string>();
                SearchItem searchItem ;

                do {

                    //Modify by kenny 2020/08/20 bcs_library_path
                    string strSql = $" a.source_id,a.related_id,a.bcs_instance_id{selectValue},b.{ThumbnailProperty},b.locked_by_id,b.config_id,b.id,b.is_current,b.bcs_added_filename,c.filename,c.id as native_file,a.sort_order{"{0}"} from[innovator].[{SyncLinkItemTypes.CAD_Structure.ToString().ToUpper()}] as a left join[innovator].[CAD] as b on a.related_id=b.id left join[innovator].[FILE] as c on b.{NativeProperty} =c.id ";
                    strSql += "where a.source_id in ({1}) and not b.id is null {2} order by a.source_id,a.related_id,a.sort_order asc";
                    Aras.IOM.Item qureyResult = _asInnovator.GetPLMSearchItems(strSql, strRules, "N'" + String.Join("',N'", compare) + "'", strConditions);

                    List<string> result = new List<string>();
                    if (qureyResult.isError()==false) {
                        for (var i = 0; i < qureyResult.getItemCount(); i++)
                        {
                            
                            string sourceId = "";
                            SearchItem sourceItem = searchItems.Where(x => x.ItemId == qureyResult.getItemByIndex(i).getProperty("source_id", "")).ToList<SearchItem>()[0];
                            do//取過的sourceItem不再重新執行.Where (對效能有很大影響)
                            {
                                //if (sourceId != "") i++;
                                if (String.IsNullOrWhiteSpace(sourceId) ==false) i++;
                                sourceId =qureyResult.getItemByIndex(i).getProperty("source_id", "");
                                qureyResult.getItemByIndex(i).getProperty("source_id", "");

                                //判斷是否是最新(後)版本,如果不是,則最後版本
                                string itemId;
                                string addedFilename;
                                string fileId = qureyResult.getItemByIndex(i).getProperty(NativeProperty, "");
                                //Modify by kenny 2010/08/12
                                //if (qureyResult.getItemByIndex(i).getProperty("is_current", "0") == "0")
                                if (qureyResult.getItemByIndex(i).getProperty("is_current", "0") == "0" && isLastVersion==true)
                                {
                                    Aras.IOM.Item lastItem = _asInnovator.GetLastItem(ItemTypeName.CAD.ToString(), qureyResult.getItemByIndex(i).getProperty("config_id", ""));
                                    itemId = lastItem.getID();
                                    addedFilename = lastItem.getProperty("bcs_added_filename", "");
                                    fileId = lastItem.getProperty(NativeProperty, "");
                                }
                                else
                                {
                                    itemId= qureyResult.getItemByIndex(i).getProperty("id", "");
                                    addedFilename = qureyResult.getItemByIndex(i).getProperty("bcs_added_filename", "");
                                }

                                List<SearchItem> searchItemList = searchItems.Where(x => x.ItemId == itemId).ToList<SearchItem>();
                                searchItem = (searchItemList.Count() > 0) ? searchItemList[0] : AddNewSearchItem(searchItems, qureyResult.getItemByIndex(i), rules, "", addedFilename, itemId, fileId);
                                if (new List<string> { itemId }.Except(result).ToList().Count() > 0) result.Add(itemId);
                                searchItem.AccessRights = (qureyResult.getItemByIndex(i).getProperty("locked_by_id", "") == "") ? SyncAccessRights.None.ToString() : (qureyResult.getItemByIndex(i).getProperty("locked_by_id", "") == AsInnovator.getUserID()) ? SyncAccessRights.FlaggedByMe.ToString() : SyncAccessRights.FlaggedByOthers.ToString();
                                searchItem.RuleProperties = GetRuleProperties(rules, qureyResult.getItemByIndex(i));
                                searchItem.RestrictedStatus = GetRestrictedStatus(searchItem, rules, type).ToString();
                                searchItem.VersionStatus = GetVersionStatus(searchItem).ToString();


                                //子階id改為config_id
                                string relatedConfigId = qureyResult.getItemByIndex(i).getProperty("config_id", "");
                                do//取過structure的不再重新執行.Where (對效能有很大影響)
                                {

                                    CADStructure newCADStructure = new CADStructure();
                                    newCADStructure.Order = int.Parse(qureyResult.getItemByIndex(i).getProperty("sort_order", "0"));
                                    newCADStructure.ItemConfigId = qureyResult.getItemByIndex(i).getProperty("config_id", "");
                                    newCADStructure.Child = searchItem;
                                    newCADStructure.InstanceId = qureyResult.getItemByIndex(i).getProperty("bcs_instance_id", "");
                                    sourceItem.CadStructure.Add(newCADStructure);
                                    i++;
                                    if (i + 1 >= qureyResult.getItemCount()) break;
                                } while (sourceId == qureyResult.getItemByIndex(i + 1).getProperty("source_id", "") && relatedConfigId == qureyResult.getItemByIndex(i + 1).getProperty("config_id", ""));

                                i--;

                                if (i + 1 >= qureyResult.getItemCount()) break;
                            } while (sourceId == qureyResult.getItemByIndex(i+1).getProperty("source_id", ""));

                        }
                    }

                    compare = result.Except(items).ToList();
                    
                    //items = searchItems.Where(x => x.ItemId != "").Select(x => x.ItemId).ToList<string>();
                    items = searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemId) ==false).Select(x => x.ItemId).ToList<string>();

                } while (compare.Count() > 0);

                return searchItems;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// 從PLM載入
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        virtual protected internal List<SearchItem> LoadFromPLM(string directory, List<SearchItem> searchItems, SyncType type)
        {
            try
            {
                //Events
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage(type.ToString(), "", "Before Events", "Start");
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemLoadFromPLM.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnLoadFromPLMDownloadBefore, SyncType.Download);

                
                //foreach (SearchItem searchItem in searchItems.Where(x => x.ItemId != "" && x.FileId!=""))
                foreach (SearchItem searchItem in searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemId) ==false && String.IsNullOrWhiteSpace(x.FileId) ==false))
                //foreach (SearchItem searchItem in searchItems.Where(x => x.ItemId != "" ))
                {
                    Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);
                    item = (item.getProperty("is_current", "0") == "0") ? _asInnovator.GetLastItem(item) : item;
                    
                    //if (searchItem.FileName == "")
                    if (String.IsNullOrWhiteSpace(searchItem.FileName))
                    {
                        Aras.IOM.Item fileItem =  AsInnovator.getItemById(ItemTypeName.File.ToString(), searchItem.FileId);
                        searchItem.FileName = fileItem.getProperty("filename","");
                    }

                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Download", "", "Start");

                    IntegrationEvents integrationEvent1 = null;
                    _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.CloseFiles.ToString(), searchItem, null, integrationEvent1, SyncEvents.CloseFiles,SyncType.LoadFromPLM);
                    if (searchItem.IsCommonPart == true && File.Exists(Path.Combine(searchItem.LibraryPath, searchItem.FileName))) { fileMessage.Status = "Finish"; continue; }
                    if (_asInnovator.DownloadFile(searchItem, IntegrationEvents, SyncEvents.OnLoadFromPLMDownloadBefore, item, searchItem.FileId, directory, searchItem.FileName)==true)
                    {
                        //searchItem.FilePath = directory;
                        fileMessage.Status = "Finish";
                    }
                }
                itemMessage.Detail = "";
                IntegrationEvents integrationEvent = null;

                SearchItem activeSearchItem = GetActiveSearchItem(searchItems);
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.OpenFiles.ToString(), ref activeSearchItem, null, integrationEvent, SyncEvents.OpenFiles, SyncType.LoadFromPLM);

                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.MoveFiles.ToString(), ref searchItems, null, integrationEvent, SyncEvents.OpenFiles, SyncType.LoadFromPLM);

                //Events
                itemMessage.Detail = "After Events";
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemLoadFromPLM.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnLoadFromPLMDownloadAfter, SyncType.Download);
                itemMessage.Status = "End";


                return searchItems;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 檢查副檔名是否正確
        /// </summary>
        /// <param name="cadItemId"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        //virtual protected internal bool CheckAddCADFile(string itemId, string full)
        //{
        //    try
        //    {

        //        Aras.IOM.Item item = AsInnovator.getItemById("CAD", itemId);
        //        //檢查選取圖檔
        //        if (item.getProperty(NativeProperty, "") == "")
        //        {
        //            MessageBox.Show(GetLanguageByKeyName("msg_CADFileDoesNotExistPLM"));
        //            return false;
        //        }

        //        //檢查副檔名(擴展名)是否一致
        //        Aras.IOM.Item file = AsInnovator.getItemById("File", item.getProperty(NativeProperty, ""));
        //        if (Path.GetExtension(file.getProperty("filename", "")).ToLower()!= Path.GetExtension(full).ToLower())
        //        {
        //            MessageBox.Show(GetLanguageByKeyName("msg_added_extension_inconsistent_with_system_file"));
        //            return false;
        //        }

        //        //檢查新增圖檔是否存在系統
        //        string fileName = Path.GetFileName(full);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw ex;
        //        string message = ex.Message;
        //        return false;
        //    }
        //}

      

        /// <summary>
        /// 從範本新增CAD物件
        /// </summary>
        /// <param name="obsSearchItems"></param>
        virtual protected internal bool AddCADFromTemplates(ObservableCollection<SearchItem> obsSearchItems)
        {
            try
            {
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("AddCADFromTemplates", "", "", "Start");

                List<SearchItem> searchItems = new List<SearchItem>();
                foreach (SearchItem searchItem in obsSearchItems)
                {
                    string fileId = searchItem.FileId;
                    string fileName = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename").Select(x => x.DataValue).FirstOrDefault();

                    if (Path.GetExtension(fileName) == "")
                    {
                        fileName += Path.GetExtension(searchItem.FileName);
                        PLMProperty plmProperty = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename").FirstOrDefault();
                        plmProperty.DataValue = fileName;
                    }

                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(fileName, "Download", "", "Start");

                    if (_asInnovator.DownloadFile(fileId, searchItem.FilePath,ref fileName) == true)
                    {
                        Dictionary<string, List<string>> search = new Dictionary<string, List<string>>();
                        List<string> files = new List<string>();
                        files.Add(fileName);

                        if (searchItem.PropertyFile.Count() < 1)
                        {
                            if (searchItem.ClassName == "") searchItem.ClassName = GetClassName(Path.GetExtension(searchItem.FileName));
                            List<PLMPropertyFile> propertyFile = _product.PdClassItems?.Where(c => c.Name == searchItem.ClassName)?.First()?.CsPropertyFile;
                            PLMPropertyFile plmPropertyFile = propertyFile.Where(x => x.Name == NativeProperty).FirstOrDefault();
                            PLMPropertyFile newPropertyFile = plmPropertyFile.Clone() as PLMPropertyFile;
                            searchItem.PropertyFile.Add(newPropertyFile);
                        }

                        searchItem.FileName = fileName;



                        //更新檔案(CAD產出檔案)
                        foreach (PLMPropertyFile propertyFile in searchItem.PropertyFile.Where(x => x.Name == NativeProperty))
                        {
                            propertyFile.FilePath = searchItem.FilePath;
                            propertyFile.FileName = fileName;
                        }

                        foreach (PLMProperty plmProperty in searchItem.PlmProperties.Where(x=>x.DataValue!=x.DisplayValue && x.DataValue==""))
                        {
                            plmProperty.DataValue = plmProperty.DisplayValue;
                            plmProperty.Value = plmProperty.DisplayValue;//OriginalValue
                            if (plmProperty.DataType == "list")
                            {

                            }
                            
                            //if (plmProperty.DataSource != "" && plmProperty.ListItem.Count > 0 && (plmProperty.DataType == "list" || plmProperty.DataType == "filter list"))
                            if (String.IsNullOrWhiteSpace(plmProperty.DataSource) ==false && plmProperty.ListItems.Count > 0 && (plmProperty.DataType == "list" || plmProperty.DataType == "filter list"))
                            {
                                plmProperty.DataValue = (plmProperty.ListItems.Where(x => x.Label == plmProperty.DisplayValue) != null) ? plmProperty.ListItems.Where(x => x.Label == plmProperty.DisplayValue).Select(x => x.Value).FirstOrDefault() : plmProperty.DisplayValue;
                                plmProperty.Value = plmProperty.DataValue;//OriginalValue
                            }
                        }

                        //Events
                        _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemAddFromTemplate.ToString(),  searchItem, IntegrationEvents, SyncEvents.OnAddFromTemplateBefore, SyncType.NewFormTemplateFile);


                        fileMessage.Detail = "Add New Item";
                        //新增CAD物件
                        SynToPLMNewItem(searchItem, IntegrationEvents, SyncEvents.OnAddTemplate, fileName, 0);

                        fileMessage.Detail = "Syn To PLM Item Files";
                        //CAD檔案同步
                        SynToPLMItemFiles(searchItem, 0);

                        fileMessage.Detail = "After Events";
                        //Events
                        _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemAddFromTemplate.ToString(),  searchItem, IntegrationEvents, SyncEvents.OnAddFromTemplateAfter, SyncType.NewFormTemplateFile);

                        fileMessage.Status  = "Finish";
                    }

                    searchItems.Add(searchItem);

                    itemMessage.Status  = "End";
                }

                IntegrationEvents integrationEvent = null;
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.OpenFiles.ToString(), ref searchItems,null, integrationEvent, SyncEvents.OpenFiles, SyncType.NewFormTemplateFile);
                return true;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("AddCADFromTemplates", ex.Message, ex.StackTrace, "Error", new ItemException(2, ex.Message, ex.StackTrace, ex));
                return false;
                //throw ex;
            }
        }
        
       
        /// <summary>
        /// 新增CAD物件
        /// </summary>
        /// <param name="className"></param>
        /// <param name="templateName"></param>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        virtual protected internal SearchItem AddCADFromTemplate(string className,string templateName, string directory, string fileName, List<PLMProperty> properties)
        {
            try
            {

                Dictionary<string, List<ClassTemplateFile>> _classesTemplates = GetClassesTemplates();
                if (_classesTemplates.Count < 1) return null;

                //List<ClassTemplateFile> list = _classesTemplates.Where(x => x.Key == className ).Select(x => x.Value).ElementAt(0);
                //string fileId = list.First().fileId;
                
                //ClassTemplateFile templateFile = _classesTemplates.Where(x => x.Key == className).Select(x => x.Value).Single().Where(x => x.Name == templateName && x.FileId != "").ElementAt(0);
                ClassTemplateFile templateFile = _classesTemplates.Where(x => x.Key == className).Select(x => x.Value).Single().Where(x => x.Name == templateName && String.IsNullOrWhiteSpace(x.FileId) ==false).FirstOrDefault();
                string fileId = templateFile.FileId;

                //下載
                if (_asInnovator.DownloadFile(fileId, directory,ref fileName) == false) return null;

                Dictionary<string, List<string>> search = new Dictionary<string, List<string>>();
                List<string> files = new List<string>();
                files.Add(fileName);
                search.Add(className, files);



                List<SearchItem> searchItems = GetPLMSearchItems(search);
                GetCADProperties(searchItems,false);

                //SearchItem searchItem = searchItems.ElementAt(0);
                SearchItem searchItem = searchItems.FirstOrDefault();
                string itemConfigId = searchItem.ItemConfigId;

                

                //判斷 itemConfigId :configId 機制未寫

                //更新屬性值(新增所填屬性)
                foreach (PLMProperty property in properties)
                {
                    List<PLMProperty> plmProperties = searchItem.PlmProperties.Where(x => x.Name == property.Name).ToList();
                    //if (plmProperties.Count > 0) plmProperties.ElementAt(0).DataValue = property.DataValue;
                    if (plmProperties.Count > 0) plmProperties.FirstOrDefault().DataValue = property.DataValue;
                }

                searchItem.FilePath = directory;
                searchItem.FileName = fileName;//@@@@

                //更新檔案(CAD產出檔案)
                foreach (PLMPropertyFile propertyFile in searchItem.PropertyFile)
                {

                    if (propertyFile.Name== NativeProperty)
                    {
                        propertyFile.FilePath = directory;
                        propertyFile.FileName = fileName;
                    }
                    /*
                    else if (propertyFile.functionName == ThumbnailProperty)
                    {
                        //產生縮圖
                        string thumbnail = fileName.Split(new char[] { '.' })[0] + ".jpg";
                        propertyFile.fileName = thumbnail;
                    }
                    */
                }

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemAddFromTemplate.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnAddFromTemplateBefore, SyncType.Add);

                //新增CAD物件
                SynToPLMNewItem(searchItem, IntegrationEvents,SyncEvents.OnAddFromTemplateBefore, fileName, 0);

                //CAD檔案同步
                SynToPLMItemFiles(searchItem, 0);

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemAddFromTemplate.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnAddFromTemplateAfter, SyncType.Add);

                return searchItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得Item Type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        virtual protected internal ItemType GetItemType(string name, SearchType type)
        {
            try
            {

                if (type == SearchType.CADRevisionSearch && _cadItemType != null) return _cadItemType;
                if (type == SearchType.CADAllRevisionsSearch && _cadRevisionItemType != null ) return _cadRevisionItemType;
                
                ItemType itemType = _itemTypes.Where(x=>x.Key == name).Select(x=>x.Value).FirstOrDefault();
                //itemType = _itemTypes.FirstOrDefault(x => x.Key == name)?.Value ?? string.Empty;
                if (itemType != null && type == SearchType.Search) return itemType;

                XElement itemTypeProperties = _asInnovator.GetSearchItemTypeProperties(name);

                itemType = new ItemType();
                //itemType = new ItemType(itemTypeProperties, name);
                itemType.Type  = type;
                itemType.BuildItemType(itemTypeProperties, itemType, name);

                //foreach (PLMProperties plmProperties in itemType.CsProperties.Where(x => x.DataSource != "" && (x.DataType == "list" || x.DataType == "filter list")))
                foreach (PLMProperty plmProperty in itemType.PlmProperties.Where(x => String.IsNullOrWhiteSpace(x.DataSource) ==false && (x.DataType == "list" || x.DataType == "filter list")))
                {
                    AddPropertyListItem(plmProperty);
                    plmProperty.IsInitial = false;
                    plmProperty.SoruceItemType = itemType;
                }

                itemType.Name = name;


                if (type == SearchType.CADRevisionSearch)
                    _cadItemType = itemType;
                else if(type == SearchType.CADAllRevisionsSearch)
                    _cadRevisionItemType = itemType;
                else
                    _itemTypes.Add(name, itemType);

                return itemType;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 同步CAD屬性
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="type"></param>
        virtual protected internal bool SyncCADsProperties(ref List<SearchItem> searchItems, SyncType type)
        {
            try
            {
                //IntegrationEvents, SyncEvents.OnSyncFormStructureBefore
                //Update : PLM 
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("SyncCADsProperties", "", "", "Start");

                foreach (SearchItem searchItem in searchItems.Where(x=>x.IsAdded==false && x.IsViewSelected==true))
                {
                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Synchronization properties", "", "Start");
                    IEnumerable<PLMProperty> plmProperties = null;
                    if (type== SyncType.SyncToPLM) {
                        plmProperties = searchItem.PlmProperties.Where(x => x.IsSyncPLM == true && (x.IsModify == true || x.SyncValue != x.DataValue));
                        bool ret= (plmProperties.Count() > 0)? _asInnovator.SynToPLMItemProperties(searchItem, plmProperties, IntegrationEvents, SyncEvents.OnSyncToPLMBefore) : _asInnovator.SynToPLMItemUpdateLibraryPath(searchItem);
                    } else {
                        plmProperties =searchItem.PlmProperties.Where(x => x.IsSyncPLM == true && x.IsModify == true );
                        bool ret = (plmProperties.Count() > 0)? _asInnovator.SynToPLMItemProperties(searchItem, plmProperties, IntegrationEvents, SyncEvents.OnSyncFormPLMBefore) : _asInnovator.SynToPLMItemUpdateLibraryPath(searchItem);
                    }
                    fileMessage.Status = "Finish";
                }


                //Add : PLM Update
                foreach (SearchItem searchItem in searchItems.Where(x => x.IsAdded == true && x.IsViewSelected == true))
                {
                    AddCADItem(searchItem, type);
                    
                    //if (searchItem.ItemConfigId != "" && searchItem.ItemId != "") searchItem.IsAdded = false;
                    if (String.IsNullOrWhiteSpace(searchItem.ItemConfigId) ==false && String.IsNullOrWhiteSpace(searchItem.ItemId) ==false) searchItem.IsAdded = false;
                }

                itemMessage.Status = "End";
                return true;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("SyncCADsProperties", ex.Message, ex.StackTrace, "Error", new ItemException(2, ex.Message, ex.StackTrace, ex));
                return false;
                //throw ex;
            }
        }

        /// <summary>
        /// CADs圖檔的檔案及結構同步
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="type"></param>
        virtual protected internal void SyncCADsFilesStructure(ref List<SearchItem> searchItems,List<StructuralChange> structureChanges, SyncType type)
        {
            try
            {

                if (type == SyncType.SyncToPLM)
                {
                    SyncToPLMCADsFilesStructure(ref searchItems, type);

                    ////以下是防呆
                    //IntegrationEvents integrationEvent = null;
                    //List<SearchItem> coseSearchItems = searchItems.Where(x => x.IsActiveCAD == false).ToList() as List<SearchItem>;
                    //_syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.CloseFiles.ToString(), ref coseSearchItems, null, integrationEvent, SyncEvents.CloseFiles, type);
                }
                else
                {
                    if (structureChanges==null) CloseCADActiveFile(searchItems, type);
                    SyncFromPLMCADsFilesStructure(ref searchItems, structureChanges, type);
                    if (structureChanges == null) OpenCADActiveFile(searchItems, type);

                    //Events: CAD Update
                    _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.UpdateCADsProperties.ToString(), ref searchItems, structureChanges, null, SyncEvents.OnUpdateCADsProperties, type);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 鎖定或解除鎖定
        /// </summary>
        /// <param name="searchItems"></param>
        virtual protected internal void LockOrUnlock(List<SearchItem> searchItems)
        {
            try
            {
                _asInnovator.LockOrUnlock(searchItems);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="isFilter"></param>
        /// <returns></returns>
        virtual protected internal Lists GetList(string listName, bool isFilter)
        {
            try
            {

                Lists listItem = _asInnovator.GetList(listName, isFilter);
                return listItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得範本
        /// </summary>
        /// <returns></returns>
        virtual protected internal Dictionary<string, List<ClassTemplateFile>> GetClassesTemplates()
        {
            try
            {

                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("GetClassesTemplates", "", "", "Start");

                if (_classesTemplates != null) return _classesTemplates;
                if (_product == null) return null;

                _classesTemplates = new Dictionary<string, List<ClassTemplateFile>>();
                foreach (ClassItem classItem in _product.PdClassItems.Where(x => x.CsTemplateFile.Count > 0)) _classesTemplates.Add(classItem.Name, classItem.CsTemplateFile);

                itemMessage.Status  = "End";
                return _classesTemplates;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("GetClassesTemplates", ex.Message, ex.StackTrace, "Error", new ItemException(2, ex.Message,ex.StackTrace,ex));
                return null;
                //throw ex;
            }
        }

        /// <summary>
        /// 取得類別(例如:範本)
        /// </summary>
        /// <returns></returns>
        virtual protected internal List<ClassItem> GetClassItems()
        {
            try
            {
               
                return _product.PdClassItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得PlugIn Functions
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        virtual protected internal ObservableCollection<ClassPlugin> GetClassPlugins(string className)
        {
            try
            {

                ObservableCollection<ClassPlugin> classPlugins = new ObservableCollection<Classes.ClassPlugin>();

                ClassItem classItem = _product.PdClassItems.FirstOrDefault(x => x.Name == className);
                if (classItem == null) return classPlugins;
                foreach (ClassPlugin plugin in classItem.CsPlugin) classPlugins.Add(plugin);

                return classPlugins;

            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("GetClassPlugins", ex.Message, ex.StackTrace, "Error",new ItemException(2, ex.Message, ex.StackTrace, ex));
                return null;
            }
        }

        

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        virtual protected internal bool DownloadFile(string fileId, String filePath, String fileName)
        {
            try
            {

                return _asInnovator.DownloadFile(fileId, filePath,ref fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 屬性同步到PLM
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="isLock"></param>
        /// <returns></returns>
        virtual protected internal bool SynToPLMNewItem(SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent, string filename, IsLock isLock)
        {
            try
            {
                List<PLMKeys> classItemKeys = GetClassItemKeys(searchItem.ClassName);

                return _asInnovator.SynToPLMNewItem(searchItem, integrationEvents, syncEvent, classItemKeys, filename, isLock);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檔案同步到PLM
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="isLock"></param>
        /// <returns></returns>
        virtual protected internal bool SynToPLMItemFiles(SearchItem searchItem, IsLock isLock)
        {
            try
            {
                Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);

                if (isLock == 0 && item.getLockStatus() == 1) item.unlockItem();
                if (isLock == IsLock.False && item.getLockStatus() == 1) item.unlockItem();

                bool ret = _asInnovator.SynToPLMItemFiles(ref item, searchItem);

                if (isLock == IsLock.True && item.getLockStatus() == 0) item.lockItem();
                searchItem.ItemId = item.getID();
                searchItem.ItemConfigId = item.getProperty("config_id", "");
                searchItem.KeyedName = item.getProperty("keyed_name", ""); 
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 同步到PLM :更新PLM圖檔結構
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="isLock"></param>
        /// <returns></returns>
        virtual protected internal bool SynToPLMItemStructure(List<SearchItem> searchItems, IsLock isLock, IsNewGeneration isNewGeneration)
        {
            try
            {
                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnSyncToPLMStructureBefore, SyncType.Structure);

                foreach (SearchItem searchItem in searchItems)
                {

                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Synchronization structure", "", "Start");
                    if (_asInnovator.SynToPLMItemStructure(searchItem, IntegrationEvents, isLock, isNewGeneration)==false ) return false ;
                    fileMessage.Status = "Finish";
                }
                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnSyncToPLMStructureAfter, SyncType.Structure);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步的插入分件或替換分件結構異動以及複製轉新增 (只更新searchItems)
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="structuralChange"></param>
        /// <returns></returns>
        virtual protected internal void SynStructuralChangeItems(List<SearchItem> searchItems, StructuralChange structuralChange)
        {
            try
            {
                //structuralChange.SourceItemId;
                SearchItem targetSearchItem =null;
                SearchItem sourceSearchItem = searchItems.Where(x => x.FileName == structuralChange.SourceFileName).FirstOrDefault();
                if (structuralChange.Type == ChangeType.CopyToAdd)
                {
                    ClsSynchronizer.VmSyncCADs.AddNewFileNameProperty(sourceSearchItem, sourceSearchItem.PlmProperties, "Copy To Add", structuralChange.TargetFileName);
                }else
                {
                    structuralChange.SourceItemConfigId = sourceSearchItem.ItemConfigId;
                    structuralChange.SourceItemId = sourceSearchItem.ItemId;
                    structuralChange.SourceFilePath = sourceSearchItem.FilePath;
                    SearchItem searchItem;
                    if (structuralChange.TargetItemId != "")
                    {
                        Aras.IOM.Item item = _asInnovator.AsInnovator.getItemById(ItemTypeName.CAD.ToString(), structuralChange.TargetItemId);
                       searchItem = GetPLMSearchItem(item).FirstOrDefault();

                        if (structuralChange.Type != ChangeType.InsertSaveAs) structuralChange.TargetFileName = searchItem.FileName;
                        if (structuralChange.IsCommonPart == false) structuralChange.TargetFilePath = searchItem.FilePath;
                        structuralChange.TargetItemConfigId = searchItem.ItemConfigId;

                    }else
                    {
                        //Parts Library 未放入系統
                        searchItem = GetCommonPartSearchItem(structuralChange.TargetFilePath, structuralChange.TargetFileName);
                    }

                    if (searchItem.FileName == structuralChange.SourceFileName) throw new Exception(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_TheSelectedDrawingFileIsTheSameAsTheParentFrawingFile"));//return false
                    structuralChange.IsExist = true;
                    targetSearchItem = searchItems.Where(x => x.FileName == searchItem.FileName).FirstOrDefault();
                    if (targetSearchItem == null)
                    {
                        structuralChange.IsExist = false;
                        searchItems.Add(searchItem);
                    }
                    targetSearchItem = searchItems.Where(x => x.FileName == searchItem.FileName).FirstOrDefault();

                    if (structuralChange.Type == ChangeType.InsertSaveAs)  ClsSynchronizer.VmSyncCADs.AddNewFileNameProperty(targetSearchItem, targetSearchItem.PlmProperties, "Insert Save As", structuralChange.TargetFileName);
                }
                
                if (structuralChange.Type== ChangeType.Insert|| structuralChange.Type == ChangeType.InsertSaveAs)
                {
                    //來源為父階,增加子階
                    CADStructure newCADStructure = new CADStructure();
                    newCADStructure.Order = (sourceSearchItem.CadStructure.Count > 0) ? (sourceSearchItem.CadStructure.Last().Order + 10) : 10;
                    newCADStructure.ItemConfigId = targetSearchItem.ItemConfigId;
                    newCADStructure.Child = targetSearchItem;
                    newCADStructure.OperationType = (structuralChange.Type == ChangeType.Insert)?1:8;
                    sourceSearchItem.CadStructure.Add(newCADStructure);
                }
                else
                {
                    //複製轉新增 / 來源為子階,並目的替換子階
                    if (structuralChange.Type == ChangeType.CopyToAdd)
                        SynStructuralCopyToAdd(searchItems, structuralChange);
                    else
                        SynStructuralReplace(searchItems, structuralChange, sourceSearchItem, targetSearchItem);
                }

                return ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// 同步到PLM :更新PLM圖檔結構
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="isLock"></param>
        /// <returns></returns>
        virtual protected internal bool SynToPLMItemStructure(SearchItem searchItem, IsLock isLock, IsNewGeneration isNewGeneration)
        {
            try
            {

                return _asInnovator.SynToPLMItemStructure(searchItem, IntegrationEvents, isLock, isNewGeneration);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得連線DB
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        virtual protected internal Lists GetDBList(string url)
        {
            try
            {

                //DBList
                Lists listItem = _asInnovator.GetDatabases(url);
                return listItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 目得目前同步的主要根的searchItem
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        virtual protected internal SearchItem GetActiveSearchItem(List<SearchItem> searchItems)
        {
            try
            {
                
                //SearchItem activeItem = searchItems.FirstOrDefault(y => y.FileName != "" && y.IsActiveCAD == true);
                SearchItem activeItem = searchItems.FirstOrDefault(y => String.IsNullOrWhiteSpace(y.FileName) ==false && y.IsActiveCAD == true);
                if (activeItem == null) activeItem = searchItems.FirstOrDefault();
                return activeItem;

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 取得物件的版本狀態
        /// </summary>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        virtual protected internal SyncVersionStatus GetVersionStatus(SearchItem searchItem)
        {
            try
            {
                SyncVersionStatus syncVersionStatus = SyncVersionStatus.Uncompared;
                
                //if (searchItem.CadItemId == "" && searchItem.CadItemConfigId == "") return syncVersionStatus;
                if (String.IsNullOrWhiteSpace(searchItem.CadItemId) && String.IsNullOrWhiteSpace(searchItem.CadItemConfigId)) return syncVersionStatus;
                if (searchItem.IsCurrent == true) syncVersionStatus = SyncVersionStatus.LatestVersion;
                
                //if (searchItem.CadItemId != searchItem.ItemId && searchItem.CadItemId != "") syncVersionStatus = SyncVersionStatus.NonLatestVersion;
                if (searchItem.CadItemId != searchItem.ItemId && String.IsNullOrWhiteSpace(searchItem.CadItemId) == false) syncVersionStatus = SyncVersionStatus.NonLatestVersion;
                
                //if (searchItem.CadItemConfigId != searchItem.ItemConfigId && searchItem.CadItemId != "") syncVersionStatus = SyncVersionStatus.NonSystem;
                if (searchItem.CadItemConfigId != searchItem.ItemConfigId && String.IsNullOrWhiteSpace(searchItem.CadItemId) ==false) syncVersionStatus = SyncVersionStatus.NonSystem;
                return syncVersionStatus;
            }
            catch (Exception ex)
            {
                //throw ex;
                string message = ex.Message;
                return SyncVersionStatus.Uncompared;
            }
        }


        /// <summary>
        /// 檢驗同步規則
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="type"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        virtual protected internal bool SyncCheckRules(IEnumerable<SearchItem> searchItems, SyncType type, SyncOperation operation)
        {
            try
            {

                switch (operation)
                {
                    case SyncOperation.EditorProperties:
                        if (type != SyncType.SyncToPLM) return true;
                        //檢查(1)是否被其他人鎖定,(2)狀態及是否是共用圖檔及其他規則
                        bool ret = IsCheckRules(searchItems, true);
                        if (IsCheckRequiredProperties(new ObservableCollection<SearchItem>(searchItems)) == false) ret = false;
                        return ret;
                        //break;
                    case SyncOperation.CADStructure:
                        if (type != SyncType.SyncToPLM) return true;
                        //檢查(1)是否被其他人鎖定,(2)狀態及是否是共用圖檔及其他規則
                        return IsCheckRules(searchItems, true);
                        //break;
                    case SyncOperation.AddTemplates:
                        //檢查必填屬性
                        return IsCheckRequiredProperties(searchItems);
                        //break;
                    //case SyncOperation.LoadListItems:
                    //    break;
                    case SyncOperation.QueryListItems:
                        if (type != SyncType.SyncToPLM) return true;
                        if (type == SyncType.SyncToPLM)
                            return ret = IsCheckRules(searchItems, true);
                        else
                            if (type != SyncType.LockOrUnlock) return true;

                        //檢查(1)是否被其他人鎖定,(2)狀態及是否是共用圖檔
                        return IsCheckRules(searchItems, false);
                        //break;
                }
                return true ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 加入CAD圖檔名稱
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="plmProperties"></param>
        virtual protected internal void AddNewFileNameProperty(SearchItem searchItem, ObservableCollection<PLMProperty> plmProperties, string label,string newFileName)
        {
            try
            {
                //Events


                if (plmProperties.Where(x => x.Name == "bcs_added_filename").Count() < 1)
                {
                    PLMProperty plmProperty = new PLMProperty();
                    plmProperty.SoruceSearchItem = searchItem;
                    plmProperty.Name = "bcs_added_filename";
                    plmProperty.Order = 0;
                    plmProperty.Label = label;// "Add New File Name";
                    plmProperty.IsSyncPLM = true;
                    plmProperty.IsRequired = true;

                    //plmProperty.SyncValue = (searchItem.ItemId != "" && searchItem.FileName != "") ? searchItem.FileName.Substring(0, (searchItem.FileName.Length - Path.GetExtension(searchItem.FileName).Length )) + "(1)" : "";
                    //plmProperty.SyncValue = (String.IsNullOrWhiteSpace(searchItem.ItemId) == false && String.IsNullOrWhiteSpace(searchItem.FileName) == false) ? (newFileName!="")? newFileName:searchItem.FileName.Substring(0, (searchItem.FileName.Length - Path.GetExtension(searchItem.FileName).Length)) + "(1)" : "";
                    plmProperty.SyncValue = (String.IsNullOrWhiteSpace(searchItem.ItemId) == false && String.IsNullOrWhiteSpace(searchItem.FileName) == false) ? (newFileName != "") ? Path.GetFileNameWithoutExtension(newFileName) : searchItem.FileName.Substring(0, (searchItem.FileName.Length - Path.GetExtension(searchItem.FileName).Length)) + "(1)" : "";
                    plmProperty.DisplayValue = plmProperty.SyncValue;
                    plmProperty.ResetSyncColorTypeValue();
                    plmProperty.IsInitial = false;
                    plmProperties.Add(plmProperty);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新輸入查詢條件值
        /// </summary>
        /// <param name="gridSelectedItems"></param>
        /// <param name="searchItemType"></param>
        virtual protected internal void UpdateSearchItemType(DataGrid gridSelectedItems,SearchItem searchItemType)
        {


            foreach (DataGridColumn col in gridSelectedItems.Columns)
            {

                DataTemplate tempData = col.HeaderTemplate as DataTemplate;
                StackPanel sp = tempData.LoadContent() as StackPanel;

                TextBox headerValue = sp.Children[1] as TextBox;
                if (headerValue == null) continue;

                PLMProperty dataContext = headerValue.DataContext as PLMProperty;

                PLMProperty property = searchItemType.PlmProperties.Where(x => x.PropertyName == dataContext.PropertyName).FirstOrDefault();

                //property.DisplayValue = (dataContext.DisplayValue==null)? "": dataContext.DisplayValue;
                property.DisplayValue = (dataContext.DisplayValue==null)? "": dataContext.DisplayValue;

                if (property.DataType!= "classification") property.DataValue = property.DisplayValue;

                switch (property.DataType)
                {
                    case "item":
                        if (String.IsNullOrWhiteSpace( property.DataValue) ==false)
                        {
                            Aras.IOM.Item item = AsInnovator.getItemByKeyedName(property.DataSource, property.DataValue);
                            property.DataValue = (item != null) ? item.getID() : property.DataValue;
                        }                
                        break;
                    case "filter list":
                    case "list":
                        property.DataValue = (property.PLMList.ListItems.Where(x => x.Label == property.DataValue) != null) ? property.PLMList.ListItems.Where(x => x.Label == property.DataValue).Select(x => x.Value ).FirstOrDefault() : property.DisplayValue;
                        break;
                    case "date":
                        SetDatePLMPropertyDataValue(property);
                        break;
                }
                property.Value = property.DataValue;

            }
        }

        /// <summary>
        /// 清空查詢欄位條件值
        /// </summary>
        /// <param name="gridSelectedItems"></param>
        /// <param name="searchItemType"></param>
        virtual protected internal void ClearSearchConditions(DataGrid gridSelectedItems, SearchItem searchItemType)
        {

            foreach (PLMProperty property in searchItemType.PlmProperties)
            {
                property.DisplayValue = "";
                property.DataValue = "";
                property.Value = "";
            }


            foreach (DataGridColumn col in gridSelectedItems.Columns)
            {

                DataTemplate tempData = col.HeaderTemplate as DataTemplate;
                StackPanel sp = tempData.LoadContent() as StackPanel;
                TextBox textBox = sp.Children[1] as TextBox;
                if (textBox == null) continue;
                textBox.Text = "";
            }
        }

        virtual protected internal void UpdateLibraryPathFiles(LibraryPath libraryPath)
        {
            try
            {
                _asInnovator.UpdateLibraryPathFiles(libraryPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region "                   方法(內部)"

        /// <summary>
        /// 重新設定SyncValue日期值
        /// </summary>
        /// <param name="property"></param>
        private void SetDatePLMPropertyDataValue(PLMProperty property)
        {
            try
            {

                if (String.IsNullOrWhiteSpace(property.DataValue) == false)
                {
                    DateTime date = DateTime.Parse(property.DataValue);
                    if (date.Year > 1911)
                    {
                        property.DataValue = date.ToString("yyyy-MM-ddTHH:mm:ss");
                        return;
                    }
                }
                property.DataValue = null;
            }

            catch (Exception ex)
            {
                string strError = ex.Message;
                property.DataValue = null;
            }
        }


        /// <summary>
        /// 來源為子階,並目的替換子階
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="structuralChange"></param>
        /// <param name="sourceSearchItem"></param>
        /// <param name="targetSearchItem"></param>
        private void SynStructuralReplace(List<SearchItem> searchItems, StructuralChange structuralChange, SearchItem sourceSearchItem, SearchItem targetSearchItem)
        {
            try
            {

                //來源為子階,並目的替換子階
                foreach (SearchItem searchItem in searchItems)
                {
                    //沒有屬性編輯,是沒有ItemConfigId
                    foreach (CADStructure cadStructure in searchItem.CadStructure.Where(x => x.Child.FileName == sourceSearchItem.FileName))
                    {
                        if (structuralChange.IsReplaceAll == true || (structuralChange.IsReplaceAll == false && cadStructure.Order == structuralChange.Order))
                        {
                            cadStructure.ItemConfigId = targetSearchItem.ItemConfigId;
                            cadStructure.Child = targetSearchItem;
                            cadStructure.OperationType = 2;
                            targetSearchItem.IsViewSelected = sourceSearchItem.IsViewSelected;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 複製轉新增
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="structuralChange"></param>
        private void SynStructuralCopyToAdd(List<SearchItem> searchItems, StructuralChange structuralChange)
        {
            try
            {

                foreach (SearchItem searchItem in searchItems)
                {
                    foreach (CADStructure cadStructure in searchItem.CadStructure.Where(x => x.Child.FileName == structuralChange.SourceFileName))
                    {
                        cadStructure.OperationType = 2;

                    }
                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 新增ClassKey及
        /// </summary>
        /// <param name="classKey"></param>
        /// <param name="strRules"></param>
        /// <param name="alias"></param>
        private string AddClassItemKeysConditions(string classKey, string alias, ref string strRules)
        {
            try
            {
                List<PLMKeys> classItemKeys = GetClassItemKeys(classKey);
                string strConditions = "";

                if (classItemKeys != null)
                {
                    classItemKeys?.ForEach(keyProperty => {
                        //if (strConditions != "") strConditions += " and ";
                        if (String.IsNullOrWhiteSpace(strConditions) == false) strConditions += " and ";
                        strConditions += alias + keyProperty.Name + "=N'" + keyProperty.Value + "'";
                    });
                    //if (strConditions != "") strConditions = " and " + strConditions;
                    if (String.IsNullOrWhiteSpace(strConditions) == false) strConditions = " and " + strConditions;

                }
                else
                {
                    GetRuleKeys(ref strRules, ref strConditions);
                }
                return strConditions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 設定屬性顯示值
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="itemProperty"></param>
        private void SetDispalyValue(SearchItem searchItem, PLMProperty itemProperty)
        {
            try
            {

                itemProperty.DisplayValue = itemProperty.DataValue;
                itemProperty.Value = itemProperty.DataValue;//OriginalValue
                if (searchItem.IsAdded == true) { itemProperty.SyncValue = itemProperty.DataValue; itemProperty.SyncDisplayValue = itemProperty.DisplayValue; }

                //if (itemProperty.DataSource != "" && itemProperty.ListItem.Count() > 0 && (itemProperty.DataType == "list" || itemProperty.DataType == "filter list"))
                if (String.IsNullOrWhiteSpace(itemProperty.DataSource)==false && itemProperty.ListItems.Count() > 0 && (itemProperty.DataType == "list" || itemProperty.DataType == "filter list"))
                {
                    itemProperty.DisplayValue = (itemProperty.ListItems.Where(x => x.Value == itemProperty.DataValue) != null) ? itemProperty.ListItems.Where(x => x.Value == itemProperty.DataValue).Select(x => x.Label).FirstOrDefault() : itemProperty.DisplayValue;
                }


            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }


        /// <summary>
        /// 關閉目標圖組圖檔
        /// </summary>
        /// <param name="searchItems"></param>
        private void CloseCADActiveFile(List<SearchItem> searchItems, SyncType type)
        {
            try
            {
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("CloseCADActiveFile", "", "", "Start");
                SearchItem activeSearchItem = GetActiveSearchItem(searchItems);
                IntegrationEvents integrationEvent = null;
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.CloseFiles.ToString(), ref activeSearchItem, null, integrationEvent, SyncEvents.CloseFiles, type);
                itemMessage.Status = "End";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 開啟目標圖組圖檔
        /// </summary>
        /// <param name="searchItems"></param>
        private void OpenCADActiveFile(List<SearchItem> searchItems, SyncType type)
        {
            try
            {
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("OpenCADActiveFile", "", "", "Start");
                SearchItem activeSearchItem = GetActiveSearchItem(searchItems);
                IntegrationEvents integrationEvent = null;
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.OpenFiles.ToString(), ref activeSearchItem, null, integrationEvent, SyncEvents.OpenFiles, type);
                itemMessage.Status = "End";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查勾選的執行項目,是否符合規則
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="isUserRules"></param>
        /// <returns></returns>
        private bool IsCheckRules(IEnumerable<SearchItem> searchItems, bool isUserRules)
        {
            try
            {
                ItemMessage itemMessage = null;
                foreach (SearchItem searchItem in searchItems.Where(x => x.IsAdded == false && x.IsViewSelected == true ))
                {
                    //SyncVersionStatus
                    if (IsCheckRules(searchItem, isUserRules)) continue;
                    if (itemMessage==null) itemMessage = ClsSynchronizer.VmMessages.AddItemMessage(GetLanguageByKeyName("msg_NotAllowedToExecute"), "", "", "Error");

                    string message = (searchItem.AccessRights != SyncAccessRights.FlaggedByOthers.ToString()) ? "" : GetLanguageByKeyName("flaggedByOthers");

                    if (message == "" && (searchItem.IsCommonPart || searchItem.IsStandardPart) && searchItem.AccessRights != SyncAccessRights.FlaggedByMe.ToString())
                        message = GetLanguageByKeyName("msg_CommonOrStandardDrawingFilesNeedToBeFlagged");

                    //if (message != "") message += ";";
                    if (String.IsNullOrWhiteSpace(message) ==false) message += ";";
                    message += (searchItem.RestrictedStatus == SyncRestrictedStatus.None.ToString()) ? "" : GetLanguageByKeyName("msg_modificationIsForbidden");
                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, message, "", "Start");

                }

                if (itemMessage == null) return true;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查是否符合規則
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="isUserRules"></param>
        /// <returns></returns>
        private bool IsCheckRules(SearchItem searchItem, bool isUserRules)
        {
            try
            {
                //SyncVersionStatus
                if (searchItem.AccessRights == SyncAccessRights.FlaggedByMe.ToString()) return true; //自己鎖定,是可以不判斷規則條件
                if (searchItem.AccessRights != SyncAccessRights.FlaggedByOthers.ToString() && searchItem.RestrictedStatus == SyncRestrictedStatus.None.ToString()) return true;
                if (searchItem.AccessRights != SyncAccessRights.FlaggedByOthers.ToString() && isUserRules == false && searchItem.RestrictedStatus == SyncRestrictedStatus.Prohibited.ToString()) return true ;

                return false ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 檢查勾選的執行項目,必填欄位是否有值
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        private bool IsCheckRequiredProperties(IEnumerable<SearchItem> searchItems)
        {
            try
            {
                if (searchItems==null) throw new Exception(GetLanguageByKeyName("msg_NotAllowedToExecute"));
                ItemMessage itemMessage = null;
                foreach (SearchItem searchItem in searchItems.Where(x => x.IsAdded == true ||  x.IsViewSelected == true))
                {

                    //SyncVersionStatus
                    
                    //List<string> properties = searchItem.PlmProperties.Where(x => x.IsRequired == true && x.IsSyncPLM == true &&  (x.DisplayValue == "" || x.DisplayValue == null)).Select(x => x.Label).ToList();
                    List<string> properties = searchItem.PlmProperties.Where(x => x.IsRequired == true && x.IsSyncPLM == true &&  String.IsNullOrWhiteSpace(x.DisplayValue)).Select(x => x.Label).ToList();
                    if (properties.Count == 0) continue;

                    if (itemMessage == null) itemMessage = ClsSynchronizer.VmMessages.AddItemMessage(GetLanguageByKeyName("msg_NotAllowedToExecute"), "", "", "Error");
                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, String.Join(",", properties), "", "Start");

                }
                if (itemMessage == null) return true;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        



        /// <summary>
        /// 設定物件屬性的Filter
        /// </summary>
        /// <param name="classItem"></param>
        /// <param name="searchItemsFilter"></param>
        private void SetSearchItemsFilter(ClassItem classItem, ref List<SearchItem> searchItemsFilter)
        {
            try
            {


                Dictionary<string, List<string>> search = new Dictionary<string, List<string>>();

                List<string> files = searchItemsFilter.Where(x=>x.PlmProperties.Count==0).Select(x => x.FileName).ToList();
                if (files.Count == 0) return;

                search.Add(classItem.Name, files);
                List<SearchItem> plmSearchItems = GetPLMSearchItems(search);
                
                //foreach (SearchItem plmSearchItem in plmSearchItems.Where(x => x.FileId != "" && x.ItemConfigId != ""))
                foreach (SearchItem plmSearchItem in plmSearchItems.Where(x => String.IsNullOrWhiteSpace(x.FileId) ==false  && String.IsNullOrWhiteSpace(x.ItemConfigId) ==false ))
                {
                    foreach (SearchItem searchItemFilter in searchItemsFilter.Where(x => x.FileName == plmSearchItem.FileName))
                    {
                        searchItemFilter.IsAdded = false;
                        searchItemFilter.IsExist = true;
                        searchItemFilter.FileId = plmSearchItem.FileId;
                        searchItemFilter.ItemConfigId = plmSearchItem.ItemConfigId;
                        searchItemFilter.KeyedName  = plmSearchItem.KeyedName;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得規則Keys
        /// </summary>
        /// <param name="strRules"></param>
        /// <returns></returns>
        private void GetRuleKeys(ref string strRules,ref string strConditions)
        {
            try
            {

                Dictionary<string, List<string>> allKeys = GetAllKeys();
                foreach (var property in allKeys)
                {
                    if (property.Value.Count < 1) continue;
                    strConditions += " and " + property.Key + " in (";
                    string strJoin = "";
                    foreach (var value in property.Value)
                    {
                        strConditions += strJoin + "N'" + value + "'";
                        strJoin = ",";
                    }
                    strConditions += ")";
                    if (strRules.IndexOf("b." + property.Key) < 0) strRules += ",b." + property.Key;
                }
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }


        /// <summary>
        /// GetPLMSearchItems:查詢結果,新增SearchItem
        /// </summary>
        /// <param name="qureyResult"></param>
        /// <param name="rules"></param>
        /// <param name="classItem"></param>
        /// <param name="searchItems"></param>
        private void AddSearchItemByQureyResult(Aras.IOM.Item qureyResult, List<ConditionalRule> rules, KeyValuePair<string, List<string>> classItem, ref List<SearchItem> searchItems)
        {
            try
            {


                for (var i = 0; i < qureyResult.getItemCount(); i++)
                {
                    SearchItem searchItem = new SearchItem();
                    searchItem.IsAdded = false;
                    searchItem.IsExist = true;
                    searchItem.FileName = qureyResult.getItemByIndex(i).getProperty("filename", "");
                    searchItem.FileId = qureyResult.getItemByIndex(i).getProperty(NativeProperty, "");
                    searchItem.ItemConfigId = qureyResult.getItemByIndex(i).getProperty("config_id", "");
                    searchItem.KeyedName  = qureyResult.getItemByIndex(i).getProperty("keyed_name", "");
                    searchItem.ItemId = qureyResult.getItemByIndex(i).getProperty("id", "");
                    
                    //searchItem.ClassName = (classItem.Key != "") ? classItem.Key : GetClassKey(qureyResult.getItemByIndex(i));
                    searchItem.ClassName = (String.IsNullOrWhiteSpace(classItem.Key) ==false) ? classItem.Key : GetClassKey(qureyResult.getItemByIndex(i));
                    searchItem.ItemType = ItemTypeName.CAD.ToString();//Modify by kenny 2020/08/13
                    searchItem.ClassThumbnail = GetClassThumbnail(searchItem.ClassName);
                    searchItem.RuleProperties = GetRuleProperties(rules, qureyResult.getItemByIndex(i));
                    
                    classItem.Value.Remove(searchItem.FileName);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得Conditional Rules
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetRuleProperties(List<ConditionalRule> rules, Aras.IOM.Item item)
        {
            try
            {
                Dictionary<string, string> ruleProperty = new Dictionary<string, string>();
                rules?.ForEach(rule =>
                {
                    if (ruleProperty.Where(x => x.Key == rule.PrepertyName).Select(x => x.Value).FirstOrDefault() == null) ruleProperty.Add(rule.PrepertyName, item.getProperty(rule.PrepertyName, ""));
                });
                return ruleProperty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得規則屬性值驗證限制狀態
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="rules"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private SyncRestrictedStatus GetRestrictedStatus(SearchItem searchItem,List<ConditionalRule> rules, SyncType type)
        {
            try
            {
                SyncRestrictedStatus syncRestrictedStatus= SyncRestrictedStatus.None;

               foreach (ConditionalRule conditionalRule in rules.Where(x=>x.Name== type.ToString())){
                    SyncRestrictedStatus restrictedStatus = ConditionalRulesRestrictedStatus(searchItem, conditionalRule);
                    if (restrictedStatus == SyncRestrictedStatus.NoModification) return restrictedStatus;
                    if (restrictedStatus == SyncRestrictedStatus.SharedNoModification) syncRestrictedStatus = restrictedStatus;
                    if (syncRestrictedStatus != SyncRestrictedStatus.SharedNoModification) syncRestrictedStatus = restrictedStatus;
                }

                //Modify by kenny 2020/08/26 ---
                if (syncRestrictedStatus == SyncRestrictedStatus.None && (searchItem.IsCommonPart || searchItem.IsStandardPart)) syncRestrictedStatus = SyncRestrictedStatus.SharedNoModification;
                //------------------------------
                return syncRestrictedStatus;

            }
            catch (Exception ex)
            {
                //throw ex;
                string message = ex.Message;
                return SyncRestrictedStatus.None;
            }
        }

        

        /// <summary>
        /// 依規則限制狀態
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="conditionalRule"></param>
        /// <returns></returns>
        private SyncRestrictedStatus ConditionalRulesRestrictedStatus(SearchItem searchItem,ConditionalRule conditionalRule)
        {
            try
            {
                SyncRestrictedStatus syncRestrictedStatus = SyncRestrictedStatus.None;
                foreach (string value in searchItem.RuleProperties.Where(x => x.Key == conditionalRule.PrepertyName).Select(x => x.Value))
                {
                    List<string> list = conditionalRule.Value.Split(',').ToList();

                    double retNum;
                    bool isRuleValueNum = Double.TryParse(conditionalRule.Value, out retNum);
                    bool isValueNum = Double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
                    bool checkValue = false;            

                    //檢查file name : 例如範本新增,不允許相同檔名 @@@@@@@@@@@@@@@
                    if (conditionalRule.Value == "CurrentFileName")continue;
                    //switch case
                    //switch (conditionalRule.Condition)
                    //{
                    //    case "in":
                    //        checkValue = (list.Where(x => x == value).ToList().Count > 0) ? true : false;
                    //        break;
                    //    case "not in":
                    //        checkValue = (list.Where(x => x == value).ToList().Count > 0) ? false : true;
                    //        break;

                    //    case "like":
                    //        checkValue = (list.Where(x => x.Contains(value)).ToList().Count > 0) ? true : false;
                    //        break;

                    //    case "eq":
                    //        checkValue = (conditionalRule.Value == value) ? true : false;
                    //        break;

                    //    case "ne":
                    //        checkValue = (conditionalRule.Value != value) ? true : false;
                    //        break;

                    //    case "gt":
                    //        if (isRuleValueNum == false || isValueNum == false) continue;
                    //        checkValue = (System.Convert.ToDecimal(conditionalRule.Value) > System.Convert.ToDecimal(value) ) ? true : false;
                    //        break;

                    //    case "ge":
                    //        if (isRuleValueNum == false || isValueNum == false) continue;
                    //        checkValue = (System.Convert.ToDecimal(conditionalRule.Value) >= System.Convert.ToDecimal(value)) ? true : false;
                    //        break;

                    //    case "lt":
                    //        if (isRuleValueNum == false || isValueNum == false) continue;
                    //        checkValue = (System.Convert.ToDecimal(conditionalRule.Value) < System.Convert.ToDecimal(value)) ? true : false;
                    //        break;

                    //    case "le":
                    //        if (isRuleValueNum == false || isValueNum == false) continue;
                    //        checkValue = (System.Convert.ToDecimal(conditionalRule.Value) <= System.Convert.ToDecimal(value)) ? true : false;
                    //        break;
                    //    default:
                    //        checkValue = true;
                    //        break;
                    //}

                    checkValue = ClsSynchronizer.OperatorCompare(conditionalRule.Condition, conditionalRule.Value, value);

                    if (checkValue==false)
                    {
                        if (conditionalRule.PrepertyName == "state") return SyncRestrictedStatus.NoModification;
                        //if (conditionalRule.PrepertyName == "bcs_is_standard_part") syncRestrictedStatus=SyncRestrictedStatus.SharedNoModification;
                        if (conditionalRule.PrepertyName == "is_standard" || conditionalRule.PrepertyName == "bcs_is_standard_part") syncRestrictedStatus = SyncRestrictedStatus.SharedNoModification;
                        if (syncRestrictedStatus != SyncRestrictedStatus.SharedNoModification) syncRestrictedStatus = SyncRestrictedStatus.Prohibited;
                    }

                }

                return syncRestrictedStatus;

                }
            catch (Exception ex)
            {
                //throw ex;
                string message = ex.Message;
                return SyncRestrictedStatus.None;
            }
        }



        /// <summary>
        /// 同步到PLM,結構來自CAD圖檔
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="type"></param>
        private void SyncToPLMCADsFilesStructure(ref List<SearchItem> searchItems, SyncType type)
        {
            try
            {

                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("SyncToPLMCADsFilesStructure", "", "", "Start");
                //IsLock isLock = IsLock.True;
                IsLock isLock = IsLock.None;

                IsNewGeneration isNewGeneration = IsNewGeneration.True ;

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMStructure.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnSyncToPLMStructureBefore, SyncType.Download);


                List<SearchItem> execSearchItems = searchItems.Where(x => x.IsAdded == false && x.IsViewSelected == true).ToList() as List<SearchItem>;
                for (int i = execSearchItems.Count-1; i>=0; i--)
                {
                    SearchItem searchItem = execSearchItems[i];
                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Synchronization structure", "", "Start");
                    _asInnovator.SynToPLMItemStructure(searchItem, IntegrationEvents, isLock, isNewGeneration);
                    fileMessage.Status = "Finish";
                }

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMStructure.ToString(), searchItems, null, IntegrationEvents, SyncEvents.OnSyncToPLMStructureAfter, SyncType.Download);

                itemMessage.Status = "End";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 同步CAD結構,來自PLM CAD圖檔
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="structureChanges"></param>
        /// <param name="type"></param>
        private void SyncFromPLMCADsFilesStructure(ref List<SearchItem> searchItems, List<StructuralChange> structureChanges, SyncType type)
        {
            try
            {

                //Events
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("SyncFromPLMCADsFilesStructure", "", "", "Start");
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncFromPLMStructure.ToString(), searchItems, structureChanges, IntegrationEvents, SyncEvents.OnSyncFormStructureBefore, SyncType.Download);

                //插入分件或替換分件紀錄在:structureChanges,當IsExist=false時,表示不存在目前結構圖檔,才要處理
                SearchItem activeItem = GetActiveSearchItem(searchItems);
                foreach (StructuralChange structuralChange in structureChanges.Where(x => x.IsExist == false))
                {
                    SearchItem searchItem = searchItems.Where(x => x.FileName == structuralChange.TargetFileName || x.ItemConfigId == structuralChange.TargetItemConfigId).FirstOrDefault();
                    if (searchItem != null)
                    {
                        searchItem.FilePath = activeItem.FilePath;

                        if (structuralChange.IsCommonPart==false ) structuralChange.TargetFilePath = activeItem.FilePath;

                        PLMProperty property = null;
                        if (structuralChange.Type ==ChangeType.InsertSaveAs)
                            property = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) == false).SingleOrDefault();

                        string fileName = (property != null) ? property.DisplayValue + Path.GetExtension(searchItem.FileName) : searchItem.FileName;
                        IntegrationEvents integrationEvent = null;
                        _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.CloseFiles.ToString(), searchItem, null, integrationEvent, SyncEvents.CloseFiles, type);

                        ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(fileName, "Download", "", "Start");

                        if (structuralChange.IsCommonPart==true)
                        {
                            searchItem.LibraryPath = structuralChange.TargetFilePath;
                            searchItem.FilePath = searchItem.LibraryPath;
                            searchItem.IsCommonPart = structuralChange.IsCommonPart;                         
                        }

                        if (structuralChange.Type == ChangeType.InsertSaveAs && structuralChange.LibraryFileName!="")
                        {
                            File.Copy(Path.Combine(structuralChange.LibraryPath, structuralChange.LibraryFileName), Path.Combine(activeItem.FilePath, structuralChange.TargetFileName));
                            structuralChange.TargetFilePath = activeItem.FilePath;
                            fileMessage.Status = "Finish"; continue;
                        }
                        else
                        {
                            if (searchItem.IsCommonPart == true && File.Exists(Path.Combine(searchItem.LibraryPath, searchItem.FileName))) { fileMessage.Status = "Finish"; continue; }
                        }                      
                        _asInnovator.DownloadFile(searchItem.FileId, activeItem.FilePath, ref fileName);
                        fileMessage.Status = "Finish";
                        searchItem.IsNewVersion = true;
                    }
                }

                foreach (StructuralChange structuralChange in structureChanges.Where(x => x.IsExist == true && x.Type== ChangeType.InsertSaveAs))
                {
                    SearchItem searchItem = searchItems.Where(x => x.ItemConfigId == structuralChange.TargetItemConfigId).FirstOrDefault();
                    if (searchItem != null)
                    {
                        PLMProperty property = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) == false).SingleOrDefault();
                        string fileName = property.DisplayValue + Path.GetExtension(searchItem.FileName);
                        if (structuralChange.IsCommonPart == false) structuralChange.TargetFilePath = activeItem.FilePath;
                        ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(fileName, "Download", "", "Start");
                        _asInnovator.DownloadFile(searchItem.FileId, activeItem.FilePath, ref fileName);
                        fileMessage.Status = "Finish";
                    }
                }


                //****************@@@@@@@@@@@@
                foreach (SearchItem searchItem in searchItems.Where(x => x.IsAdded == false && x.IsViewSelected == true && IsExistInStructureChanges(structureChanges, x.FileName) ==false))
                //foreach (SearchItem searchItem in searchItems.Where(x => x.ItemId!="" && x.IsAdded == false && x.IsViewSelected == true && IsExistInStructureChanges(structureChanges,x.FileName) ==false))
                {
                    if (IsInStructureChangesParent(structureChanges, searchItem)) continue;
                    //if (searchItem.ItemId == "")
                    if (String.IsNullOrWhiteSpace(searchItem.ItemId))
                    {
                        //表示不維護屬性,取得CAD Id @@@@@@@@@@@@@@@@
                    }

                    Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId );
                    //@@@@@@@@@@@@@@@@@@
                    //foreach (var aa in structureChanges.Where(y=>y.SourceFileName== searchItem.FileName && y.IsExist == false){

                    //}

                    IntegrationEvents integrationEvent = null;
                    _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.CloseFiles.ToString(),  searchItem, null, integrationEvent, SyncEvents.CloseFiles, type);

                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Download", "", "Start");

                    if (_asInnovator.DownloadFile(searchItem, IntegrationEvents, SyncEvents.OnSyncFormStructureBefore, item, searchItem.FileId, searchItem.FilePath, searchItem.FileName) == true )
                    {
                        searchItem.IsNewVersion = true;
                        fileMessage.Status  = "Finish";
                    }
                    else
                    {
                        //searchItem.filePath = directory;
                    }

                }
                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncFromPLMStructure.ToString(), searchItems, structureChanges, IntegrationEvents, SyncEvents.OnSyncFormStructureAfter, SyncType.Download);
                itemMessage.Status  = "End";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 是否為插入與替換分件異動
        /// </summary>
        /// <param name="structureChanges"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool IsExistInStructureChanges(List<StructuralChange> structureChanges,string fileName)
        {
            try
            {
                if (structureChanges == null) return false;
                StructuralChange structuralChange = structureChanges.Where(x => x.TargetFileName.ToLower() == fileName.ToLower() ||  x.SourceFileName.ToLower() == fileName.ToLower()).FirstOrDefault();
                if (structuralChange != null) return true;

                //if (structuralChange != null)
                //{              
                //    //if (structuralChange.SourceFileName == "" || structuralChange.TargetFileName == "") return true;
                //    if (String.IsNullOrWhiteSpace(structuralChange.SourceFileName) || String.IsNullOrWhiteSpace(structuralChange.TargetFileName)) return true;
                //}
                if (String.IsNullOrWhiteSpace(structuralChange.SourceFileName) || String.IsNullOrWhiteSpace(structuralChange.TargetFileName)) return true;


                //再檢查傳是否為替換分件勾選項之父親
                return false;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 是否為替換分件勾選項之父親
        /// </summary>
        /// <param name="structureChanges"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        private bool IsInStructureChangesParent(List<StructuralChange> structureChanges, SearchItem searchItem)
        {
            try
            {

                if (searchItem.CadStructure.Count == 0) return false;
                
                //foreach (StructuralChange structuralChange in structureChanges.Where(x=> x.SourceFileName != ""))
                foreach (StructuralChange structuralChange in structureChanges.Where(x=> String.IsNullOrWhiteSpace(x.SourceFileName) ==false))
                {
                    CADStructure structure = searchItem.CadStructure.Where(x => x.Child.FileName== structuralChange.SourceFileName || x.Child.FileName == structuralChange.TargetFileName).FirstOrDefault();
                    if (structure != null) return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 新增CAD的Item物件
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="type"></param>
        private void AddCADItem(SearchItem searchItem, SyncType type)
        {
            try
            {

                string itemConfigId = searchItem.ItemConfigId;

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemAddFromTemplate.ToString(), searchItem, IntegrationEvents, SyncEvents.OnAddItemBefore, type);

                //新增CAD物件
                SynToPLMNewItem(searchItem, IntegrationEvents, SyncEvents.OnAddFromTemplateAfter, searchItem.FileName, 0);

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemAddFromTemplate.ToString(), searchItem, IntegrationEvents, SyncEvents.OnAddItemAfter, type);

                //return searchItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        /// <summary>
        /// 取得CAD整合定義(依目前傳入CAD產品)
        /// </summary>
        private void GetConfigurations()
        {
            try
            {
                XDocument xmlConfig = _asInnovator.GetConfigurations(CADSoftware);
                if (_product == null)
                {
                    XElement xmlRequired = _asInnovator.GetCADIsRequiredProperties();
                    _product = new Product(xmlConfig, xmlRequired);
                    _product.Name = CADSoftware;
                    _product.BuildClasses();

                    IsResolveAllLightweightSuppres = _product.IsResolveAllLightweightSuppres;
                    _syncCADEvents.IsResolveAllLightweightSuppres = IsResolveAllLightweightSuppres;
                    _asInnovator.IsResolveAllLightweightSuppres = IsResolveAllLightweightSuppres;

                    IsResolveAllSuppres = _product.IsResolveAllSuppres;
                    _syncCADEvents.IsResolveAllSuppres = IsResolveAllLightweightSuppres;
                    _asInnovator.IsResolveAllSuppres = IsResolveAllSuppres;

                    string nativeProperty = "";
                    string thumbnailProperty = "";

                    List<PLMPropertyFile> propertyFile = _product.PdClassItems?.Where(c => c.Name !="")?.First()?.CsPropertyFile;
                    
                    //foreach (ClassItem classItem in _product.PdClassItem?.Where(c => c.Name != "")) {
                    foreach (ClassItem classItem in _product.PdClassItems?.Where(c => String.IsNullOrWhiteSpace(c.Name) ==false)) {
                        if (nativeProperty=="") nativeProperty = classItem.CsPropertyFile.Where(x => x.FunctionName == "native_property").Select(x => x.Name).First();
                        if (thumbnailProperty == "") thumbnailProperty = classItem.CsPropertyFile.Where(x => x.FunctionName == "thumbnail_property").Select(x => x.Name).First();
                    }
                    if (nativeProperty != "") NativeProperty = nativeProperty;
                    if (thumbnailProperty != "") ThumbnailProperty = thumbnailProperty;


                   foreach (ClassItem classItem in _product.PdClassItems)
                    {
                        //classItem.Thumbnail
                        
                        string imagePath = ClsSynchronizer.VmSyncCADs.GetImageFullName(classItem.Thumbnail);
                        
                        //if (imagePath == "")
                        if (String.IsNullOrWhiteSpace(imagePath))
                        {
                            string fileName = (_product.Name + "_" + classItem.Name + ".png").ToLower();
                            string path = @"pack://application:,,,/BCS.CADs.Synchronization;component/Images/";
                            imagePath = (GetUri(path + (_product.Name + "_" + classItem.Name + ".jpg").ToLower()) != null) ? path + fileName : path + classItem.Name + ".png";
                        }
                        classItem.ThumbnailFullName = imagePath;
                        foreach (ClassTemplateFile classTemplateFile in classItem.CsTemplateFile)
                        {
                            classTemplateFile.ClassThumbnail = classItem.ThumbnailFullName;
                        }
                    }

                    

                    _syncCADEvents.ClassItems = _product.PdClassItems;
                    _asInnovator.ClassItems= _product.PdClassItems;
                }

                this.PartsLibrary = _product.PartsLibrary;
                _asInnovator.PartsLibrary = this.PartsLibrary;
                _syncCADEvents.PartsLibrary = this.PartsLibrary;
                //_syncCADEvents = new SyncCADEvents();


                ResetPropertiesListItem();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Uri GetUri(string fullName)
        {
            try
            {
                Uri uri = new Uri(fullName);
                return uri;

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 重新設定屬性的List
        /// </summary>
        private void ResetPropertiesListItem()
        {
            try
            {

                 _lists = new Dictionary<string, Lists>();
                if (_product == null) return;
                
                //foreach (ClassItem classItem in _product.PdClassItem?.Where(c => c.Name != ""))
                foreach (ClassItem classItem in _product.PdClassItems?.Where(c => String.IsNullOrWhiteSpace(c.Name) ==false))
                {
                  foreach( PLMProperty plmProperty in classItem.CsProperties.Where(x=>x.DataSource!="" && (x.DataType=="list" || x.DataType == "filter list")))
                    {
                        AddPropertyListItem(plmProperty);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增屬性的ListItem
        /// </summary>
        /// <param name="plmProperty"></param>
        private void AddPropertyListItem(PLMProperty plmProperty)
        {
            try
            {
                
                //if (plmProperty.DataSource == "" || (plmProperty.DataType != "list" && plmProperty.DataType != "filter list")) return;
                if (String.IsNullOrWhiteSpace(plmProperty.DataSource) || (plmProperty.DataType != "list" && plmProperty.DataType != "filter list")) return;
                Lists lists = _lists.Where(x => x.Key == plmProperty.DataSource)?.FirstOrDefault().Value;
                if (lists == null)
                {
                    bool isFilter = (plmProperty.DataType == "list") ? false : true;
                    lists=_asInnovator.GetList(plmProperty.DataSource, isFilter);
                    _lists.Add(plmProperty.DataSource, lists);
                }

                plmProperty.PLMList = lists;

                ObservableCollection<PLMListItem> addListItems = lists.ListItems;
                if (plmProperty.DataType == "filter list" && plmProperty.Pattern!="" && plmProperty.SoruceSearchItem != null)
                {
                    PLMProperty sProperty = plmProperty.SoruceSearchItem.PlmProperties.Where(x => x.Name == plmProperty.Pattern).FirstOrDefault();
                    if (sProperty != null)
                    {
                        addListItems = (ObservableCollection<PLMListItem>)lists.ListItems.Where(x => x.Filter == sProperty.DataValue);
                    }
                }
                plmProperty.ListItems = addListItems;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增查詢PLM物件
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="item"></param>
        /// <param name="rules"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private SearchItem AddNewSearchItem(List<SearchItem> searchItems, Aras.IOM.Item item, List<ConditionalRule> rules, string className, string addedFilename,string itemId,string fileId)
        {
            try
            {
                SearchItem searchItem = searchItems.Where(x => x.FileName == addedFilename).FirstOrDefault();
                if (searchItem == null)
                {
                    searchItem = new SearchItem();
                    searchItems.Add(searchItem);
                    searchItem.ItemType = ItemTypeName.CAD.ToString(); //Modify by kenny 2020/08/13
                }
                searchItem.IsCurrent = (item.getProperty("is_current", "0")=="1")? true : false ;
                
                searchItem.IsAdded = (addedFilename == "") ? false : true;
                searchItem.IsExist = true;
                searchItem.FileName = item.getProperty("filename", "");
                searchItem.ItemConfigId = item.getProperty("config_id", "");
                searchItem.KeyedName  = item.getProperty("keyed_name", ""); 

                searchItem.ItemId = itemId;
                searchItem.ClassName = (className != "") ? className : GetClassKey(item);
                searchItem.ClassThumbnail = GetClassThumbnail(searchItem.ClassName);
                searchItem.FileId = fileId;
                searchItem.RuleProperties = GetRuleProperties(rules, item);


                searchItem.Thumbnail = item.getProperty(ThumbnailProperty, "");

                //Modify by kenny 2020/08/20 bcs_library_path
                searchItem.LibraryPath = item.getProperty("bcs_library_path", "");
                searchItem.IsCommonPart = System.Convert.ToBoolean(int.Parse(item.getProperty("bcs_is_common_part", "0")));
                //searchItem.IsStandardPart = System.Convert.ToBoolean(int.Parse(item.getProperty("bcs_is_standard_part", "0")));
                searchItem.IsStandardPart = System.Convert.ToBoolean(int.Parse(item.getProperty("is_standard", "0")));
                return searchItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
       
        /// <summary>
        /// 取得CAD類別定義鍵值
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private List<PLMKeys> GetClassItemKeys(string className)
        {
            try
            {
                if (_product==null) return null;
                List<PLMKeys> classKeys = _product.PdClassItems?.Where(c => c.Name == className)?.First()?.CsKeys;
                return classKeys;
            }
            catch (Exception ex)
            {
                //throw ex;
                string strError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 取得CAD類別
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private string GetClassName(string extension)
        {
            try
            {
                if (_product == null) return null;
                extension = (extension.Substring(0, 1)==".")? extension.Substring(1):extension;
                if (extension == "") return "";
                string className = _product.PdClassItems?.Where(c => c.Extension.ToLower() == extension.ToLower()).Select(x=>x.Name).FirstOrDefault();
                return className;
            }
            catch (Exception ex)
            {
                throw ex;
                //string strError = ex.Message;
            }
        }


        /// <summary>
        /// 取得所有CAD類別特定屬性值 (<CAD類別, Dictionary<屬性, 屬性值>>)
        /// 
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Dictionary<string, string>> GetClassesKeys()
        {
            try
            {

                if (_classesKeys != null) return _classesKeys;
                if (_product == null) return null;

                _classesKeys = new Dictionary<string, Dictionary<string, string>>();

                Dictionary<string, string> allKeys = new Dictionary<string, string>();
                foreach (var classItem in _product.PdClassItems)
                {

                    List<PLMKeys> plmKeys = classItem.CsKeys;
                    Dictionary<string, string> keys = new Dictionary<string, string>();
                    foreach (PLMKeys plmKey in plmKeys)
                    {
                        keys.Add(plmKey.Name, plmKey.Value);
                    }
                    _classesKeys.Add(classItem.Name, keys);
                }


                return _classesKeys;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 所有CAD類別特定屬性值(依屬性分類<屬性, List<屬性值>)
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<string>> GetAllKeys()
        {
            try
            {

                if (_allKeys != null) return _allKeys;
                if (_product == null) return null;

                _allKeys = new Dictionary<string, List<string>>();
                foreach (var classItem in _product.PdClassItems)
                {
                    foreach (PLMKeys plmKey in classItem.CsKeys)
                    {
                        if (_allKeys.ContainsKey(plmKey.Name) == false)
                        {
                            List<string> value = new List<string>();
                            value.Add(plmKey.Value);
                            _allKeys.Add(plmKey.Name, value);
                        }
                        else
                        {
                            //List<string> value = _allKeys.Where(x => x.Key == plmKey.Name).Select(x => x.Value).ElementAt(0);
                            List<string> value = _allKeys.Where(x => x.Key == plmKey.Name).Select(x => x.Value).FirstOrDefault();
                            if (value.Exists(e => e.EndsWith(plmKey.Value)) == false) value.Add(plmKey.Value);
                        }
                    }
                }


                return _allKeys;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得分類
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        //private string GetClassKey(Aras.IOM.Item item,ref string extension)
        private string GetClassKey(Aras.IOM.Item item)
        {
            try
            {
                foreach (var classItem in _product.PdClassItems)
                {
                    if (CheckClassKeyValues(item, classItem)) return classItem.Name; 
                }
               
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得類別的圖示
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private string GetClassThumbnail(string className)
        {
            try
            {
                foreach (var classItem in _product.PdClassItems.Where(x=>x.Name== className))
                {
                    return classItem.ThumbnailFullName;
                }

                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查值是否符合分類定義
        /// </summary>
        /// <param name="item"></param>
        /// <param name="classItem"></param>
        /// <returns></returns>
        private bool CheckClassKeyValues(Aras.IOM.Item item, ClassItem classItem)
        {
            try
            {


                foreach (PLMKeys plmKey in classItem.CsKeys)
                {
                    if (item.getProperty(plmKey.Name, "") != plmKey.Value) return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion




    }

}
