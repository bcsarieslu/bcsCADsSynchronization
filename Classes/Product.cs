
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Entities;
using BCS.CADs.Synchronization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion


namespace BCS.CADs.Synchronization.Classes 
{
    /// <summary>
    /// 產品配置
    /// </summary>
    internal class Product : IProduct
    {
        #region "                   宣告區"

        /// <summary>
        /// Aras Innovator Configurations
        /// </summary>
        private XDocument _xmlSetting = null;
        private XElement _xmlRequired = null;
        #endregion

        #region "                   屬性"

        public List<ClassItem> PdClassItems { get; set; } = new List<ClassItem>();
        public List<ClassComposition> PdComposition { get; set; } = new List<ClassComposition>();
        public List<IntegrationEvents> PdEvents { get; set; } = new List<IntegrationEvents>();
        public List<ConditionalRule> PdRules { get; set; } = new List<ConditionalRule>();
        public List<OperationOption> PdOptions { get; set; } = new List<OperationOption>();

        public bool IsResolveAllLightweightSuppres { get; set; } = true;
        public bool IsResolveAllSuppres { get; set; } = true;

        [XmlSettingAttributeName("name")]
        public string Name { get; set; } = "";

        public CommonPartsLibrary PartsLibrary { get; set; } = new CommonPartsLibrary();


        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"
        /// <summary>
        /// 外部程式進入點
        /// </summary>
        /// <param name="xd"></param>
        public Product(XDocument xd, XElement xr)
        {
            _xmlSetting = xd;
            _xmlRequired = xr;
        }

        public void BuildClasses()
        {
            try
            {
                //BCS CAD Integration Definition(BCS CAD Define Class),BCS Class Composition(BCS CAD Define Class),BCS CAD Integration Events,BCS CAD Conditional Rules(Property),BCS CAD Operation Options,BCS CAD PA
                //foreach (XElement xmlRelItem in _xmlSetting.Descendants("Result").Single().Descendants("Item").Where(c => c.Attribute("type").Value == "BCS CAD Integration Definition"))
                //foreach (XElement xmlRelItem in _xmlSetting.Descendants("Result").Single()?.Elements("Item")?.Elements("Relationships")?.Elements("Item").Where(c => c.Attribute("type").Value == "BCS CAD Integration Definition"))

                //XElement xElement =_xmlSetting.Descendants("Result").Single()?.Elements("Item").SingleOrDefault();
                XElement xElement =_xmlSetting.Descendants("Result").Single()?.Elements("Item").FirstOrDefault();
                this.IsResolveAllLightweightSuppres = Convert.ToBoolean(int.Parse(xElement.Elements("bcs_is_resolve_all_lightweight").Single()?.Value));
                this.IsResolveAllSuppres = Convert.ToBoolean(int.Parse(xElement.Elements("bcs_is_resolve_suppres").Single()?.Value));
                foreach (XElement xmlRelItem in _xmlSetting.Descendants("Result").Single()?.Elements("Item")?.Elements("Relationships")?.Elements())
                {
                    switch (xmlRelItem.Attribute("type").Value)
                    {
                        case "BCS CAD Integration Definition"://CAD整合定義
                            if (xmlRelItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                string strType = xmlRelItem.Attribute("type")?.Value;
                                string strId = xmlRelItem.Attribute("id")?.Value;
                                var classItem = new ClassItem(xmlRelItem, _xmlRequired);
                                

                                //鍵名稱
                                classItem.KeyedName = xmlRelItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value;

                                //類別名稱
                                classItem.Name = xmlRelItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("name").Single()?.Value;

                                //CAD檔案類型
                                //classItem.FileType = xmlRelItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_cad_file_types").Single()?.Value;
                                classItem.FileClassName = xmlRelItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_cad_file_types").Single()?.Value;

                                //副檔名
                                classItem.Extension = xmlRelItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_extension_name").Single()?.Value;

                                //相關的副檔名
                                classItem.Extensions = xmlRelItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_extensions").Single()?.Value;

                                //縮圖
                                classItem.Thumbnail = xmlRelItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_thumbnail").Single()?.Value;

                                classItem.BuildClass();

                                PdClassItems.Add(classItem);

                            }
                            break;
                        case "BCS Class Composition"://類別組成
                            if (xmlRelItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                var classCompItem = new ClassComposition();
                                //父階類別鍵名稱
                                classCompItem.ParentKeyClass = xmlRelItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value;
                                //子階類別鍵名稱
                                classCompItem.SonKeyClass = xmlRelItem.Elements("id")?.Single()?.Attribute("keyed_name")?.Value;

                                PdComposition.Add(classCompItem);
                            }
                            break;

                        case "BCS CAD Integration Events"://CAD整合事件

                            var integrationEvents = new IntegrationEvents();
                            //序號
                            integrationEvents.Order = int.Parse(xmlRelItem.Elements("sort_order").Single()?.Value);

                            //事件
                            //integrationEvents.eventName = xmlRelItem.Elements("bcs_cad_event").Single()?.Value;
                            integrationEvents.EventName = (SyncEvents)Enum.Parse(typeof(SyncEvents), xmlRelItem.Elements("bcs_cad_event").Single()?.Value);

                            //Enum.Parse(typeof(Colors), colorName)
                            //方法
                            if (xmlRelItem.Elements("bcs_method")?.Single()?.Attribute("keyed_name")?.Value != null) integrationEvents.MethodName = xmlRelItem.Elements("bcs_method")?.Single()?.Attribute("keyed_name")?.Value;
                       
                            //外部組件
                            if (xmlRelItem.Elements("bcs_external_component")?.Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                //外部組件:組鍵名稱
                                integrationEvents.ComponentAssemblyName = xmlRelItem.Elements("bcs_external_component")?.Single()?.Elements("Item").Single()?.Elements("bcs_assembly_name").Single()?.Value;
                                //外部組件:類別名稱
                                integrationEvents.ComponentClassName = xmlRelItem.Elements("bcs_external_component")?.Single()?.Elements("Item").Single()?.Elements("bcs_class_name").Single()?.Value;

                                if (xmlRelItem.Elements("bcs_external_component")?.Single()?.Elements("Item").Single()?.Elements("related_id").Single()?.Attribute("keyed_name")?.Value != null)
                                {
                                    //外部組件:檔案名稱
                                    integrationEvents.ComponentFileName = xmlRelItem.Elements("bcs_external_component")?.Single()?.Elements("Item").Single()?.Elements("related_id").Single()?.Attribute("keyed_name")?.Value;

                                    //外部組件:檔案
                                    integrationEvents.ComponentFileId = xmlRelItem.Elements("bcs_external_component")?.Single()?.Elements("Item").Single()?.Elements("related_id").Single()?.Elements("Item").Single()?.Attribute("id")?.Value;
                                }
                            }
                            PdEvents.Add(integrationEvents);

                            break;
                        case "BCS CAD Conditional Rules"://條件規則
                            if (xmlRelItem.Elements("related_id").Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                var conditionalRule = new ConditionalRule();
                                //序號
                                conditionalRule.Order = int.Parse(xmlRelItem.Elements("sort_order").Single()?.Value);

                                //名稱
                                conditionalRule.Name = xmlRelItem.Elements("bcs_name").Single()?.Value;

                                //屬性名稱
                                conditionalRule.PrepertyName = xmlRelItem.Elements("related_id").Single()?.Attribute("keyed_name")?.Value;

                                //條件
                                conditionalRule.Condition = xmlRelItem.Elements("bcs_condition").Single()?.Value;

                                //條件值
                                conditionalRule.Value = xmlRelItem.Elements("bcs_value").Single()?.Value;

                                PdRules.Add(conditionalRule);
                            }

                            break;
                        case "BCS CAD Operation Options"://操作選項
                            //sort_order,bcs_operation,bcs_name,bcs_is_checked,bcs_is_visible,bcs_is_enabled,bcs_method(name)

                            var operationOption = new OperationOption();
                            //序號
                            operationOption.Order = int.Parse(xmlRelItem.Elements("sort_order").Single()?.Value);

                            //作業
                            operationOption.Name = xmlRelItem.Elements("bcs_operation").Single()?.Value;

                            //檢查
                            operationOption.IsChecked = Convert.ToBoolean(int.Parse(xmlRelItem.Elements("bcs_is_checked")?.Single()?.Value));

                            //可見
                            operationOption.IsVisible  = Convert.ToBoolean(int.Parse(xmlRelItem.Elements("bcs_is_visible")?.Single()?.Value));

                            //啟用
                            operationOption.IsEnabled  = Convert.ToBoolean(int.Parse(xmlRelItem.Elements("bcs_is_enabled")?.Single()?.Value));

                            //方法
                            if (xmlRelItem.Elements("bcs_method")?.Single()?.Attribute("keyed_name")?.Value != null) operationOption.MethodName = xmlRelItem.Elements("bcs_method")?.Single()?.Attribute("keyed_name")?.Value;

                            PdOptions.Add(operationOption);

                            break;

                        case "BCS CAD PA"://CAD產品授權
                            break;

                        case "BCS CAD Referenced Documents"://參考文件:共用零件庫路徑
                            string name = xmlRelItem.Elements("name").Single()?.Value;
                            string path = xmlRelItem.Elements("bcs_common_parts_library_path").Single()?.Value;
                            //if (String.IsNullOrWhiteSpace(name) ==false && String.IsNullOrWhiteSpace(path) == false) PartsLibrary.Paths.Add(name,path);
                            if (String.IsNullOrWhiteSpace(name) == false && String.IsNullOrWhiteSpace(path) == false) PartsLibrary.Paths.Add(new LibraryPath(name, path));
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
