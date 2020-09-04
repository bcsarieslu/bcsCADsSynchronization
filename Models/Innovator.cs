
#region "                   名稱空間"
using Aras.IOM;
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Entities;
using BCS.CADs.Synchronization.PLMList;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
#endregion


namespace BCS.CADs.Synchronization.Models
{



    internal class Innovator 
    {
        #region "                   宣告區"
        /// <summary>
        /// Dictionary key = HttpServerConnection
        /// </summary>
        private static Dictionary<string, HttpServerConnection> _asServerConn = new Dictionary<string, HttpServerConnection>();
        /// <summary>
        /// Dictionary key = Aras.IOM.Innovator
        /// </summary>
        private static Dictionary<string, Aras.IOM.Innovator> _asInnovator = new Dictionary<string, Aras.IOM.Innovator>();

        /// <summary>
        /// CAD產品之相關定義:XDcoument
        /// </summary>
        private XDocument  _xmlConfigurations = null;

        private XElement _cadRequiredProperties = null;

        private Dictionary<string, Lists> _lists { get; set; } = new Dictionary<string, Lists>();

        private Dictionary<string, Dictionary<string, string>> _language { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        private SyncCADEvents _syncCADEvents = new SyncCADEvents();


        #endregion

        #region "                   屬性"
        /// <summary>
        /// CAD軟體名稱
        /// </summary>
        internal string CADSoftware { get; set; } = "";

        internal List<ClassItem> ClassItems
        {
            set
            {
                _syncCADEvents.ClassItems = value;
            }

        }
        CommonPartsLibrary _partsLibrary = new CommonPartsLibrary();
        internal CommonPartsLibrary PartsLibrary {
            set{
                _partsLibrary = value;
                _syncCADEvents.PartsLibrary = _partsLibrary;
            }
        }



        private ResourceDictionary _languageResources;
        protected internal ResourceDictionary LanguageResources
        {
            get
            {
                return _languageResources;
            }
            set
            {
                _languageResources = value;
                _syncCADEvents.LanguageResources = _languageResources;
            }
        }

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

        internal bool IsResolveAllLightweightSuppres
        {
            set
            {
                _syncCADEvents.IsResolveAllLightweightSuppres = value;
            }

        }
        internal bool IsResolveAllSuppres
        {
            set
            {
                _syncCADEvents.IsResolveAllSuppres = value;
            }

        }

        /// <summary>
        /// 原始檔案屬性
        /// </summary>
        internal string NativeProperty { get; set; } = "native_file";

        internal string ThumbnailProperty { get; set; } = "thumbnail";
        

        /// <summary>
        /// Get Aras.IOM.Innovator
        /// </summary>
        internal Aras.IOM.Innovator AsInnovator
        {
            get
            {
                Aras.IOM.Innovator value = null;
                if (_asInnovator.ContainsKey("Aras.IOM.Innovator")) value = _asInnovator["Aras.IOM.Innovator"];
                return value;
            }
        }


        Dictionary<string, Dictionary<string, List<SyncEvents>>> _execSyncEvents;
        private Dictionary<string, Dictionary<string, List< SyncEvents>>> ExecSyncEvents
        {
            get
            {

                if (_execSyncEvents == null)
                {
                    _execSyncEvents = new Dictionary<string, Dictionary<string, List<SyncEvents>>>();

                    AddSyncEvents( SyncEvents.OnLoadFromPLMDownloadItemBefore, SyncEvents.OnLoadFromPLMDownloadItemAfter, SyncCadCommands.SystemLoadFromPLM, SyncEvents.OnLoadFromPLMDownloadBefore);

                    AddSyncEvents( SyncEvents.OnSyncToPLMItemStructureBefore, SyncEvents.OnSyncToPLMItemStructureAfter, SyncCadCommands.SystemSyncToPLMStructure, SyncEvents.OnSyncToPLMStructureBefore);

                    AddSyncEvents( SyncEvents.OnSyncFormItemStructureBefore, SyncEvents.OnSyncFormItemStructureAfter, SyncCadCommands.SystemSyncFromPLMStructure, SyncEvents.OnSyncFormStructureBefore);

                    AddSyncEvents( SyncEvents.OnSyncToPLMItemBefore, SyncEvents.OnSyncToPLMItemAfter, SyncCadCommands.SystemSyncToPLMProperties, SyncEvents.OnSyncToPLMBefore);

                    AddSyncEvents( SyncEvents.OnSyncFormPLMItemBefore, SyncEvents.OnSyncFormPLMItemAfter, SyncCadCommands.SystemSyncFromPLMProperties, SyncEvents.OnSyncFormPLMBefore);

                    AddSyncEvents( SyncEvents.OnAddFromTemplateItemBefore, SyncEvents.OnAddFromTemplateItemAfter, SyncCadCommands.SystemAddFromTemplate, SyncEvents.OnAddFromTemplateBefore);

                    AddSyncEvents(SyncEvents.OnCopyToAddItemBefore, SyncEvents.OnCopyToAddItemAfter, SyncCadCommands.SystemCopyToAdd, SyncEvents.OnCopyToAddItemBefore);

                }
                return _execSyncEvents;
            }
        }



        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"


        /// <summary>
        /// HttpServerConnection Login for default connection
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strDBName"></param>
        /// <param name="strLogin"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        protected internal bool UserLogin(string strUrl, string strDBName,string strLogin,string strPassword)
        {
            try
            {

                return UserLogin(false, strUrl, strDBName, strLogin, strPassword);


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// HttpServerConnection Login for Windows Authentication (using Microsoft Active Directory service domain)
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strDBName"></param>
        /// <returns></returns>
        protected internal bool WinAuthLogin(string strUrl, string strDBName)
        {
            try
            {

                return UserLogin(true , strUrl, strDBName,"","");


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HttpServerConnection Logout
        /// </summary>
        /// <returns></returns>
        protected internal bool UserLogoff()
        {
            try
            {

                if (_asServerConn.ContainsKey("HttpServerConnection"))
                {
                    HttpServerConnection value = _asServerConn["HttpServerConnection"];
                    value.Logout();
                    _asServerConn.Remove("HttpServerConnection");
                }
                if (_asInnovator.ContainsKey("Aras.IOM.Innovator")) _asInnovator.Remove("Aras.IOM.Innovator");
                return true;

            }
            catch (Exception ex)
            {
                throw ex; 
            }

        }

        /// <summary>
        /// 取得CAD產品相關定義
        /// </summary>
        /// <param name="strProductName"></param>
        /// <returns></returns>
        protected internal XDocument GetConfigurations(string strProductName)
        {
            try
            {
                //Modify by kenny 2020/09/03
                //if (_xmlConfigurations != null ) return _xmlConfigurations;

                string aml = GetCADConfigAML(strProductName);

                Aras.IOM.Item itemConfigurations = AsInnovator.applyAML(aml);
                if (itemConfigurations.isError()) return _xmlConfigurations;

                //XmlDocument xmlDocument = new XmlDocument();
                string xml = itemConfigurations.ToString();
                //xmlDocument.LoadXml(xml);
                //_xmlConfigurations = ToXDocument(xmlDocument);
                _xmlConfigurations = XDocument.Parse(xml);
                return _xmlConfigurations;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// SQL Command 查詢 (CAD程式,只能在測試使用,要將方法寫在Server Events)
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        protected internal Aras.IOM.Item RunQueryForTest(string strSql)
        {
            try
            {
                Aras.IOM.Item queryItems = null;
                string strMethodName = "";

                if (AsInnovator.getItemByKeyedName("Method", "bcs_RunQuery") != null)
                {
                    strMethodName = "bcs_RunQuery";
                    if (strSql.ToLower().IndexOf("<sqlCommend>".ToLower()) < 0)
                    {
                        strSql = strSql.Replace("<=", "@le@").Replace("<>", "@ne@").Replace("<", "@lt@");
                        strSql = "<sqlCommend>" + strSql + "</sqlCommend>";
                    }
                }
                else
                {
                    if (AsInnovator.getItemByKeyedName("Method", "ExecuteSQL") != null)
                    {
                        strMethodName = "ExecuteSQL";
                        if (strSql.ToLower().IndexOf("<SQLCMD>".ToLower()) < 0) strSql = "<SQLCMD>" + strSql + "</SQLCMD>";
                    }
                }
                if (strMethodName == "")
                {
                    queryItems = AsInnovator.applySQL(strSql);
                }
                else
                {
                    queryItems = AsInnovator.applyMethod(strMethodName, strSql);
                }

                return queryItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 取得CAD同步整合物件
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        protected internal Aras.IOM.Item GetPLMSearchItems(string sqlCommand ,string rules, string files,string conditions)
        {
            try
            {
                Aras.IOM.Item queryItems = null;
                
                if (AsInnovator.getItemByKeyedName("Method", "bcs_CAD_GetPLMSearchItems") != null)
                {
                    string strSql = "<sqlCommand>" + sqlCommand + "</sqlCommand><rules>" + rules + "</rules><files>" + files + "</files><conditions>" + conditions + "</conditions>";
                    queryItems = AsInnovator.applyMethod("bcs_CAD_GetPLMSearchItems", strSql);
                }
                else
                {
                    string strSql = "select " + String.Format(sqlCommand, rules, files, conditions);
                    queryItems = AsInnovator.applySQL(strSql);
                }
                return queryItems;
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
        protected internal Lists GetList(string listName, bool isFilter)
        {
            try
            {

                List<Lists> listItems = _lists.Where(x => x.Key == listName).Select(x => x.Value).ToList();
                //if (listItem.Count() > 0) return listItem.ElementAt(0);
                if (listItems.Count() > 0) return listItems.FirstOrDefault();
                List<string> selects = new List<string> { "value", "id", isFilter ? "sort_order,filter" : "sort_order", "label" };
                List<string> code = new List<string>();
                foreach (var lagnlist in _language)
                {
                    //string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).ElementAt(0);
                    string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).FirstOrDefault();
                    //if (suffix != "")
                    if (String.IsNullOrWhiteSpace(suffix) ==false)
                    {
                        selects.AddRange(new List<string> { "label" + suffix });
                        //if (lagnlist.Value.Where(x => x.Key == "code").Select(x => x.Value).ElementAt(0) != "") code.AddRange(new List<string> { lagnlist.Value.Where(x => x.Key == "code").Select(x => x.Value).ElementAt(0) });
                        if (lagnlist.Value.Where(x => x.Key == "code").Select(x => x.Value).FirstOrDefault() != "") code.AddRange(new List<string> { lagnlist.Value.Where(x => x.Key == "code").Select(x => x.Value).FirstOrDefault() });
                    }
                }

                string itemtype = isFilter ? "Filter Value" : "Value";
                string aml = GetConfigList(listName, itemtype, selects);

                Aras.IOM.Item itemList = AsInnovator.applyAML(aml);
                if (itemList.isError()) throw new Exception(GetLanguageByKeyName("msg_GetLanguageFailed") + $"({itemList.getErrorString()})");
                if (itemList.getItemCount() < 1) throw new Exception(String.Format(GetLanguageByKeyName("msg_GetLanguageFailed")));

                Lists list = new Lists();
                list.Name = listName;
                list.Id = itemList.getItemByIndex(0).getProperty("id", "");
                //itemList.getItemByIndex(0).getRelationships(itemtype).getItemCount
                for (var i=0; i< itemList.getItemByIndex(0).getRelationships(itemtype).getItemCount(); i++)
                {
                    PLMListItem listItem = new PLMListItem();
                    //序號
                    listItem.Order = int.Parse(itemList.getItemByIndex(0).getRelationships(itemtype).getItemByIndex(i).getProperty("sort_order","0"));
                    //標籤
                    listItem.Label = itemList.getItemByIndex(0).getRelationships(itemtype).getItemByIndex(i).getProperty("label","");
                    //值
                    listItem.Value = itemList.getItemByIndex(0).getRelationships(itemtype).getItemByIndex(i).getProperty("value", "");
                    //過濾
                    listItem.Filter = isFilter ? itemList.getItemByIndex(0).getRelationships(itemtype).getItemByIndex(i).getProperty("filter", ""):"";
                    //是否為過濾
                    listItem.IsFilter = isFilter;

                    //標籤其他語系
                    for (var j=0;j< code.Count();j++)
                    {
                        listItem.Labels.Add(code[j], itemList.getItemByIndex(0).getRelationships(itemtype).getItemByIndex(i).getProperty("label" , "", code[j]));
                        
                        //if (listIfem.Label == "") listIfem.Label = itemList.getItemByIndex(0).getRelationships(itemtype).getItemByIndex(i).getProperty("label", "", code[j]);//@@@@@
                        if (String.IsNullOrWhiteSpace(listItem.Label)) listItem.Label = itemList.getItemByIndex(0).getRelationships(itemtype).getItemByIndex(i).getProperty("label", "", code[j]);//@@@@@
                    }
                    if (listItem.Label == "") listItem.Label = listItem.Value;//Modify by kenny 2020/08/13
                    list.ListItems.Add(listItem);
                }
                

                _lists.Add(listName, list);

                return list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得物件屬性值
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="properties"></param>
        protected internal void GetCADProperties(IEnumerable<SearchItem> searchItems, List<PLMProperty> properties, List<PLMPropertyFile> plmPropertyFile)
        {

            try
            {

                if (searchItems.Count() < 1) return;
                
                //List<string> selects = properties.Where(x => x.Name != "").Select(x => x.Name).ToList();
                List<string> selects = properties.Where(x => String.IsNullOrWhiteSpace(x.Name) ==false).Select(x => x.Name).ToList();
                selects.Add("config_id");
                selects.Add("id");

                //List<string> itemConfigIds = searchItems.Where(x => x.ItemConfigId != "").Select(x => x.ItemConfigId).ToList();
                List<string> itemConfigIds = searchItems.Where(x => String.IsNullOrWhiteSpace(x.ItemConfigId) ==false ).Select(x => x.ItemConfigId).ToList();

                string aml = GetConfigCadAmlBuilder(selects, itemConfigIds);
                Aras.IOM.Item items= AsInnovator.applyAML(aml);

                if (items.isError()==false )
                {
                    for (var i = 0;i< items.getItemCount(); i++){
                        string itemConfigId = items.getItemByIndex(i).getProperty("config_id", "");
                        string itemId = items.getItemByIndex(i).getProperty("id", "");
                        
                        //List<SearchItem> plm = searchItems.Where(x => x.FileName != "").Where(x => x.ItemConfigId == itemConfigId).ToList<SearchItem>();
                        List<SearchItem> plm = searchItems.Where(x => String.IsNullOrWhiteSpace(x.FileName) ==false).Where(x => x.ItemConfigId == itemConfigId).ToList<SearchItem>();
                        if (plm.Count() < 1) continue;

                        ObservableCollection<PLMProperty> newProperties = (plm[0].PlmProperties.Count>0)? plm[0].PlmProperties: new ObservableCollection<PLMProperty>();
                        while (newProperties.Count > 0) RemovePropertiesItem(newProperties);


                        foreach (PLMProperty property in properties)
                        {
                            PLMProperty newProperty = property.Clone() as PLMProperty;
                            
                            //if (newProperty.Name != "")
                            if (String.IsNullOrWhiteSpace(newProperty.Name) ==false)
                            {
                                //值
                                string value = items.getItemByIndex(i).getProperty(newProperty.Name, "");
                                newProperty.DataValue = value;
                                //原始值
                                newProperty.Value = value;//OriginalValue
                                // 同步對方的值
                                newProperty.SyncValue = value;
                                //顯示值
                                newProperty.DisplayValue = value;

                                newProperty.SoruceSearchItem = plm[0];
                                
                                //if (newProperty.DataSource != "" && newProperty.ListItem.Count() > 0 && (newProperty.DataType == "list" || newProperty.DataType == "filter list")){
                                if (String.IsNullOrWhiteSpace(newProperty.DataSource) ==false && newProperty.ListItems.Count() > 0 && (newProperty.DataType == "list" || newProperty.DataType == "filter list")){
                                    newProperty.DisplayValue = (newProperty.ListItems.Where(x => x.Value == newProperty.DataValue) != null) ? newProperty.ListItems.Where(x => x.Value == newProperty.DataValue).Select(x => x.Label).FirstOrDefault() : newProperty.DisplayValue;
                                    ResetFilterListItems(newProperties, newProperty);
                                }
                                //else if (newProperty.DataType == "item" && newProperty.Value!="")
                                else if (newProperty.DataType == "item" && String.IsNullOrWhiteSpace(newProperty.Value) ==false)
                                {
                                    newProperty.KeyedName= items.getItemByIndex(i).getPropertyAttribute(newProperty.Name, "keyed_name");
                                    newProperty.KeyedId = newProperty.Value;
                                    newProperty.DisplayValue = newProperty.KeyedName;
                                }
                                else if (newProperty.DataType == "classification" && String.IsNullOrWhiteSpace(newProperty.Value) == false)
                                {
                                    string[] arry = newProperty.Value.Split((char)47);
                                    newProperty.DisplayValue = arry[arry.Length - 1];
                                    newProperty.KeyedId = newProperty.Value;
                                    newProperty.KeyedName = newProperty.DisplayValue;
                                }
                                 newProperty.SyncDisplayValue = newProperty.DisplayValue;
                                newProperty.ResetSyncColorTypeValue();
                                newProperty.IsInitial = false;
                                newProperty.IsExist = true;
                                
                            }
                                
                            newProperties.Add(newProperty);

                            
                        }
                        plm[0].ItemId = items.getItemByIndex(i).getProperty("id", "");
                        plm[0].PlmProperties = newProperties;
                        AddPLMPropertyFiles(plm[0], plmPropertyFile);
                    }
                }
                
                //List<SearchItem> plmList = searchItems.Where(x => x.FileName != "").Where(x => x.PlmProperties.Count==0).ToList<SearchItem>();
                List<SearchItem> plmList = searchItems.Where(x => String.IsNullOrWhiteSpace(x.FileName) ==false).Where(x => x.PlmProperties.Count==0).ToList<SearchItem>();
                foreach (SearchItem searchItem in plmList){
                    ObservableCollection<PLMProperty> newProperties = new ObservableCollection<PLMProperty>();
                    foreach (PLMProperty property in properties)
                    {
                        PLMProperty newProperty = property.Clone() as PLMProperty;
                        newProperty.SoruceSearchItem = searchItem;
                        newProperties.Add(newProperty);
                        
                    }
                    searchItem.PlmProperties = newProperties;
                    AddPLMPropertyFiles(searchItem, plmPropertyFile);
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
        protected internal void LockOrUnlock(IEnumerable<SearchItem> searchItems)
        {
            try
            {

                if (searchItems.Count() < 1) return;

                //List<SearchItem> plm = searchItems.Where(x => x.FileName != "").Where(x => x.IsViewSelected == true).ToList<SearchItem>();
                IEnumerable<SearchItem> plm = searchItems.Where(x => String.IsNullOrWhiteSpace(x.FileName) ==false).Where(x => x.IsViewSelected == true).ToList<SearchItem>();
                if (plm.Count() < 1) return;

                foreach (SearchItem searchItem in plm)
                {
                    
                    //if (searchItem.ItemId != "")
                    if (String.IsNullOrWhiteSpace(searchItem.ItemId) ==false)
                    {
                        Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);
                        if (item == null || searchItem.VersionStatus == SyncVersionStatus.NonLatestVersion.ToString()) continue;

                        ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "LockOrUnlock", "", "Start");
                        if (searchItem.AccessRights == SyncAccessRights.FlaggedByMe.ToString())
                        {
                            item.unlockItem();
                            searchItem.AccessRights = SyncAccessRights.None.ToString();
                        }
                        else if (searchItem.AccessRights == SyncAccessRights.None.ToString())
                        {
                            item.lockItem();
                            searchItem.AccessRights = SyncAccessRights.FlaggedByMe.ToString();
                        }
                        fileMessage.Status = "Finish";
                    }
                }
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
        protected internal bool SynToPLMItemProperties(SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, IsLock isLock)
        {
            try
            {
                Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);
                //if (isLock == 0 && item.getLockStatus() == 1) item.unlockItem();
                if (isLock == IsLock.False && item.getLockStatus() == 1) item.unlockItem();

                item.setAction("edit");
                item.setAttribute("version", "0");
                item.setAttribute("serverEvents", "0");
                
                foreach (PLMProperty itemProperty in searchItem.PlmProperties.Where(x => x.IsSyncPLM == true))
                {
                    SetProperty(item, itemProperty);
                }

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItem, integrationEvents, SyncEvents.OnSyncToPLMItemBefore, SyncType.Properties, ref item);


                //Aras.IOM.Item editItem = item.apply("edit");
                Aras.IOM.Item editItem =  item.apply();

                if (editItem == null) return false;
                if (editItem.isError()==true) throw new Exception(editItem.getErrorString());


                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItem, integrationEvents, SyncEvents.OnSyncToPLMItemAfter, SyncType.Properties, ref editItem);


                //if (isLock == 1 && editItem.getLockStatus() == 0) editItem.lockItem();
                if (isLock == IsLock.True  && editItem.getLockStatus() == 0) editItem.lockItem();
                return true;
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
        /// <param name="plmProperties"></param>
        /// <param name="integrationEvents"></param>
        /// <param name="syncEvent"></param>
        /// <returns></returns>
        protected internal bool SynToPLMItemProperties(SearchItem searchItem, IEnumerable<PLMProperty> plmProperties, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent)
        {
            try
            {
                Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);

                bool isLocked = (item.getLockStatus() == 1) ? true : false;
                if (item.getLockStatus() == 1) item.unlockItem();

                ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Update", "", "Start");

                item.setAction("edit");
                item.setAttribute("version", "0");
                item.setAttribute("serverEvents", "0");

                foreach (PLMProperty itemProperty in plmProperties)
                {
                    SetProperty(item, itemProperty);
                }
                //Modify by kenny 2020/08/20 : bcs_library_path
                UpdateLibraryPath(item, searchItem);


                SyncEvents syncEventBefore = SyncEvents.None;
                SyncEvents syncEventAfter = SyncEvents.None;
                string syncName = "";
                GetExecSyncEvents(syncEvent.ToString(),ref syncEventBefore, ref syncEventAfter, ref syncName);

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, syncName, searchItem, integrationEvents, syncEventBefore, SyncType.Properties, ref item);
            
                Aras.IOM.Item editItem = item.apply();

                if (editItem == null) return false;
                if (editItem.isError()) throw new Exception(editItem.getErrorString());

                fileMessage.Status = "Finish";

                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, syncName, searchItem, integrationEvents, syncEventAfter, SyncType.Properties, ref editItem);
                if (isLocked==true) editItem.lockItem();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected internal bool SynToPLMItemUpdateLibraryPath(SearchItem searchItem)
        {
            try
            {
                //if (((searchItem.IsCommonPart == false && searchItem.IsStandardPart == false) || searchItem.LibraryPath != "" || _partsLibrary.Paths.Count() == 0)) return true;
                if (( searchItem.LibraryPath != "" || _partsLibrary.Paths.Count() == 0)) return true;
                Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);
                item.setAction("edit");
                item.setAttribute("version", "0");
                item.setAttribute("serverEvents", "0");

                if (UpdateLibraryPath(item, searchItem) == false) return true;

                Aras.IOM.Item editItem = item.apply();

                if (editItem == null) return false;
                if (editItem.isError()) throw new Exception(editItem.getErrorString());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected internal void CopyToAddItems(IEnumerable<SearchItem> searchItems, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents)
        {
            try
            {

                Dictionary<string, string> cloneItemIds = new Dictionary<string, string>();
                foreach (SearchItem searchItem in searchItems.Where(x => x.IsViewSelected == true))
                {

                    ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Update", "", "Start");
                    
                    //PLMProperties property = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename" && x.DisplayValue != "").SingleOrDefault();
                    PLMProperty property = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) ==false).SingleOrDefault();
                    if (property == null) continue;
                    string newFileName = property.DisplayValue + Path.GetExtension(searchItem.FileName);
                    fileMessage.Name = newFileName;

                    string itemtype = ItemTypeName.CAD.ToString();
                    Aras.IOM.Item item = AsInnovator.getItemById(itemtype, searchItem.ItemId);
                    Aras.IOM.Item cloneItem = item.clone(false);

                    string itemNumber = cloneItem.getProperty("item_number", "");
                    itemNumber = (itemNumber != "") ? "Copy of " + itemNumber : "";
                    cloneItem.setProperty("item_number", itemNumber);
                    cloneItem.setProperty("bcs_added_filename", newFileName);

                    //Aras.IOM.Item newFileItem = NewFileItem(searchItem.FilePath, newFileName);
                    //cloneItem.setProperty(NativeProperty, newFileItem.getID());
                    cloneItem.setProperty(NativeProperty, "");

                    cloneItemIds.Add(searchItem.ItemConfigId, cloneItem.getID());

                    searchItem.ItemId = cloneItem.getID();
                    searchItem.ItemConfigId = searchItem.ItemId;
                    searchItem.FileId = "";// newFileItem.getID();
                    searchItem.FileName = newFileName;

                    //Events
                    SyncEvents syncEventBefore = SyncEvents.None;
                    SyncEvents syncEventAfter = SyncEvents.None;
                    string syncName = "";
                    GetExecSyncEvents(SyncEvents.OnCopyToAddItemBefore.ToString(), ref syncEventBefore, ref syncEventAfter, ref syncName);
                    _syncCADEvents.ExecCadEvents(AsInnovator, syncName, searchItem, integrationEvents, syncEventBefore, SyncType.CopyToAdd, ref cloneItem);

                    cloneItem = cloneItem.apply();
                    if (cloneItem == null) throw new Exception(string.Format (GetLanguageByKeyName("msg_FailedToCloneObject"), itemtype) );
                    if (cloneItem.isError()) throw new Exception(cloneItem.getErrorString());

                    fileMessage.Status = "Finish";
                    GetExecSyncEvents(SyncEvents.OnCopyToAddItemAfter.ToString(), ref syncEventBefore, ref syncEventAfter, ref syncName);
                    _syncCADEvents.ExecCadEvents(AsInnovator, syncName, searchItem, integrationEvents, syncEventBefore, SyncType.CopyToAdd, ref cloneItem);

                }

                ReplaceCadStructureByCloneItems(cloneItemIds, searchItems);
                foreach (SearchItem searchItem in searchItems.Where(x => x.IsNewVersion == true))
                {

                    Aras.IOM.Item  item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);
                    if (searchItem.IsViewSelected==false)
                    {
                        item.setAction("version");
                        Aras.IOM.Item newVersion = item.apply();
                        searchItem.ItemId = newVersion.getID();
                        item = newVersion;
                    }
                    UpdateCADStructure(searchItem, item);
                }
                    

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 上傳圖檔
        /// </summary>
        /// <param name="searchItems"></param>
        protected internal void UploadFiles(IEnumerable<SearchItem> searchItems)
        {
            try
            {

                Dictionary<string, string> cloneItemIds = new Dictionary<string, string>();
                foreach (SearchItem searchItem in searchItems.Where(x => x.IsViewSelected == true || x.IsNewVersion == true))
                {
                    string itemtype = ItemTypeName.CAD.ToString();
                    Aras.IOM.Item item = AsInnovator.getItemById(itemtype, searchItem.ItemId);
                    item.setAction("edit");
                    item.setAttribute("version", "0");
                    item.setAttribute("serverEvents", "0");

                    Aras.IOM.Item newFileItem = NewFileItem(searchItem.FilePath, searchItem.FileName);
                    item.setProperty(NativeProperty, newFileItem.getID());

                    item = item.apply();
                    if (item == null) throw new Exception(String.Format (GetLanguageByKeyName("msg_FailedToEditObject"), itemtype));
                    if (item.isError()) throw new Exception(item.getErrorString());

                    searchItem.FileId = newFileItem.getID();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 鎖定CAD物件
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="integrationEvents"></param>
        /// <returns></returns>
        protected internal bool LockItem(SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents,IsLock isLock)
        {
            try
            {
                Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);
                if (item.getLockStatus() == 1) return true ;

                //Events
                SyncEvents syncEvents = (isLock == IsLock.True) ? SyncEvents.OnLockItemBefore : SyncEvents.OnUnlockItemBefore;
                SyncType syncType = (isLock == IsLock.True) ? SyncType.Locked : SyncType.Unlocked;
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItem, integrationEvents, syncEvents, syncType, ref item);

                //(isLock == IsLock.True) ? item.lockItem() : item.unlockItem(); 
                if (isLock == IsLock.True){item.lockItem();}else{item.unlockItem();}

                //Events
                syncEvents = (isLock == IsLock.True) ? SyncEvents.OnLockItemAfter : SyncEvents.OnUnlockItemAfter;
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItem, integrationEvents, syncEvents, syncType, ref item);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 同步到PLM,新增CAD物件
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="isLock"></param>
        /// <returns></returns>
        protected internal bool SynToPLMNewItem(SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent, List<PLMKey> classItemKeys, string filename, IsLock isLock)
        {
            try
            {
                string syncName = "";
                SyncEvents syncEventBefore = SyncEvents.None;
                SyncEvents syncEventAfter = SyncEvents.None;

                ItemMessage fileMessage = ClsSynchronizer.VmMessages.AddItemMessage(searchItem.FileName, "Add", "", "Start");
                string itemtype = ItemTypeName.CAD.ToString();
                Aras.IOM.Item item = AsInnovator.newItem(itemtype, "add");

                item.setProperty("bcs_added_filename", filename);

                foreach (var itemProperty in searchItem.PlmProperties.Where(x => x.IsSyncPLM == true || x.IsRequired==true))
                {
                    SetProperty(item, itemProperty);
                }

                foreach (PLMKey keyProperty in classItemKeys.Where(x => x.Name !=""))
                {
                    item.setProperty(keyProperty.Name, keyProperty.Value);
                }

                //Modify by kenny 2020/08/20 : bcs_library_path
                UpdateLibraryPath(item,searchItem);

                GetExecSyncEvents(syncEvent.ToString(), ref syncEventBefore, ref syncEventAfter, ref syncName);

                //Events
                if (syncEventBefore != SyncEvents.None) _syncCADEvents.ExecCadEvents(AsInnovator, syncName, searchItem, integrationEvents, syncEventBefore, SyncType.Add, ref item);

                Aras.IOM.Item newItem = item.apply();
                if (newItem == null) throw new Exception(String.Format(GetLanguageByKeyName("msg_FailedToAddObject"), itemtype)); 
                if (newItem.isError()) throw new Exception(newItem.getErrorString());

                fileMessage.Status = "Finish";
                //if (isLock == 1 && newItem.getLockStatus() == 0) newItem.lockItem();
                if (isLock == IsLock.True && newItem.getLockStatus() == 0) newItem.lockItem();
                searchItem.ItemId = newItem.getID();
                searchItem.ItemConfigId = newItem.getProperty("config_id","");

                //同步CAD範本圖檔屬性
                IntegrationEvents integrationEvent = null;
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.AddTemplate.ToString(), ref searchItem, integrationEvent, SyncEvents.OnAddTemplate, SyncType.NewFormTemplateFile, ref item);
                //searchItem.plmProperties.Where(x => x.name == "").Select(x => x.value);
                //Events
                if (syncEventBefore != SyncEvents.None) _syncCADEvents.ExecCadEvents(AsInnovator,syncName, searchItem, integrationEvents, syncEventAfter, SyncType.Add, ref item);
                
                return true;
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
        protected internal bool SynToPLMItemFiles(ref Aras.IOM.Item item,SearchItem searchItem)
        {
            try
            {
                item.setAction("edit");
                item.setAttribute("version", "0");
                item.setAttribute("serverEvents", "0");
                
                //foreach (PLMPropertyFile itempropertyFile in searchItem.PropertyFile.Where(x => x.FileName !=""|| x.FilePath != ""))
                foreach (PLMPropertyFile itempropertyFile in searchItem.PropertyFile.Where(x => String.IsNullOrWhiteSpace(x.FileName) ==false|| String.IsNullOrWhiteSpace(x.FilePath) ==false))
                {
                    //if (IsCanNewFile(searchItem, itempropertyFile)){ 
                    if (File.Exists(Path.Combine(itempropertyFile.FilePath, itempropertyFile.FileName))){

                        Aras.IOM.Item newFileItem = NewFileItem(itempropertyFile.FilePath, itempropertyFile.FileName);
                        string value = (itempropertyFile.DataType == "image") ? @"vault:///?fileId=" + newFileItem.getID() : newFileItem.getID();
                        item.setProperty(itempropertyFile.Name, value);
                        if (itempropertyFile.FunctionName == "native_property") searchItem.FileId = newFileItem.getID();
                    }
                }

                Aras.IOM.Item editItem = item.apply();
                if (editItem == null) return false;
                if (editItem.isError()) throw new Exception(editItem.getErrorString());
                item = editItem;
                return true;
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
        protected internal bool SynToPLMItemStructure(SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, IsLock isLock, IsNewGeneration isNewGeneration)
        {
            try
            {

                Aras.IOM.Item item = AsInnovator.getItemById(ItemTypeName.CAD.ToString(), searchItem.ItemId);
                item = (item.getProperty("is_current","0")=="0")? GetLastItem(item) : item;
                if (isLock == IsLock.False  && item.getLockStatus() == 1) item.unlockItem();
                int intLockStatus = item.getLockStatus();

                //Events (產生native_property,thumbnail_property縮圖等其他圖檔)
                //Event
                IntegrationEvents integrationEvent = null;
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.ExportPropertyFile.ToString(), ref searchItem, integrationEvent, SyncEvents.OnAddTemplate, SyncType.NewFormTemplateFile, ref item);

                //if (isNewGeneration == true)
                if (isNewGeneration == IsNewGeneration.True)
                {
                    item.setAction("version");
                    Aras.IOM.Item newVersion = item.apply();
                    searchItem.ItemId = newVersion.getID();
                    item = newVersion;
                    searchItem.IsNewVersion = true;
                }

                //檔案同步
                SynToPLMItemFiles(ref item,searchItem);

                //Events
                 _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItem, integrationEvents, SyncEvents.OnSyncToPLMItemStructureBefore, SyncType.Structure, ref item);

                if (searchItem.ClassName == "Drawing" && CADSoftware !="AutoCAD")
                    Update2DDrawingCADStructure(searchItem, item);
                else
                    UpdateCADStructure(searchItem, item);
                
                //Events
                _syncCADEvents.ExecCadEvents(AsInnovator, SyncCadCommands.SystemSyncToPLMProperties.ToString(), searchItem, integrationEvents, SyncEvents.OnSyncToPLMItemStructureAfter, SyncType.Structure, ref item);

                //@@@@@尚未開發 : 有設定關聯檔案(不同relationship),同時需要一併上傳,並建立關係(2D圖檔或其他文件)
                //@@@@@尚未開發 : Family Tables的資料結構更新 

                if (isLock == IsLock.False  && item.getLockStatus() == 1) item.unlockItem();
                if (isLock == IsLock.True  && item.getLockStatus() == 0) item.lockItem();
                if (isLock==IsLock.None && intLockStatus != item.getLockStatus())
                {
                    if (intLockStatus == 1) { item.lockItem(); } else { item.getLockStatus(); }
                }
                _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.UpdateCADsProperties.ToString(), ref searchItem, null, null, SyncEvents.OnUpdateCADsProperties, SyncType.Structure);
                if (searchItem.IsActiveCAD==false ) _syncCADEvents.ExecCadEvent(AsInnovator, SyncCadCommands.CloseFiles.ToString(), ref searchItem, null, integrationEvent, SyncEvents.CloseFiles, SyncType.Structure);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 2D建立關聯
        /// </summary>
        /// <param name="searchItems"></param>
        /// <param name="searchItem"></param>
        protected internal void Get2DDrawingCADSourceItem(IEnumerable<SearchItem> searchItems, SearchItem searchItem)
        {
            try
            {
                
                //if (searchItem.ItemId == "") return;
                if (String.IsNullOrWhiteSpace(searchItem.ItemId)) return;
                string itemType = Regex.Replace(SyncLinkItemTypes.BCS_CAD_Drawing.ToString(), "_", " ");

                Aras.IOM.Item qureyResult = GetPLMSearchItems(" a.id,a.related_id,a.source_id,c.filename,b.bcs_added_filename,b.config_id,b.id as source_item_id  from [innovator].[" + SyncLinkItemTypes.BCS_CAD_Drawing.ToString().ToUpper() + "] as a left join [innovator].[CAD] b on a.source_id=b.id left join [innovator].[File] as c on b.{0}=c.id where a.related_id='{1}' and b.is_current=1{2} ", NativeProperty, searchItem.ItemId, "");

                for (var i = 0; i < qureyResult.getItemCount(); i++)
                {
                    string fileName = (qureyResult.getItemByIndex(i).getProperty("filename", "") != "") ? qureyResult.getItemByIndex(i).getProperty("filename", "") : qureyResult.getItemByIndex(i).getProperty("bcs_added_filename", "");
                    if (fileName != "")
                    {

                        CADStructure cadStructure = searchItem.CadStructure.Where(x => x.ItemConfigId == qureyResult.getItemByIndex(i).getProperty("config_id", "")).FirstOrDefault();
                        SearchItem sonSearchItem = searchItems.Where(x=>x.ItemConfigId == qureyResult.getItemByIndex(i).getProperty("config_id", "") || x.FileName.ToLower() == fileName.ToLower()).FirstOrDefault();
                        if (cadStructure == null && sonSearchItem !=null)
                        {

                            //if (sonSearchItem.ItemConfigId == "")
                            if (String.IsNullOrWhiteSpace(sonSearchItem.ItemConfigId))
                            {
                                sonSearchItem.ItemConfigId = qureyResult.getItemByIndex(i).getProperty("config_id", "");
                                sonSearchItem.ItemId = qureyResult.getItemByIndex(i).getProperty("source_item_id", "");
                            }
                            CADStructure newCADStructure = new CADStructure();
                            newCADStructure.Order = 10;
                            newCADStructure.ItemConfigId = sonSearchItem.ItemConfigId;
                            newCADStructure.Child = sonSearchItem;
                            newCADStructure.ItemType = itemType;
                            newCADStructure.InstanceId = "";
                            searchItem.CadStructure.Add(newCADStructure);
                        }
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得最新(後)一版本物件
        /// </summary>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        protected internal Aras.IOM.Item GetLastItem(Aras.IOM.Item currentItem)
        {
            try
            {
                Aras.IOM.Item lastItem = GetLastItem(currentItem.getType(), currentItem.getProperty("config_id", ""));
                return lastItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 取得最新(後)一版本物件
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemConfigId"></param>
        /// <returns></returns>
        internal Aras.IOM.Item GetLastItem(string itemType,string itemConfigId)
        {
            try
            {
                Aras.IOM.Item item = AsInnovator.newItem(itemType, "get");
                item.setProperty("config_id", itemConfigId);
                item.setProperty("is_current", "1");
                Aras.IOM.Item lastItem = item.apply();

                if (lastItem == null) return null;
                if (lastItem.isError()) throw new Exception(lastItem.getErrorString());
                return lastItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得CAD的必填屬性
        /// </summary>
        protected internal XElement GetCADIsRequiredProperties()
        {
            try
            {

                if (_cadRequiredProperties != null) return _cadRequiredProperties;
                //name,id,DataType,data_source,is_required,label
                List<string> selects = new List<string> { "name", "id", "data_type", "data_source","pattern", "is_required", "label" };

                foreach (var lagnlist in _language)
                {
                    //string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).ElementAt(0);
                    string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).FirstOrDefault();
                    if (suffix != "") selects.AddRange(new List<string> { "label" + suffix });
                }
                string aml = GetConfigCADRequiredProperties(selects);

                Aras.IOM.Item itemRequiredProperties = AsInnovator.applyAML(aml);
                if (itemRequiredProperties.isError()) return null;

                //XmlDocument xmlDocument = new XmlDocument();
                string xml = itemRequiredProperties.ToString();
                //xmlDocument.LoadXml(xml);
                //XDocument xmlDoc = ToXDocument(xmlDocument);
                XDocument xmlDoc = XDocument.Parse(xml);
                _cadRequiredProperties = xmlDoc.Descendants("Result").Single();
                return _cadRequiredProperties;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// 取得查詢類別的物件特殊屬性
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected internal XElement GetSearchItemTypeProperties(string itemTypeName)
        {
            try
            {

                //if (_cadRequiredProperties != null) return null;


                List<string> selects = new List<string> { "name", "id", "data_type", "data_source","pattern", "is_required", "label" ,"keyed_name" ,"related_id","sort_order", "column_width" };
                foreach (var lagnlist in _language)
                {
                    //string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).ElementAt(0);
                    string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).FirstOrDefault();
                    if (suffix != "") selects.AddRange(new List<string> { "label" + suffix });
                }

                
                string aml = GetConfigSearchItemTypeProperties(itemTypeName, selects);

                Aras.IOM.Item itemTypeProperties = AsInnovator.applyAML(aml);
                if (itemTypeProperties.isError()) return null;

                //XmlDocument xmlDocument = new XmlDocument();
                string xml = itemTypeProperties.ToString();

                //xmlDocument.LoadXml(xml);
                //XDocument xmlDoc = ToXDocument(xmlDocument);
                XDocument xmlDoc = XDocument.Parse(xml);
                _cadRequiredProperties = xmlDoc.Descendants("Result").Single();
                return _cadRequiredProperties;

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
        protected internal Lists GetDatabases(string url)
        {
            try
            { 
                Lists list = new Lists();
                
                //Aras Innovator API
                HttpServerConnection httpServerConnection = IomFactory.CreateHttpServerConnection(url);
                if (httpServerConnection != null)
                {
                    string[] database = httpServerConnection.GetDatabases();
                    int i = 0;
                    foreach (string dbName in database)
                    {
                        PLMListItem listIfem = new PLMListItem();
                        //序號
                        listIfem.Order = i;
                        //標籤
                        listIfem.Label = dbName;
                        //值
                        listIfem.Value = dbName;
                        //過濾
                        listIfem.Filter =  "";
                        //是否為過濾
                        listIfem.IsFilter = false ;
                        //標籤其他語系
                        list.ListItems.Add(listIfem);
                        i++;
                    }
                    if (database.Count() > 0) return list;
                }


                //if (url.Substring(url.Length - 1) != @"/") url += @"/";
                //url = url + @"Server/ServerConfigurationSettings.aspx?name=support";
                //string value = GetWebContent(url);
                //XDocument xDocument= XDocument.Parse(value);
                
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 有問題,暫不使用
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected internal string GetWebContent(string url)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;

            //要注意的這是這個編碼方式，還有內容的Xml內容的編碼方式
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            byte[] data = encoding.GetBytes(url);

            //準備請求，設置參數
            request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "text/xml";// "text/xml";"application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;

            outstream = request.GetRequestStream();
            outstream.Write(data, 0, data.Length);
            outstream.Flush();
            outstream.Close();
            //發送請求並獲取相應響應數據

            response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才開始向目標網頁發送Post請求
            instream = response.GetResponseStream();

            sr = new StreamReader(instream, encoding);
            //返回結果網頁(html)代碼

            string content = sr.ReadToEnd();
            return content;
        }



        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected internal bool DownloadFile(string fileId, String filePath, ref String fileName)
        {
            try
            {
                Aras.IOM.Item fileItem = AsInnovator.getItemById(ItemTypeName.File.ToString(), fileId);
                if (fileItem == null) return false;
                if (fileItem.isError()) return false;
                if (fileName == "") fileName = fileItem.getProperty("filename", "");
                return DownloadFile(null, null, SyncEvents.None, null, fileItem, filePath, fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected internal bool DownloadFile(SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent, Aras.IOM.Item item ,string fileId, String filePath, String fileName)
        {
            try
            {
                //當為根圖及解鎖時,才能下載共用圖檔
                searchItem.FilePath = filePath;
                if (UpdateLibraryPath(searchItem)) return true;
                filePath = searchItem.FilePath;//path有可能變為共用路徑

                Aras.IOM.Item fileItem = AsInnovator.getItemById(ItemTypeName.File.ToString(), fileId);
                if (fileItem == null) return false;
                if (fileItem.isError()) return false;
                
                return DownloadFile(searchItem, integrationEvents, syncEvent, item, fileItem, filePath, fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="fileItem"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected internal bool DownloadFile(SearchItem searchItem, List<BCS.CADs.Synchronization.Classes.IntegrationEvents> integrationEvents, SyncEvents syncEvent, Aras.IOM.Item item ,Aras.IOM.Item fileItem, String filePath, String fileName)
        {
            try
            {

                //SyncEvents.OnLoadFromPLMDownloadBefore
                SyncEvents syncEventBefore = SyncEvents.None;
                SyncEvents syncEventAfter = SyncEvents.None;
                string syncName = "";
                GetExecSyncEvents(syncEvent.ToString(), ref syncEventBefore, ref syncEventAfter, ref syncName);

                //Events
                if (syncEventBefore != SyncEvents.None) _syncCADEvents.ExecCadEvents(AsInnovator, syncName, searchItem, integrationEvents, syncEventBefore, SyncType.Download, ref item);

                //if (@filePath.Substring(filePath.Length - 1)!= @"\") @filePath+= @"\";
                AsInnovator.getConnection().DownloadFile(fileItem, Path.Combine(filePath, fileName), true);

                if (syncEventAfter != SyncEvents.None) _syncCADEvents.ExecCadEvents(AsInnovator, syncName, searchItem, integrationEvents, syncEventAfter, SyncType.Download, ref item);
                
                //return (File.Exists(Path.Combine(filePath, fileName))) ? true : false;
                return File.Exists(Path.Combine(filePath, fileName));


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 是否是共用件及標準件的路徑及圖檔
        /// </summary>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        private bool UpdateLibraryPath(SearchItem searchItem)
        {
            try
            {
                if (((searchItem.IsCommonPart == true || searchItem.IsStandardPart == true) &&  _partsLibrary.Paths.Count() > 0) == false) return false;
                //當為根圖及解鎖時,才能下載共用圖檔
                if (searchItem.IsRoot == true && searchItem.IsCurrent && searchItem.AccessRights == SyncAccessRights.FlaggedByMe.ToString()) return false;

                if (String.IsNullOrWhiteSpace(searchItem.LibraryPath))
                {
                    //檢查圖檔是否在共用區位置
                    return FileInPartsLibrary(searchItem);
                }else
                    //表示為共用件及標準件,同時searchItem.LibraryPath為空值               
                    if (Directory.Exists(searchItem.LibraryPath) == false) return false; //表示沒有權限,下載本機

                searchItem.FilePath = searchItem.LibraryPath;
                return File.Exists(Path.Combine(searchItem.LibraryPath, searchItem.FileName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 符合共用件及標準件的路徑,更新bcs_library_path屬性值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="searchItem"></param>
        private bool UpdateLibraryPath(Aras.IOM.Item item, SearchItem searchItem)
        {
            try
            {
                //if (((searchItem.IsCommonPart == true || searchItem.IsStandardPart == true) && _partsLibrary.Paths.Count() > 0) == false) return ;
                //if (((searchItem.IsCommonPart == false && searchItem.IsStandardPart == false) || _partsLibrary.Paths.Count() == 0)) return ;
                bool isUpdate = false;
                if (_partsLibrary.Paths.Count() == 0) return false;

                //目前指定共用區設定
                string path = Path.GetDirectoryName(Path.Combine(searchItem.FilePath, searchItem.FileName));
                string libPath = _partsLibrary.Paths.Where(x => x.Path.ToLower() == path.ToLower()).Select(x => x.Path).FirstOrDefault();
                if (String.IsNullOrWhiteSpace(libPath) == false)
                {
                    if (File.Exists(Path.Combine(libPath, searchItem.FileName)))
                    { item.setProperty("bcs_library_path", libPath); isUpdate = true; }
                }
                
                if (isUpdate == false) {
                    //所有的共用區設定
                    if (FileInPartsLibrary(searchItem))
                    { item.setProperty("bcs_library_path", searchItem.LibraryPath); isUpdate = true; }
                }

                if (searchItem.IsCommonPart == false && searchItem.IsStandardPart == false && item.getProperty("bcs_library_path", "")!="")
                {
                    //符合設定的共用區
                    searchItem.IsCommonPart = true;
                    item.setProperty("bcs_is_common_part", "1");
                    isUpdate = true;
                }

                return isUpdate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查圖檔是否在共用區位置
        /// </summary>
        /// <param name="searchItem"></param>
        /// <returns></returns>
        private bool FileInPartsLibrary(SearchItem searchItem)
        {
            try
            {
                // foreach (string path in _partsLibrary.Paths.Select(x => x.Value))
                foreach (string path in _partsLibrary.Paths.Select(x => x.Path ))
                {
                    if (File.Exists(Path.Combine(path, searchItem.FileName)))
                    {
                        searchItem.LibraryPath = path;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //LibraryPath
        protected internal void UpdateLibraryPathFiles(LibraryPath libraryPath)
        {
            try
            {
                if (libraryPath.FileItems.Count() == 0) return;
                string path = $@"{libraryPath.Path}";
                string sqlCommand = $" a.id,a.{ThumbnailProperty},b.filename,b.id as fileId{"{0}"} from [innovator].[CAD] as a left join [innovator].[FILE] as b{"{1}"} on a.{NativeProperty}=b.id where a.is_current=1 and a.bcs_library_path=N'{path}' and not a.{ThumbnailProperty}='' and b.filename in {"{2}"} ";
                
                string files = "(N'" + String.Join("',N'", libraryPath.FileItems.Select(x => x.Name).ToArray()) + "')";
                Aras.IOM.Item qureyResult = GetPLMSearchItems(sqlCommand, "", "", files);
                for (var i = 0; i < qureyResult.getItemCount(); i++)
                {
                    string fileName = qureyResult.getItemByIndex(i).getProperty("filename", "");
                    LibraryFileInfo fileInfo= libraryPath.FileItems.Where(x => x.Name.ToLower() == fileName.ToLower()).FirstOrDefault();
                    if (fileInfo == null) continue;
                    fileInfo.ItemId = qureyResult.getItemByIndex(i).getProperty("id", "");
                    fileInfo.Thumbnail = qureyResult.getItemByIndex(i).getProperty(ThumbnailProperty, "");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 是否允許新增File
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="itempropertyFile"></param>
        /// <returns></returns>
        private bool IsCanNewFile(SearchItem searchItem, PLMPropertyFile itempropertyFile)
        {
            try
            {
                if (File.Exists(Path.Combine(itempropertyFile.FilePath, itempropertyFile.FileName)) == false) return false;
                if (itempropertyFile.FunctionName != "native_property") return true;
                if (searchItem.IsRoot) return true;
                if (((searchItem.IsCommonPart == true || searchItem.IsStandardPart == true) && _partsLibrary.Paths.Count() > 0) == false) return true;
                string path = Path.GetDirectoryName(Path.Combine(itempropertyFile.FilePath, itempropertyFile.FileName));
                //string libPath = _partsLibrary.Paths.Where(x => x.Value == path).Select(x => x.Value).FirstOrDefault();
                string libPath = _partsLibrary.Paths.Where(x => x.Path.ToLower() == path.ToLower()).Select(x => x.Path).FirstOrDefault();
                return String.IsNullOrWhiteSpace(libPath)? true:false;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


      


        /// <summary>
        /// 取得正在運行Process相關的Id
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        protected internal List<string> GetProcessIds(string processName)
        {
            try
            {
                List<string> processIds = Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(processName.ToLower())).Select(x => x.Id.ToString ()).ToList();
                return processIds;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 依查詢條件,取得Item Type的相關物件
        /// </summary>
        /// <param name="searchItemType"></param>
        /// <param name="nativeProperty"></param>
        /// <returns></returns>
        protected internal Aras.IOM.Item GetItemTypeSearch(SearchItem searchItemType,string nativeProperty)
        {
            try
            {
                bool isCondition = false;
                //string where = "";
                StringBuilder where = new StringBuilder();

                //Modify by kenny 2020/08/05
                //foreach (PLMProperty property in searchItemType.PlmProperties.Where(x=>x.DataValue!="" && x.DataValue != null))
                foreach (PLMProperty property in searchItemType.PlmProperties.Where(x=>String.IsNullOrWhiteSpace(x.DataValue)==false && x.DataType!= "revision"))
                {
                    string value = property.DataValue;

                    //if (value.Substring(0, 1) == "*") value = "%" + value.Substring(1);
                    //if (value.Substring(property.DataValue.Length-1) == "*") value = value.Substring(0, value.Length-1)+ "%";
                    //string condition = (value.Substring(0, 1) == "%" || value.Substring(property.DataValue.Length - 1) == "%")? " like ":"=";
                    value = value.Replace("*", "%");
                    string condition = (value.IndexOf("%") <0) ? "=" : " like " ;

                    if (where.ToString() != "") where.Append(" and ");
                    string name = "a." + property.Name;
                    if (property.DataType.Contains("date"))
                    {
                        DateTime date = DateTime.Parse(value);
                        if (date.Year > 1911)
                        {
                            name = "CONVERT(VARCHAR, a." + property.Name + ", 120)";
                            condition = " like ";
                            value = "%" + date.ToString("yyyy-MM-dd") + "%";
                        }
                    }
                    //else if (property.DataType.Contains("item") && property.DataSource != "")
                    else if (property.DataType.Contains("item") && String.IsNullOrWhiteSpace(property.DataSource) ==false)
                    {

                        
                        //if (property.KeyedId != "")
                        if (String.IsNullOrWhiteSpace(property.KeyedId) ==false)
                        {
                            value = property.KeyedId;
                        }
                        else
                        {
                            Aras.IOM.Item item = AsInnovator.getItemByKeyedName(property.DataSource, value);
                            value = (item != null) ? item.getID() : value;
                        }
                        
                    }
                    //where.Append("a." + name);
                    where.Append(name);
                    where.Append(condition);
                    where.Append("'");
                    where.Append(value);
                    where.Append("'");
                    isCondition = true;
                }

                if (isCondition == true )
                    where.Append(" and a.is_current=1");
                else
                    where.Append(" a.is_current=1");

                //List<string> select = searchItemType.PlmProperties.Where(x => x.Name != "").Select(x => x.Name).ToList<string>();
                //Modify by kenny 2020/08/05
                //List<string> select = searchItemType.PlmProperties.Where(x => String.IsNullOrWhiteSpace(x.Name) ==false).Select(x => x.Name).ToList<string>();
                List<string> select = searchItemType.PlmProperties.Where(x => String.IsNullOrWhiteSpace(x.Name) ==false).Where(x=>x.DataType != "revision").Select(x => x.Name).ToList<string>();
                select.Add("id");
                select.Add("config_id");
                select.Add("keyed_name");
                //Modif by kenny 2020/08/05 ----------
                select.Add("major_rev");
                select.Add("is_current");
                //------------------------------------
                select.Add("generation");

                //Modify by kenny 2020/08/20 bcs_library_path
                if (searchItemType.ItemType == ItemTypeName.CAD.ToString())//Modify by kenny 2020/08/26
                    AddCADSelectDefaultProperties(searchItemType.PlmProperties, select);


                if (nativeProperty != "")
                {
                    select.Add(nativeProperty);
                    where.Append(" and not ");
                    where.Append("a." + nativeProperty);
                    where.Append("=''");
                }

                string selectValue = "a." + String.Join(",a.", select);

                StringBuilder sbSelects = new StringBuilder();
                StringBuilder sbLeftJoin = new StringBuilder();

                int i = 0;
                
                //foreach (PLMProperties property in searchItemType.PlmProperties.Where(x => x.Name != "" && x.DataType=="item" && x.DataSource!=""))
                foreach (PLMProperty property in searchItemType.PlmProperties.Where(x => String.IsNullOrWhiteSpace(x.Name) ==false&& x.DataType=="item" && x.DataSource!=""))
                {
                    sbSelects.Append(",");
                    sbSelects.Append ($"a{i}.keyed_name as {property.Name}_keyed_name");
                    sbLeftJoin.Append($"left join [innovator].[{property.DataSource.ToUpper().Replace(" ", "_")}] as a{i} on a.{property.Name}=a{i}.id ");
                    i++;
                }

                selectValue += sbSelects.ToString();
                ////string sqlCommand = " TOP 100 {0}{1} from [innovator].[" + searchItemType.ClassName + "] where {2}";
                //string sqlCommand = " TOP 500 {0} from [innovator].[" + searchItemType.ClassName + "] as a {1} where {2}";
                string sqlCommand = " TOP 500 {0} from [innovator].[" + searchItemType.ItemType + "] as a {1} where {2}"; //Modify by kenny 2020/08/13

                Aras.IOM.Item qureyResult = GetPLMSearchItems(sqlCommand, selectValue, sbLeftJoin.ToString(), where.ToString());
                int count = qureyResult.getItemCount();


                return qureyResult;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
        }

        protected internal void AddCADSelectDefaultProperties(IEnumerable<PLMProperty> PlmProperties, List<string> select)
        {

            try
            {

                List<string> properties = new List<string> { "bcs_library_path", "is_standard", "bcs_is_common_part" };
                foreach (string name in properties)
                {

                    if (PlmProperties.Where(x => x.Name == name).FirstOrDefault() == null)
                        select.Add(name);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增預設查詢屬性(select條件)
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="select"></param>
        protected internal void AddCADSelectDefaultProperties(IEnumerable<ConditionalRule> rule, List<string> select)
        {

            try
            {

                List<string> properties = new List<string> { "bcs_library_path", "is_standard", "bcs_is_common_part" };
                foreach (string name in properties)
                {

                    if (rule.Where(x => x.PrepertyName == name).FirstOrDefault() == null)
                        select.Add(name);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     
        /// <summary>
        /// 取得物件所有版本
        /// </summary>
        /// <param name="searchItemType"></param>
        /// <param name="nativeProperty"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected internal Aras.IOM.Item GetAllRevisions(ItemType searchItemType, string nativeProperty,string itemId)
        {
            try
            {

                Aras.IOM.Item item = AsInnovator.getItemById(searchItemType.Name , itemId);

                List<string> select = searchItemType.PlmProperties.Where(x => String.IsNullOrWhiteSpace(x.Name) == false).Where(x => x.DataType != "revision").Select(x => x.Name).ToList<string>();
                select.Add("id");
                select.Add("config_id");
                select.Add("keyed_name");
                select.Add("major_rev");
                select.Add("generation");

                string selectValue = "a." + String.Join(",a.", select);

                StringBuilder sbSelects = new StringBuilder();
                StringBuilder sbLeftJoin = new StringBuilder();

                int i = 0;

                foreach (PLMProperty property in searchItemType.PlmProperties.Where(x => String.IsNullOrWhiteSpace(x.Name) == false && x.DataType == "item" && x.DataSource != ""))
                {
                    sbSelects.Append(",");
                    sbSelects.Append($"a{i}.keyed_name as {property.Name}_keyed_name");
                    sbLeftJoin.Append($"left join [innovator].[{property.DataSource.ToUpper().Replace(" ", "_")}] as a{i} on a.{property.Name}=a{i}.id ");
                    i++;
                }

                selectValue += sbSelects.ToString();
                string sqlCommand = " TOP 500 {0} from [innovator].[" + searchItemType.Name  + "] as a {1} where a.config_id='{2}' order by a.generation asc";

                Aras.IOM.Item qureyResult = GetPLMSearchItems(sqlCommand, selectValue, sbLeftJoin.ToString(), item.getProperty("config_id", ""));
                int count = qureyResult.getItemCount();


                return qureyResult;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 依據Pattern的屬性值,作為過濾的條件值
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="plmProperty"></param>
        protected internal void ResetFilterListItems(ObservableCollection<PLMProperty> properties, PLMProperty plmProperty)
        {

            try
            {

                
                //if (plmProperty.DataType != "filter list" || plmProperty.Pattern == "") return;
                if (plmProperty.DataType != "filter list" || String.IsNullOrWhiteSpace(plmProperty.Pattern)) return;

                //Lists lists = _lists.Where(x => x.Key == plmProperty.DataSource)?.FirstOrDefault().Value;

                PLMProperty sProperty = properties.Where(x => x.Name == plmProperty.Pattern).FirstOrDefault();
                if (sProperty == null) return;

                //List<PLMListItem> filterListItems = lists.ListItem.Where(x => x.Filter == sProperty.DataValue).ToList<PLMListItem>();
                List<PLMListItem> filterListItems = plmProperty.PLMList.ListItems.Where(x => x.Filter == sProperty.DataValue).ToList<PLMListItem>();
                plmProperty.ListItems = new ObservableCollection<PLMListItem>(filterListItems);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 加入產出檔案列示定義
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="plmPropertyFile"></param>
        protected internal void AddPLMPropertyFiles(SearchItem searchItem, List<PLMPropertyFile> plmPropertyFile)
        {
            try
            {

                List<PLMPropertyFile> newPLMPropertyFile = new List<PLMPropertyFile>();
                foreach (PLMPropertyFile propertyFile in plmPropertyFile)
                {
                    PLMPropertyFile newPropertyFile = propertyFile.Clone() as PLMPropertyFile;
                    newPLMPropertyFile.Add(newPropertyFile);
                }
                searchItem.PropertyFile = newPLMPropertyFile;


            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        /// <summary>
        /// 取某一版本
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="configId"></param>
        /// <param name="majorRev"></param>
        /// <param name="generation"></param>
        /// <returns></returns>
        protected internal Aras.IOM.Item GetRevisionItem(string itemType, string configId, string majorRev, string generation)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                stringBuilder.Append($"<Item type = '{itemType}' action = 'get' >");
                stringBuilder.Append($"<config_id>{configId}</config_id>");
                stringBuilder.Append($"<major_rev>{majorRev}</major_rev>");
                stringBuilder.Append($"<generation>{generation}</generation>");
                stringBuilder.Append("</Item>");
                stringBuilder.Append("</AML>");
                string aml = stringBuilder.ToString();
                return AsInnovator.applyAML(aml);
            }
            catch (Exception ex)
            {
                //string strError = ex.Message;
                throw ex;
            }
        }

        /// <summary>
        /// 取得Classification
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected internal XDocument GetClassification(string itemType)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                stringBuilder.Append($"<Item type = 'ItemType' action = 'get' >");
                stringBuilder.Append($"<name>{itemType}</name>");
                stringBuilder.Append("</Item>");
                stringBuilder.Append("</AML>");
                string aml = stringBuilder.ToString();
                Aras.IOM.Item item = AsInnovator.applyAML(aml);

                string xml = item.getProperty("class_structure","");

                //XmlDocument xmlDocument = new XmlDocument();
                //xmlDocument.LoadXml(xml);
                //xmlDocument.Save(@"D:\Temp\001\20200831.xml");

                XDocument xmlDoc =(xml!="")? XDocument.Parse(xml):null;
                return xmlDoc;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion

        #region "                   方法(內部)"

        /// <summary>
        /// Clone物件,取代CAD結構
        /// </summary>
        /// <param name="cloneItemIds"></param>
        /// <param name="searchItems"></param>
        private void ReplaceCadStructureByCloneItems(Dictionary<string, string> cloneItemIds, IEnumerable<SearchItem> searchItems)
        {
            try
            {
                Dictionary<string, string> cloneItemId = new Dictionary<string, string>();
                foreach (SearchItem searchItem in searchItems.Where(x => x.CadStructure.Count() > 0))
                {
                    foreach (var cloneItem in cloneItemIds)
                    {
                        ReplaceCadStructure(searchItem, cloneItem.Key, cloneItem.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 設定更新結構
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="sourceConfigIdId"></param>
        /// <param name="replaceConfigIdId"></param>
        private void ReplaceCadStructure(SearchItem searchItem, string sourceConfigIdId, string replaceConfigIdId)
        {
            try
            {

                Dictionary<string, string> cloneItemId = new Dictionary<string, string>();
                foreach (CADStructure link in searchItem.CadStructure.Where(x => x.ItemConfigId == sourceConfigIdId))
                {
                    link.ItemConfigId = replaceConfigIdId;
                    searchItem.IsNewVersion = true;//要更新結構
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 產生新檔案物件
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Aras.IOM.Item NewFileItem(string directory, string fileName)
        {
            try
            {
                Aras.IOM.Item newFileItem = null;

                if (File.Exists(Path.Combine(directory, fileName)))
                {
                    string itemtype = ItemTypeName.File.ToString();
                    Aras.IOM.Item newFile = AsInnovator.newItem(itemtype, "add");
                    newFile.setProperty("filename", fileName);
                    newFile.setProperty("actual_filename", directory);
                    newFile.setProperty("checkedout_path", directory);
                    //newFile.setProperty("mimetype", "");
                    //newFile.setProperty("file_type", "");
                    newFile.attachPhysicalFile(Path.Combine(directory, fileName));
                    newFileItem = newFile.apply();
                    if (newFileItem == null) throw new Exception(String.Format(GetLanguageByKeyName("msg_FailedToAddObject"), itemtype));
                    if (newFileItem.isError()) throw new Exception(newFileItem.getErrorString());
                }

                return newFileItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 刪除properties
        /// </summary>
        /// <param name="properties"></param>
        private void RemovePropertiesItem(ObservableCollection<PLMProperty> properties)
        {
            try
            {

                properties.RemoveAt(0);

            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

        }

        /// <summary>
        /// 更新CAD結構
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="item"></param>
        private void UpdateCADStructure(SearchItem searchItem, Aras.IOM.Item item)
        {
            try
            {


                //刪除所有連結
                Aras.IOM.Item qureyResult = GetPLMSearchItems(" id,related_id from [innovator].[{0}] where source_id='{1}'{2}", SyncLinkItemTypes.CAD_Structure.ToString().ToUpper(), item.getID(), "");
                for (var i = 0; i < qureyResult.getItemCount(); i++)
                {
                    AsInnovator.getItemById("CAD Structure", qureyResult.getItemByIndex(i).getProperty("id", "")).apply("purge");
                }

                //建立新連結
                List<string> itemConfigIds = searchItem.CadStructure.Select(x => x.ItemConfigId).ToList();
                if (itemConfigIds.Count() < 1) return;
                itemConfigIds = itemConfigIds.Distinct().ToList();

                string itemType = Regex.Replace(SyncLinkItemTypes.CAD_Structure.ToString(), "_", " ");
                foreach (CADStructure structrue in searchItem.CadStructure)
                {
                    Aras.IOM.Item newItem = AsInnovator.newItem(itemType, "add");
                    newItem.setProperty("source_id", item.getID());
                    newItem.setProperty("related_id", structrue.Child.ItemId);
                    newItem.setProperty("sort_order", structrue.Order.ToString());
                    newItem.setProperty("bcs_instance_id", structrue.InstanceId);
                    newItem.setProperty("bcs_is_suppressed", ((structrue.IsSuppressed == true) ? 1 : 0).ToString());//Modify by kenny 2020/08/13
                    Aras.IOM.Item linkItem = newItem.apply();
                    if (linkItem == null) throw new Exception(String.Format(GetLanguageByKeyName("msg_FailedToAddObject"), itemType) );
                    if (linkItem.isError()) throw new Exception(linkItem.getErrorString());
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新2D關連結構
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="item"></param>
        private void Update2DDrawingCADStructure(SearchItem searchItem, Aras.IOM.Item item)
        {
            try
            {

                string itemType = Regex.Replace(SyncLinkItemTypes.BCS_CAD_Drawing.ToString(), "_", " ");

                SearchItem sourceSearchItem = searchItem.CadStructure.Where(x => x.ItemType == itemType).Select(x => x.Child).FirstOrDefault();

                if (sourceSearchItem == null) return;
                //更新所有連結
                Aras.IOM.Item qureyResult = GetPLMSearchItems(" a.id,a.related_id,a.source_id from [innovator].[" + SyncLinkItemTypes.BCS_CAD_Drawing.ToString().ToUpper() + "] as a left join [innovator].[CAD] b on a.source_id=b.id left join [innovator].[File] as c on b.{0}=c.id where a.related_id='{1}' and (c.filename = (N'{2}') or b.bcs_added_filename = (N'{2}')) and b.is_current=1 ", NativeProperty, item.getID(), sourceSearchItem.FileName);

                Aras.IOM.Item linkItem;
                for (var i = 0; i < qureyResult.getItemCount(); i++)
                {

                    if (qureyResult.getItemByIndex(i).getProperty("related_id", "") == item.getID()) continue;
                    linkItem = AsInnovator.getItemById(itemType, qureyResult.getItemByIndex(i).getProperty("id", ""));
                    linkItem.setAction("edit");
                    linkItem.setAttribute("version", "0");
                    linkItem.setAttribute("serverEvents", "0");
                    linkItem.setProperty("related_id", item.getID());
                    Aras.IOM.Item editItem = linkItem.apply();
                    if (editItem == null) return;
                    if (editItem.isError() == true) throw new Exception(editItem.getErrorString());
                }

                if (qureyResult.getItemCount() > 0) return;


                //if (sourceSearchItem.ItemConfigId == "") throw new Exception(sourceSearchItem.FileName + " config_id is null");
                if (String.IsNullOrWhiteSpace(sourceSearchItem.ItemConfigId)) throw new Exception(String.Format(GetLanguageByKeyName("msg_FailedToAddObject"), sourceSearchItem.FileName)) ;
                Aras.IOM.Item lastItem = GetLastItem(ItemTypeName.CAD.ToString(), sourceSearchItem.ItemConfigId);

                //建立新連結
                Aras.IOM.Item newItem = AsInnovator.newItem(itemType, "add");
                newItem.setProperty("source_id", lastItem.getID());
                newItem.setProperty("related_id", item.getID());
                linkItem = item.apply();
                if (linkItem == null) throw new Exception(String.Format(GetLanguageByKeyName("msg_FailedToAddObject"), itemType));
                if (linkItem.isError()) throw new Exception(newItem.getErrorString());

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// User Login主要程式
        /// </summary>
        /// <param name="isAuth"></param>
        /// <param name="strUrl"></param>
        /// <param name="strDBName"></param>
        /// <param name="strLogin"></param>
        /// <param name="strPassword"></param>
        /// <returns></returns>
        private bool UserLogin(bool isAuth, string strUrl, string strDBName, string strLogin, string strPassword)
        {
            try
            {
                string connkey = "Server.Connection";
                string innovatorkey = "Aras.IOM.Innovator";

                if (_asServerConn.ContainsKey(connkey))
                {
                    HttpServerConnection value = _asServerConn[connkey];
                    value.Logout();
                    _asServerConn.Remove(connkey);
                }


                if (_asInnovator.ContainsKey(innovatorkey)) _asInnovator.Remove("Aras.IOM.Innovator");

                HttpServerConnection asConnection = (isAuth) ? IomFactory.CreateWinAuthHttpServerConnection(strUrl, strDBName) : IomFactory.CreateHttpServerConnection(strUrl, strDBName, strLogin, strPassword);

                if (asConnection == null) throw new Exception((isAuth) ? GetLanguageByKeyName("msg_FailToWinAuthHttpServerConnection") : GetLanguageByKeyName("msg_FailToHttpServerConnection"));

                Aras.IOM.Item itemLogin = asConnection.Login();
                if (itemLogin == null) throw new Exception(GetLanguageByKeyName("msg_LoginFailed"));
                if (itemLogin.isError()) throw new Exception(itemLogin.getErrorString());

                _asServerConn.Add(connkey, asConnection);//"HttpServerConnection";"WinAuthHttpServerConnection"
                _asInnovator.Add(innovatorkey, IomFactory.CreateInnovator(asConnection));

                _xmlConfigurations = null;
                _cadRequiredProperties = null;
                _language.Clear();
                SystemLanguages();

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        /// <summary>
        /// 取得執行事件
        /// </summary>
        /// <param name="syncEvent"></param>
        /// <param name="syncEventBefore"></param>
        /// <param name="syncEventAfter"></param>
        /// <param name="syncName"></param>
        private void GetExecSyncEvents(string syncEvent, ref SyncEvents syncEventBefore, ref SyncEvents syncEventAfter, ref string syncName)
        {
            try
            {

                syncEventBefore = SyncEvents.None;
                syncEventAfter = SyncEvents.None;
                syncName = "";

                foreach (Dictionary<string, List<SyncEvents>> syncEvents in ExecSyncEvents.Where(x => x.Key == syncEvent.ToString()).Select(x => x.Value))
                {

                    syncName = syncEvents.Keys.First();
                    foreach (List<SyncEvents> listEvents in syncEvents.Select(x => x.Value))
                    {
                        syncEventBefore = listEvents.First();
                        syncEventAfter = listEvents.Last();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增SyncEvents
        /// </summary>
        /// <param name="syncEvents"></param>
        /// <param name="beforeItemEvent"></param>
        /// <param name="afteItemEvent"></param>
        /// <param name="syncCadCommand"></param>
        /// <param name="beforeEvent"></param>
        private void AddSyncEvents(SyncEvents beforeItemEvent, SyncEvents afteItemEvent, SyncCadCommands syncCadCommand, SyncEvents beforeEvent)
        {
            try
            {

                Dictionary<string, List<SyncEvents>>  syncEvents = new Dictionary<string, List<SyncEvents>>();
                List<SyncEvents> listEvents = new List<SyncEvents>();
                listEvents.Add(beforeItemEvent);
                listEvents.Add(afteItemEvent);
                syncEvents.Add(syncCadCommand.ToString(), listEvents);
                _execSyncEvents.Add(beforeEvent.ToString(), syncEvents);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新屬性顯示值
        /// </summary>
        /// <param name="currentProperties"></param>
        private void UpdateListDisplayValue(ObservableCollection<PLMProperty> currentProperties)
        {

            try
            {
                
                //foreach (PLMProperties plmProperty in currentProperties.Where(x => x.DataSource != "" && x.ListItem.Count() > 0 && (x.DataType == "list" || x.DataType == "filter list")))
                foreach (PLMProperty plmProperty in currentProperties.Where(x => String.IsNullOrWhiteSpace(x.DataSource)==false && x.ListItems.Count() > 0 && (x.DataType == "list" || x.DataType == "filter list")))
                {

                    plmProperty.DisplayValue = (plmProperty.ListItems.Where(x => x.Value == plmProperty.DataValue) != null) ? plmProperty.ListItems.Where(x => x.Value == plmProperty.DataValue).Select(x => x.Label).FirstOrDefault() : plmProperty.DisplayValue;
                    //plmProperty.DisplayValue[0] = (plmProperty.ListItem.Where(x => x.value == plmProperty.value) != null) ? plmProperty.ListItem.Where(x => x.value == plmProperty.value).Select(x => x.label).FirstOrDefault() : plmProperty.DisplayValue[0];
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 屬性值的設定(目前有對:日期格式調整)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemProperty"></param>
        private void SetProperty(Aras.IOM.Item item ,PLMProperty itemProperty)
        {
            
            try
            {
                string value = itemProperty.DataValue;
                if (itemProperty.DataType.Contains("date"))
                {
                    DateTime date = DateTime.Parse(itemProperty.DataValue);
                    if (date.Year > 1911) value = date.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                else if (itemProperty.DataType.Contains("item"))
                {
                    
                    //if (itemProperty.KeyedId != "" && itemProperty.KeyedName == itemProperty.DataValue) value =itemProperty.KeyedId;
                    if (String.IsNullOrWhiteSpace(itemProperty.KeyedId) ==false && itemProperty.KeyedName == itemProperty.DataValue) value =itemProperty.KeyedId;
                    else
                    {
                        Aras.IOM.Item dataSourceItem = AsInnovator.getItemByKeyedName(itemProperty.DataSource, itemProperty.DataValue);
                        value = (dataSourceItem != null) ? dataSourceItem.getID() : value;
                    }
                }
                item.setProperty(itemProperty.Name, value);

            }
            catch (Exception ex)
            {
                //throw ex;
                string strError = ex.Message;
            }
        }



        /// <summary>
        /// XmlDocument convert to XDocument
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        /*
        private XDocument ToXDocument(XmlDocument xmlDocument)
        {

            //using (var nodeReader = new XmlNodeReader(xmlDocument))
            //{
            //    nodeReader.MoveToContent();
            //    return XDocument.Load(nodeReader);
            //}

            XDocument xDocument = XDocument.Parse(xmlDocument.OuterXml);
            return xDocument;

        }
        */

        /// <summary>
        /// CAD整合定義:CAD產品定義
        /// </summary>
        /// <param name="strProductName"></param>
        /// <returns></returns>
        private string  GetCADConfigAML(string strProductName)
        {
            try
            {
                List<string> selects = new List<string> ();
                List<string> code = new List<string>();
                foreach (var lagnlist in _language)
                {
                    //string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).ElementAt(0);
                    string suffix = lagnlist.Value.Where(x => x.Key == "suffix").Select(x => x.Value).FirstOrDefault();
                    if (String.IsNullOrWhiteSpace(suffix) ==false) selects.AddRange(new List<string> { "label" + suffix });
                }

                return GetConfigCADConfigAML(strProductName, selects);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 系統語系
        /// </summary>
        private void SystemLanguages()
        {
            try
            {
                if (_language.Count() >0) return;

                List<string> selects = new List<string> {"name","id","code","suffix" };
                string aml = GetConfigSystemLanguages(selects);

                Aras.IOM.Item itemLangs = AsInnovator.applyAML(aml);
                if (itemLangs.isError()) return;

                for (var i=0;i< itemLangs.getItemCount(); i++)
                {
                    Dictionary<string, string> langProperty = new Dictionary<string, string>();
                    foreach (var propertyName in selects)
                    {
                        langProperty.Add(propertyName, itemLangs.getItemByIndex(i).getProperty(propertyName, ""));
                    }
                    _language.Add(itemLangs.getItemByIndex(i).getProperty("name", ""), langProperty);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        /// <summary>
        /// 取得列表定義
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="itemtype"></param>
        /// <param name="selects"></param>
        /// <returns></returns>
        private string GetConfigList(string listName, string itemtype, List<string> selects)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                stringBuilder.Append("<Item type = 'List' action = 'get' select = 'id,name' >");
                stringBuilder.Append("<is_current>1</is_current>");
                stringBuilder.Append($"<name>{listName}</name>");
                stringBuilder.Append("<Relationships>");
                stringBuilder.Append($"<Item type = '{itemtype}' action = 'get' select = '{String.Join(",", selects)}' >");
                stringBuilder.Append("</Item>");
                stringBuilder.Append("</Relationships>");
                stringBuilder.Append("</Item>");
                stringBuilder.Append("</AML>");
                return stringBuilder.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得查詢類別的物件特殊屬性配置定義
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="selects"></param>
        /// <returns></returns>
        private string GetConfigSearchItemTypeProperties(string itemType, List<string> selects)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                stringBuilder.Append($"<Item type = 'Property' action = 'get' select = '{String.Join(",", selects)}' orderBy='sort_order ASC' >");
                stringBuilder.Append("<is_current>1</is_current>");
                stringBuilder.Append("<is_hidden>0</is_hidden>");
                stringBuilder.Append("<source_id>");
                stringBuilder.Append(AsInnovator.getItemByKeyedName("ItemType", itemType).getID());
                stringBuilder.Append("</source_id>");
                stringBuilder.Append("<data_type condition='not in' >'foreign'</data_type>");

                stringBuilder.Append("</Item>");
                stringBuilder.Append("</AML>");
                return stringBuilder.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得CAD的必填屬性配置定義
        /// </summary>
        /// <param name="selects"></param>
        /// <returns></returns>
        private string GetConfigCADRequiredProperties(List<string> selects)
        {
            try
            {

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                stringBuilder.Append($"<Item type = 'Property' action = 'get' select = '{String.Join(",", selects)}' >");
                stringBuilder.Append("<is_current>1</is_current>");
                stringBuilder.Append("<is_required>1</is_required>");
                stringBuilder.Append("<source_id>");
                stringBuilder.Append(AsInnovator.getItemByKeyedName("ItemType", ItemTypeName.CAD.ToString()).getID());
                stringBuilder.Append("</source_id>");
                stringBuilder.Append("<name condition='not in' >'created_on','created_by_id','config_id','permission_id','modified_on','modified_by_id','id'</name>");
                stringBuilder.Append("</Item>");
                stringBuilder.Append("</AML>");
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// CAD整合定義:CAD產品定義
        /// </summary>
        /// <param name="strProductName"></param>
        /// <param name="selects"></param>
        /// <returns></returns>
        private string GetConfigCADConfigAML(string strProductName, List<string> selects)
        {
            try
            {

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                //處理抑制:bcs_check_lightweightallresolved
                //stringBuilder.Append("<Item type = 'BCS CAD Integration CM' action = 'get' select = 'bcs_cad_classification,id' >");
                stringBuilder.Append("<Item type = 'BCS CAD Integration CM' action = 'get' select = 'bcs_cad_classification,id,bcs_is_resolve_all_lightweight,bcs_is_resolve_suppres' >");
                stringBuilder.Append("<bcs_cad_classification>" + strProductName + "</bcs_cad_classification>");
                //stringBuilder.Append("<bcs_cad_classification>CATIA</bcs_cad_classification>");
                stringBuilder.Append("<is_current>1</is_current>");
                stringBuilder.Append("<Relationships>"); //BCS CAD Integration Definition(BCS CAD Define Class),BCS Class Composition(BCS CAD Define Class),BCS CAD Integration Events,BCS CAD Conditional Rules(Property),BCS CAD Operation Options,BCS CAD PA
                stringBuilder.Append("<Item type = 'BCS CAD Integration Definition' action = 'get' select = 'sort_order,bcs_cad_classification,related_id' >");
                stringBuilder.Append("<related_id>");
                //stringBuilder.Append("<Item type = 'BCS CAD Define Class' action = 'get' select = 'id,source_id,related_id,bcs_cad_classification,name,bcs_cad_file_types,bcs_extension_name,bcs_extensions' >");
                stringBuilder.Append("<Item type = 'BCS CAD Define Class' action = 'get' select = 'id,source_id,related_id,bcs_cad_classification,name,bcs_cad_file_types,bcs_extension_name,bcs_extensions,bcs_thumbnail' >");

                stringBuilder.Append("<Relationships>");//BCS CAD Class Keys(Property),BCS CAD Properties(Property),BCS CAD File Property(BCS CAD Dedicated Format),BCS CAD Relationship File,BCS CAD Class Template File(File),BCS CAD Plug in Module(BCS External Component File)
                stringBuilder.Append("<Item type = 'BCS CAD Class Keys' action = 'get' select = 'related_id,sort_order,bcs_value' >");
                stringBuilder.Append("<related_id>");
                stringBuilder.Append("<Item type = 'Property' action = 'get' select = 'name,id' ></Item>");
                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");

                stringBuilder.Append("<Item type = 'BCS CAD Properties' action = 'get' select = 'related_id,sort_order,bcs_ref_cad_classification,bcs_sync_plm,bcs_sync_cad,bcs_cad_property_classification,bcs_cad_property,bcs_cad_default_property,data_type,data_source,bcs_parameters' >");
                stringBuilder.Append("<related_id>");
                //stringBuilder.Append("<Item type = 'Property' action = 'get' select = 'name,id,data_type,data_source' ></Item>");
                stringBuilder.Append(String.Format("<Item type = 'Property' action = 'get' select = 'name,id,data_type,data_source,pattern,is_required,label{0}' ></Item>", (selects.Count() < 1) ? "" : "," + String.Join(",", selects)));
                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");

                stringBuilder.Append("<Item type = 'BCS CAD File Property' action = 'get' select = 'related_id,sort_order' >");
                stringBuilder.Append("<related_id>");
                stringBuilder.Append("<Item type = 'BCS CAD Dedicated Format' action = 'get' select = 'name,id,bcs_property_name(keyed_name,data_type)' ></Item>");
                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");

                stringBuilder.Append("<Item type = 'BCS CAD Relationship File' action = 'get' select = 'related_id,sort_order,bcs_cad_relationship(name)' ></Item>");

                stringBuilder.Append("<Item type = 'BCS CAD Class Template File' action = 'get' select = 'related_id,sort_order,name,bcs_ref_related_filename' >");
                stringBuilder.Append("<related_id>");
                stringBuilder.Append("<Item type = 'File' action = 'get' select = 'filename,id' ></Item>");
                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");


                stringBuilder.Append("<Item type = 'BCS CAD Plug in Module' action = 'get' select = 'related_id,sort_order,name,bcs_tips,bcs_image' >");
                stringBuilder.Append("<related_id>");
                stringBuilder.Append("<Item type = 'BCS External Component File' action = 'get' select = 'id,bcs_assembly_name,bcs_class_name,bcs_ref_related_filename,related_id' ></Item>");
                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");

                stringBuilder.Append("</Relationships>");
                stringBuilder.Append("</Item>");

                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");

                stringBuilder.Append("<Item type = 'BCS Class Composition' action = 'get' select = 'sort_order,bcs_sub_class,related_id' >");
                stringBuilder.Append("<related_id>");
                //stringBuilder.Append("<Item type = 'BCS CAD Define Class' action = 'get' select = 'id,source_id,related_id,bcs_cad_classification,name,bcs_cad_file_types,bcs_extension_name,bcs_extensions' ></Item>");
                stringBuilder.Append("<Item type = 'BCS CAD Define Class' action = 'get' select = 'id,source_id,related_id,bcs_cad_classification,name,bcs_cad_file_types,bcs_extension_name,bcs_extensions,bcs_thumbnail' ></Item>");
                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");


                stringBuilder.Append("<Item type = 'BCS CAD Integration Events' action = 'get' select = 'sort_order,bcs_cad_event,bcs_method,bcs_external_component(bcs_assembly_name,bcs_class_name,bcs_ref_related_filename,related_id)' ></Item>");
                //BCS CAD Conditional Rules(Property),BCS CAD Operation Options,BCS CAD PA
                stringBuilder.Append("<Item type = 'BCS CAD Conditional Rules' action = 'get' select = 'sort_order,bcs_name,bcs_condition,bcs_value,related_id' >");
                stringBuilder.Append("<related_id>");
                stringBuilder.Append("<Item type = 'Property' action = 'get' select = 'name,id' ></Item>");
                stringBuilder.Append("</related_id>");
                stringBuilder.Append("</Item>");

                stringBuilder.Append("<Item type = 'BCS CAD Operation Options' action = 'get' select = 'sort_order,bcs_operation,bcs_name,bcs_is_checked,bcs_is_visible,bcs_is_enabled,bcs_method(name)' ></Item>");
                //CAD產品授權
                //aml += "<Item type = 'BCS CAD PA' action = 'get' select = 'bcs_product' ></Item>";

                //參考文件:共用零件庫路徑
                stringBuilder.Append("<Item type = 'BCS CAD Referenced Documents' action = 'get' select = 'sort_order,name,bcs_common_parts_library_path' ></Item>");

                stringBuilder.Append("</Relationships>");
                stringBuilder.Append("</Item>");


                stringBuilder.Append("</AML>");


                return stringBuilder.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 透過Config Id取得CAD物件屬性(AML)
        /// </summary>
        /// <param name="selects"></param>
        /// <param name="itemConfigIds"></param>
        /// <returns></returns>
        private string GetConfigCadAmlBuilder(List<string> selects, List<string> itemConfigIds)
        {
            try
            {

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                stringBuilder.Append($"<Item type = 'CAD' action = 'get' select = '{String.Join(",", selects)}' >");
                stringBuilder.Append("<is_current>1</is_current>");
                stringBuilder.Append($"<config_id condition='in'>'{String.Join("','", itemConfigIds)}'</config_id>");
                stringBuilder.Append("</Item>");
                stringBuilder.Append("</AML>");
                string aml = stringBuilder.ToString();

                return aml;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return "";
            }
        }


        /// <summary>
        /// 系統語系定義
        /// </summary>
        /// <param name="selects"></param>
        /// <returns></returns>
        private string GetConfigSystemLanguages(List<string> selects)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<AML>");
                stringBuilder.Append($"<Item type = 'Language' action = 'get' select = '{String.Join(",", selects)}' >");
                stringBuilder.Append("</Item>");
                stringBuilder.Append("</AML>");
                return stringBuilder.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion


        //private string get
        //BCS CAD Integration CM : bcs_cad_classification 	產品分類

        //RelationshipTypes (BCS CAD Integration CM) : 
        //BCS CAD Integration Definition  CAD整合定義 BCS CAD Define Class
        //
        //BCS Class Composition   類別組成 BCS CAD Define Class
        //BCS CAD Integration Events CAD整合事件
        //BCS CAD Conditional Rules   條件規則 Property
        //BCS CAD Operation Options   操作選項


        //BCS CAD Define Class
        //bcs_cad_classification 產品分類    list(Authoring Tools)	
        //name 類別名稱    string
        //bcs_cad_file_types CAD檔案類型     list(BCS CAD File Types)	
        //bcs_extension_name 副檔名
        //bcs_extensions 相關的副檔名


        //RelationshipTypes (BCS CAD Define Class ):
        //BCS CAD Class Keys  CAD類別特定屬性值 Property
        //sort_order 序號  integer X       left 	70 	0 		
        //bcs_value 特定屬性值   string (512)
        //BCS CAD Properties 系統欄位和CAD屬性  Property
        //sort_order 序號  integer X       left 	70 	0 		
        //bcs_ref_cad_classification 產品分類    foreign left 	200 	30 	Pattern: bcs_cad_classification
        //bcs_sync_plm    同步PLM 	
        //bcs_sync_cad 同步CAD  		
        //related_id PLM屬性   item(Property)
        //bcs_cad_property_classification CAD屬性分類     filter list	Pattern: bcs_ref_cad_classification
        //bcs_cad_property    CAD屬性 string 	
        //bcs_cad_default_property CAD預設屬性     filter list Pattern: bcs_ref_cad_classification
        //data_type   CAD資料類型 list(Data Types) 
        //data_source CAD資料來源
        //BCS CAD File Property CAD檔案屬性     BCS CAD Dedicated Format
        //BCS CAD Relationship File   CAD關聯檔案定義
        //BCS CAD Class Template File     類別範本檔案 File
        //BCS CAD Plug in Module 外掛模組    BCS External Component File

    }
}
