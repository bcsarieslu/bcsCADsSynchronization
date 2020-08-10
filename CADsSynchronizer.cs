
#region "                   名稱空間"
using BCS.CADs.Synchronization.Entities;
using BCS.CADs.Synchronization.PLMList;
using BCS.CADs.Synchronization.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BCS.CADs.Synchronization;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.ConfigProperties;
using System.Collections.ObjectModel;
using BCS.CADs.Synchronization.ViewModels;
using Microsoft.Win32;
using System.Windows.Controls;
//using System.Windows.Forms;
#endregion


namespace BCS.CADs.Synchronization
{
    public class CADsSynchronizer : IDisposable
    {


        #region "                   宣告區"
        ConnPLM _Plm = new ConnPLM();
        private SyncCADEvents _syncCADEvents = new SyncCADEvents();
        #endregion

        #region "                   屬性"
        /// <summary>
        /// CAD軟體名稱
        /// </summary>
        public string CADSoftware { get; set; }

        /// <summary>
        /// 使用者是否登錄
        /// </summary>
        public bool IsActiveLogin { get; set ; } = false;


        /// <summary>
        /// 縮圖屬性
        /// </summary>
        public string ThumbnailProperty {
            get
            {
                return _Plm.ThumbnailProperty;
            }
        } 

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"


        /// <summary>
        /// 回收資源
        /// </summary>
        public void Dispose()
        {
        }



        /// <summary>
        /// 登錄
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strDBName"></param>
        /// <param name="strLogin"></param>
        /// <param name="strPassword"></param>
        protected internal void Login(string strUrl, string strDBName, string strLogin, string strPassword)
        {
            try
            {

                IsActiveLogin =false ;
                _Plm.Url = strUrl;
                _Plm.Database = strDBName;
                _Plm.LoginName = strLogin;
                _Plm.Password  = strPassword;
                _Plm.CADSoftware = CADSoftware;
                _Plm.Login();
                _syncCADEvents.LanguageResources = GetLanguageResources();
                _syncCADEvents.IsResolveAllLightweightSuppres = _Plm.IsResolveAllLightweightSuppres;
                _syncCADEvents.IsResolveAllSuppres = _Plm.IsResolveAllSuppres;
                
                IsActiveLogin = _Plm.IsActiveLogin;
                if (IsActiveLogin == true) _syncCADEvents.ClassItems = _Plm.GetClassItems();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        protected internal void Logoff()
        {
            try
            {
                _Plm.Logoff();
                IsActiveLogin = _Plm.IsActiveLogin;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得查詢物件
        /// </summary>
        /// <param name="search"></param>
        protected internal List<SearchItem> GetPLMSearchItems(Dictionary<string, List<string>> search)
        {
            try
            {
                List<SearchItem> searchItems = _Plm.GetPLMSearchItems(search);

                return searchItems;
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
        protected internal List<PLMProperty> GetCADProperties(SearchItem searchItem, string className)
        {
            try
            {
                //多筆(集合物件)
                return _Plm.GetCADProperties(searchItem,className);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得物件屬性值(依查詢結果)
        /// </summary>
        /// <param name="searchItems"></param>
        protected internal void GetCADProperties(List<SearchItem> searchItems)
        {
            try
            {
                //多筆(集合物件)
                _Plm.GetCADProperties(searchItems,false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得物件屬性值(依查詢結果)
        /// </summary>
        /// <param name="search"></param>
        protected internal void GetCADProperties(Dictionary<string, List<string>> search)
        {
            try
            {
                List<SearchItem> searchItems = _Plm.GetPLMSearchItems(search);
                GetCADProperties(searchItems);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步PLM屬性
        /// </summary>
        /// <param name="search"></param>
        protected internal List<SearchItem> SynToPLMItemProperties(List<SearchItem> searchItems, IsLock isLock)
        {
            try
            {

                //多筆(集合物件)
                List<SearchItem> retSearchItems =  _Plm.SynToPLMItemProperties(searchItems, isLock);
                
                return retSearchItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得CAD結構
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        protected internal void GetPLMItemsStructure(List<SearchItem> searchItems, SyncType type)
        {
            try
            {
                _Plm.CADItemsStructure(searchItems, type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步PLM結構及檔案
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="isLock"></param>
        /// <param name="isNewGeneration"></param>
        /// <returns></returns>
        protected internal bool SynToPLMItemsStructure(List<SearchItem> searchItems, IsLock isLock, IsNewGeneration isNewGeneration)
        {
            try
            {

                return _Plm.SynToPLMItemStructure(searchItems, isLock, isNewGeneration); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得CAD結構
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        protected internal void GetCADStructure(SyncType type,ref List<SearchItem> searchItems)
        {
            try
            {
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage("GetCADStructure", "ExecCadEvent", "", "Start");
                _syncCADEvents.ExecCadEvent(_Plm.AsInnovator, SyncCadCommands.GetActiveCADStructure.ToString(), ref searchItems, null, null, SyncEvents.OnGetCADStructure, type);


                itemMessage.Value = "CADItemsStructure";
                //當SyncType.SyncFromPLM,其結構是取PLM的結構
                if (SyncType.SyncFromPLM == type) _Plm.CADItemsStructure(searchItems, type);

                itemMessage.Value = "GetSearchItemsCADIds";
                _Plm.GetSearchItemsCADIds(type,searchItems);
                itemMessage.Status = "End";
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("GetCADStructure", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                //throw ex;
            }
        }

        /// <summary>
        /// 同步的插入分件或替換分件結構異動 (只更新searchItems)
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="structuralChange"></param>
        /// <returns></returns>
        protected internal void SynStructuralChangeItems(List<SearchItem> searchItems, StructuralChange structuralChange)
        {
            try
            {

               _Plm.SynStructuralChangeItems(searchItems, structuralChange);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected internal void AddNewFileNameProperty(SearchItem searchItem, ObservableCollection<PLMProperty> plmProperties, string label,string newFileName)
        {
            try
            {

                _Plm.AddNewFileNameProperty(searchItem, plmProperties, label, newFileName);

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
        protected internal void UpdateSearchItemType(DataGrid gridSelectedItems, SearchItem searchItemType)
        {
            try
            {

                _Plm.UpdateSearchItemType(gridSelectedItems, searchItemType);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得圖檔
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected internal string GetImageFullName(string value)
        {
            try
            {

                return _Plm.GetImageFullName(value);

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
        protected internal ItemType GetItemType(string name, bool isSearch)
        {
            try
            {

                return _Plm.GetItemType(name, isSearch);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得CAD結構
        /// </summary>
        /// <param name="searchItems"></param>
        /// <returns></returns>
        protected internal SearchItem GetActiveCADDocument()
        {
            try
            {

                List<SearchItem> searchItems=null;
                _syncCADEvents.ExecCadEvent(_Plm.AsInnovator,SyncCadCommands.GetActiveCADDocument.ToString(), ref searchItems,null, null, SyncEvents.None, SyncType.Structure);

                //var aa = searchItems.FirstOrDefault(x => x.FileName != "" && x.IsActiveCAD == true)?.FileName ?? string.Empty;
                
                //SearchItem searchItem = searchItems.FirstOrDefault(x => x.FileName != "" && x.IsActiveCAD == true);
                SearchItem searchItem = searchItems.FirstOrDefault(x => String.IsNullOrWhiteSpace(x.FileName) ==false && x.IsActiveCAD == true);
                return searchItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 執行外部程式功能
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        protected internal SearchItem RunFunction(ClassPlugin plugin, SearchItem searchItem)
        {
            try
            {
                List<SearchItem> searchItems = new List<SearchItem>();
                searchItems.Add(searchItem);
                _Plm.GetCADProperties(searchItems, true);

                IntegrationEvents integrationEvents = new IntegrationEvents();
                integrationEvents.ComponentAssemblyName = plugin.ComponentAssemblyName;
                integrationEvents.ComponentClassName = plugin.ComponentClassName;
                integrationEvents.ComponentFileName = plugin.ComponentFileName;
                integrationEvents.MethodName = SyncCadCommands.RunFunction.ToString();
               
                integrationEvents.Name = plugin.Name;
                _syncCADEvents.ExecCadEvent(_Plm.AsInnovator, SyncCadCommands.RunFunction.ToString(), ref searchItem,null, integrationEvents, SyncEvents.None, SyncType.PluginModule);
                return searchItem;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 範得範本
        /// </summary>
        /// <returns></returns>
        protected internal Dictionary<string, List<ClassTemplateFile>> GetClassesTemplates()
        {
            try
            {
                return _Plm.GetClassesTemplates();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得類別分類
        /// </summary>
        /// <returns></returns>
        protected internal List<ClassItem> GetClassItems()
        {
            try
            {
                return _Plm.GetClassItems();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得PlugIn Functions
        /// </summary>
        /// <returns></returns>
        protected internal ObservableCollection<ClassPlugin> GetClassPlugins(string className)
        {
            try
            {

                return _Plm.GetClassPlugins(className);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 執行同步更新CAD圖檔屬性
        /// </summary>
        /// <param name="searchItems"></param>
        protected internal bool SyncCADsProperties(ref List<SearchItem> searchItems, SyncType type)//, SyncType synctype
        {
            try
            {

                //Sync: PLM (Add/Update)
                if (_Plm.SyncCADsProperties(ref searchItems, type)==false ) return false ;
                _syncCADEvents.ExecCadEvent(_Plm.AsInnovator,SyncCadCommands.UpdateCADsProperties.ToString(), ref searchItems,null, null, SyncEvents.OnUpdateCADsProperties, type);
                return true;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("SyncCADsProperties", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return false;
                //throw ex;
            }
        }


        /// <summary>
        /// CADs圖檔的檔案及結構同步
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="type"></param>
        protected internal bool SyncCADsFilesStructure(ref List<SearchItem> searchItems, List<StructuralChange> structureChanges, SyncType type)//, SyncType synctype ,bool isGetProperties
        {
            try
            {

                _Plm.GetSearchItemsCADIds(type,searchItems);             
                _Plm.SyncCADsFilesStructure(ref searchItems, structureChanges, type);
                return true;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("SyncCADsFilesStructure", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return false;
                //throw ex;
            }
        }

        /// <summary>
        /// 鎖定或解除鎖定
        /// </summary>
        /// <param name="searchItems"></param>
        protected internal bool LockOrUnlock(List<SearchItem> searchItems)
        {
            try
            {
                _Plm.LockOrUnlock(searchItems);
                return true;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("SyncCADsFilesStructure", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return false;
                //throw ex;
            }
        }

        /// <summary>
        /// CAD是否有Active CAD
        /// </summary>
        /// <returns></returns>
        protected internal bool IsActiveCAD()
        {

            try
            {
                SyncCADEvents syncCADEvents =_Plm.GetSyncCADEvents;
                return syncCADEvents.IsActiveCADEvent();
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
        protected internal SearchItem GetActiveSearchItem(List<SearchItem> searchItems)
        {
            try
            {


                return _Plm.GetActiveSearchItem(searchItems);

            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("GetClassesTemplates", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return null;
            }
        }

        /// <summary>
        /// 取得所選到的PLM的CAD物件屬性值for List<SearchItem>
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="type"></param>
        protected internal bool GetSelectedCADsProperties(List<SearchItem> searchItems, SyncType type)
        {
            try
            {
                //Events
                _Plm.GetCADProperties(searchItems, true);
                _syncCADEvents.ExecCadEvent(_Plm.AsInnovator,SyncCadCommands.GetSelectedCADsProperties.ToString(), ref searchItems,null, null, SyncEvents.OnGetCADsProperties, type);
                foreach(SearchItem searchItem in searchItems.Where(x=>x.IsViewSelected==true))
                {
                    searchItem.VersionStatus = _Plm.GetVersionStatus(searchItem).ToString();
                }
                return true; 
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("GetSelectedCADsProperties", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return false;
                //throw ex;
            }
        }

        /// <summary>
        /// 複製轉新增的圖檔名稱
        /// </summary>
        /// <param name="searchItems"></param>
        protected internal void SearchItemsCopyFileNameProperty(List<SearchItem> searchItems)
        {
            try
            {
                _Plm.SearchItemsCopyFileNameProperty(searchItems);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢驗同步規則
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="type"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected internal bool SyncCheckRules(IEnumerable<SearchItem> searchItems, SyncType type, SyncOperation operation)
        {
            try
            {
                return _Plm.SyncCheckRules(searchItems, type, operation);
            }
            catch (Exception ex)
            {
                //throw ex;
                ClsSynchronizer.VmMessages.AddItemMessage("SyncCheckRules", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return false;
            }
        }

        /// <summary>
        /// 取得範本屬性
        /// </summary>
        /// <param name="obslistTemplates"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected internal ObservableCollection<SearchItem> GetTemplatesProperties(ObservableCollection<ClassTemplateFile> obslistTemplates, string filePath)
        {
            try
            {
                //Events
                ObservableCollection<SearchItem> ObsSearchItems = _Plm.GetTemplatesProperties(obslistTemplates, filePath);

                return ObsSearchItems;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage("GetTemplatesProperties", ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return null;
                //throw ex;
            }
        }

        /// <summary>
        /// 同步查詢item Type物件
        /// </summary>
        /// <param name="searchItemType"></param>
        /// <param name="LoadFromPLM"></param>
        /// <returns></returns>
        protected internal ObservableCollection<SearchItem> SyncItemTypeSearch(SearchItem searchItemType)
        {
            try
            {
                //Events
                ObservableCollection<SearchItem> ObsSearchItems = _Plm.SyncItemTypeSearch(searchItemType);

                return ObsSearchItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 鎖定CAD物件
        /// </summary>
        /// <param name="search"></param>
        protected internal List<SearchItem> LockPLMItems(List<SearchItem> searchItems)
        {
            try
            {

                //多筆(集合物件)
                List<SearchItem> retSearchItems = _Plm.LockItems(searchItems);
                return retSearchItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// CAD物件解鎖
        /// </summary>
        /// <param name="search"></param>
        protected internal List<SearchItem> UnlockPLMItems(List<SearchItem> searchItems)
        {
            try
            {

                //多筆(集合物件)
                List<SearchItem> retSearchItems = _Plm.UnlockItems(searchItems);
                return retSearchItems;
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
        protected internal List<SearchItem> LoadFromPLM(string directory, List<SearchItem> searchItems, SyncType type)
        {
            try
            {
                searchItems = _Plm.CADItemsStructure(searchItems, type);

                //List<SearchItem> retSearchItems = (type== SyncType.CopyToAdd)? _Plm.CopyToAdd(directory, searchItems, type): _Plm.LoadFromPLM(directory, searchItems, type);
                List<SearchItem> retSearchItems = _Plm.LoadFromPLM(directory, searchItems, type);

                return retSearchItems;
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage(type.ToString(), ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return null;
                //throw ex;
            }
        }


        /// <summary>
        /// 取得PLM的CAD所有結構
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected internal List<SearchItem> CADItemStructure( SearchItem searchItem, SyncType type)
        {
            try
            {
                return _Plm.CADItemStructure(searchItem.ItemId, type);
            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage(type.ToString(), ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return null;
                //throw ex;
            }
        }
        protected internal string GetFileNameByCADItemId(string itemId)
        {
            try
            {
                return _Plm.GetFileNameByCADItemId(itemId);
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
        /// <param name="directory"></param>
        /// <param name="type"></param>
        protected internal bool CopyToAdd(List<SearchItem> searchItems, string directory, SyncType type)
        {
            try
            {
                ItemMessage itemMessage = ClsSynchronizer.VmMessages.AddItemMessage(type.ToString(), "", "", "Start");
                bool ret = _Plm.CopyToAdd(searchItems, directory, type);
                if (ret) itemMessage.Status = "End";
                return ret;

            }
            catch (Exception ex)
            {
                ClsSynchronizer.VmMessages.AddItemMessage(type.ToString(), ex.Message, ex.StackTrace, "Error", new ItemException(1, ex.Message, ex.StackTrace, ex));
                return false;
            }

        }


        /// <summary>
        /// 檢查副檔名是否正確
        /// </summary>
        /// <param name="cadItemId"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        //protected internal bool CheckAddCADFile(string cadItemId,string full)
        //{
        //    try
        //    {

        //        return _Plm.CheckAddCADFile(cadItemId, full);
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
        protected internal bool AddCADFromTemplates(ObservableCollection<SearchItem> obsSearchItems)
        {
            try
            {

                //新增CAD物件
                return _Plm.AddCADFromTemplates(obsSearchItems);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 新增CAD物件
        /// </summary>
        /// <param name="className"></param>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected internal SearchItem AddCADFromTemplate(string className, string templateName, string directory, string fileName, List<PLMProperty> properties)
        {
            try
            {

                //新增CAD物件
               return _Plm.AddCADFromTemplate(className, templateName, directory, fileName, properties);
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
        protected internal Lists GetDBList(string url)
        {
            try
            {

                Lists listItems = _Plm.GetDBList(url);
                return listItems;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected internal string GetUrl()
        {
            try
            {
                return _Plm.Url;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 元件路徑
        /// </summary>
        protected internal static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// 提供測試HelloWorld
        /// </summary>
        protected internal void HelloWorld()
        {
            try
            {

                MessageBox.Show($"Hello World ! {DateTime.Now}");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 選取路徑
        /// </summary>
        /// <param name="selectedPath"></param>
        /// <returns></returns>
        protected internal string SelectFolder(string selectedPath)
        {
            try
            {

                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "選取路徑";
                dialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
                dialog.SelectedPath = (selectedPath != "") ? selectedPath : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//UserProfile
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ClsSynchronizer.VmDirectory = dialog.SelectedPath;
                    return ClsSynchronizer.VmDirectory;
                }
                return "";

            }
            catch (Exception ex)
            {
                //throw ex;
                string message = ex.Message;
                return "";
            }

        }

        protected internal string CopyToAddSaveFile(string selectedPath,string filter)
        {
            try
            {
                
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = (selectedPath != "") ? Path.GetDirectoryName(selectedPath) : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog1.Filter = filter;
                saveFileDialog1.Title = ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("label_CopyFileName");// "複製轉新增(圖檔名稱)";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    ClsSynchronizer.VmDirectory = saveFileDialog1.FileName;
                    return ClsSynchronizer.VmDirectory;
                }
                return "";



            }
            catch (Exception ex)
            {
                //throw ex;
                string message = ex.Message;
                return "";
            }

        }

        /// <summary>
        /// 設定介面語言(載入語系)
        /// </summary>
        /// <param name="win"></param>
        protected internal void LoadLanguage(dynamic win)
        {
            ResourceDictionary dict = _Plm.LanguageResources;
            win.Resources.MergedDictionaries.Add(dict);
        }

        /// <summary>
        /// 取得目前語系資源
        /// </summary>
        /// <param name="win"></param>
        public ResourceDictionary GetLanguageResources()
        {
            return _Plm.LanguageResources;
        }

        /// <summary>
        /// 設定語系資源
        /// </summary>
        /// <param name="win"></param>
        public void SetLanguageResources(string language)
        {
            ClsSynchronizer.Language = language;
            SetLanguageResources();
        }

        /// <summary>
        /// 設定語系資源
        /// </summary>
        /// <param name="win"></param>
        protected internal void SetLanguageResources()
        {
            _Plm.SetLanguageResources();
            _syncCADEvents.LanguageResources = GetLanguageResources();
        }

        private string GetSyncLanguagesCSV()
        {

           
            StringBuilder sb = new StringBuilder();
            sb.Append("key");
            Dictionary<string, Dictionary<string,string>> dicLanguage = new Dictionary<string, Dictionary<string, string>>();
            List<string> langs = new List<string>();
            string path = AssemblyDirectory + @"\Lang"; 
            foreach (var lang in Directory.GetFiles(path).Select(x => Path.GetFileNameWithoutExtension(x)))
            {
                sb.Append($",{lang}");
                langs.Add(lang);
                SetLanguageResources(lang);
                ResourceDictionary resLang = GetLanguageResources();
                foreach (string key in resLang.Keys)
                {
                    Dictionary<string, string> item = dicLanguage.Where(x => x.Key == key).Select(x=>x.Value).FirstOrDefault();
                    if (item == null)
                    {
                        item = new Dictionary<string, string>();
                        dicLanguage.Add(key, item);
                    }
                    item.Add(lang, resLang[key].ToString());                    
                }
            }

            foreach (var keyItem in dicLanguage)
            {
                sb.Append($"\r\n{keyItem.Key}");
                foreach (string lang in langs)
                {
                    var item = keyItem.Value.Where(x => x.Key == lang).Select(x=>x.Value).FirstOrDefault();
                    string value = (item == null) ? "" : item;
                    sb.Append($",{value}");
                }
            }

            if (File.Exists(Path.Combine(AssemblyDirectory, "CADsSynchronizationLang.csv")))
                File.Delete(Path.Combine(AssemblyDirectory, "CADsSynchronizationLang.csv"));

            FileStream fs = new FileStream(Path.Combine(AssemblyDirectory, "CADsSynchronizationLang.csv"), FileMode.CreateNew);
            using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
            {
                writer.Write(sb.ToString());
            }
            fs.Close();
            fs.Dispose();

            return sb.ToString();
        }

        /// <summary>
        /// 取得Language值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected internal string GetLanguageByKeyName(string key)
        {
            return _Plm.GetLanguageByKeyName(key);
        }

        #endregion

        #region "                   方法(內部)"

        #endregion




    }

}
