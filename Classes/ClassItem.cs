
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Entities;
using BCS.CADs.Synchronization.PLMList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
#endregion


namespace BCS.CADs.Synchronization.Classes
{
    //BCS CAD Class Keys(Property),BCS CAD Properties(Property),BCS CAD File Property(BCS CAD Dedicated Format),BCS CAD Relationship File,BCS CAD Class Template File(File),BCS CAD Plug in Module(BCS External Component File)
    /// <summary>
    /// CAD類別定義
    /// </summary>
    public class ClassItem:IClasses 
    {
        #region "                   宣告區"
        /// <summary>
        /// 傳入的XML設定檔
        /// </summary>
        private XElement _xmlSetting = null;
        private XElement _xmlRequired = null;
        

        #endregion

        #region "                   屬性"

        public List<PLMKey> CsKeys { get; set; } = new List<PLMKey>();
        public List<PLMProperty> CsProperties { get; set; } = new List<PLMProperty>();

        public List<PLMPropertyFile> CsPropertyFile { get; set; } = new List<PLMPropertyFile>();

        public List<ClassRelationship> CsRelationship { get; set; } = new List<ClassRelationship>();

        public List<ClassTemplateFile> CsTemplateFile { get; set; } = new List<ClassTemplateFile>();

        public List<ClassPlugin> CsPlugin { get; set; } = new List<ClassPlugin>();

        /// <summary>
        /// 鍵名稱
        /// </summary>
        [XmlSettingAttributeName("keyed_name")]//鍵名稱
        public string KeyedName { get; set; }

        /// <summary>
        /// 類別名稱
        /// </summary>
        [XmlSettingTagNameAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// CAD檔案類別名稱
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_cad_file_types")]
        public string FileClassName { get; set; }


        ///// <summary>
        ///// CAD檔案類型
        ///// </summary>
        //[XmlSettingTagNameAttribute("bcs_cad_file_types")]
        //public string FileType { get; set; }

        /// <summary>
        /// 副檔名
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_extension_name")]
        public string Extension { get; set; }

        /// <summary>
        /// 相關的副檔名
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_extensions")]
        public string Extensions { get; set; }

        /// <summary>
        /// 縮圖
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_thumbnail")]
        public string Thumbnail { get; set; } = "";

        /// <summary>
        /// 縮圖圖檔完整名稱
        /// </summary>
        public string ThumbnailFullName { get; set; } = "";

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"
        /// <summary>
        /// 外部程式進入點
        /// </summary>
        /// <param name="xd"></param>
        public ClassItem(XElement xd,XElement xr)
        {
            _xmlSetting = xd;
            _xmlRequired = xr;
            //SetClassItem();

        }



        /// <summary>
        /// 初始化傳入的XML
        /// </summary>
        public void BuildClass()
        {
            try
            {
                //@@@ Descendants 類似Find ,Elements類似Childern

                int intCount = _xmlSetting.Elements("related_id").Single().Elements("Item").Single().Elements("Relationships").Single().Elements().Count();
                XElement xmlRelation = _xmlSetting.Elements("related_id").Single().Elements("Item").Single().Elements("Relationships").Single();

                foreach (XElement xmlItem in _xmlSetting.Elements("related_id")?.Elements("Item")?.Elements("Relationships")?.Elements())
                {

                    switch (xmlItem.Attribute("type").Value)
                    {
                        case "BCS CAD Class Keys"://CAD類別特定屬性值
                            var key = new PLMKey();

                            //序號
                            key.Order = int.Parse(xmlItem.Elements("sort_order").Single()?.Value);

                            //PLM屬性名稱
                            key.Name = xmlItem.Elements("related_id").Single()?.Attribute("keyed_name")?.Value;

                            //CAD檔案類別名稱
                            key.FileClassName = FileClassName;

                            //PLM屬性值
                            key.Value = xmlItem.Elements("bcs_value").Single()?.Value;
                            CsKeys.Add(key);
                            break;
                        case "BCS CAD Properties"://系統欄位和CAD屬性
                            var property = new PLMProperty();

                            //序號
                            string sortOrder = xmlItem.Elements("sort_order")?.Single()?.Value;
                            if (string.IsNullOrWhiteSpace(sortOrder) == false ) property.Order = int.Parse(xmlItem.Elements("sort_order")?.Single()?.Value);

                            //同步PLM
                            property.IsSyncPLM = Convert.ToBoolean(int.Parse(xmlItem.Elements("bcs_sync_plm")?.Single()?.Value)); 

                            //同步CAD
                            property.IsSyncCAD = Convert.ToBoolean(int.Parse(xmlItem.Elements("bcs_sync_cad")?.Single()?.Value)); 

                            //CAD屬性分類
                            property.CadClass = xmlItem.Elements("bcs_cad_property_classification")?.Single()?.Value;

                            //CAD預設屬性名稱
                            property.CadName = xmlItem.Elements("bcs_cad_property")?.Single()?.Value;

                            //CAD預設屬性
                            property.CadDefaultName = xmlItem.Elements("bcs_cad_default_property")?.Single()?.Value;

                            //CAD資料類型
                            property.CadDataType = xmlItem.Elements("data_type")?.Single()?.Value;

                            //CAD資料來源
                            if (xmlItem.Elements("data_source")?.Single()?.Attribute("keyed_name")?.Value != null) property.CadDataSource = xmlItem.Elements("data_source")?.Single()?.Attribute("keyed_name")?.Value;

                            ////PLM資料樣式
                            //property.Pattern = xmlItem.Elements("Pattern")?.Single()?.Value;

                            //參數
                            property.Parameters = xmlItem.Elements("bcs_parameters")?.Single()?.Value;

                            if (xmlItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value !=null)
                            {
                                //PLM屬性名稱
                                property.Name = xmlItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value;

                                //PLM資料類型
                                property.DataType = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("data_type").Single()?.Value;

                                if (property.Name == "classification") property.DataType = "classification";//Modify by kenny 2020/08/31

                                //PLMCAD資料來源
                                property.DataSource = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("data_source").Single()?.Attribute("keyed_name")?.Value;

                                //PLM資料樣式
                                property.Pattern = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("pattern").Single()?.Value;
                                

                                //必填
                                property.IsRequired = Convert.ToBoolean(int.Parse(xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("is_required").Single()?.Value));
                                if (property.IsRequired) property.ResetSyncColorTypeValue();

                                //PLM屬性標籤名稱
                                property.Label = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("label").Single()?.Value;

                                foreach (XElement xmltag in xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements().Where(x => x.Name.ToString() == @"{http://www.aras.com/I18N}label"))
                                {
                                    string value = (xmltag.Attribute("is_null")?.Value == "1") ? xmltag.FirstAttribute.NextAttribute.Value : xmltag.FirstAttribute.Value;
                                    //標籤其他語系
                                    property.Labels.Add(value, xmltag.Value);
                                }
                            }



                            CsProperties.Add(property);
                            break;

                        case "BCS CAD File Property"://CAD檔案屬性
                            var fileProperty = new PLMPropertyFile();
                            //序號

                            string order = xmlItem.Elements("sort_order")?.Single()?.Value;
                            if (string.IsNullOrWhiteSpace(order) == false) fileProperty.Order = int.Parse(xmlItem.Elements("sort_order")?.Single()?.Value);
                            if (xmlItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                //功能名稱 (例如 : native_property,viewable_property,thumbnail_property)
                                fileProperty.FunctionName = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("name").Single()?.Value;

                                //屬性名稱
                                fileProperty.Name  = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_property_name").Single()?.Attribute("keyed_name")?.Value;

                                //資料類型
                                fileProperty.DataType = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_property_name").Single()?.Elements("Item").Single()?.Elements("data_type").Single()?.Value;
                            }
                            CsPropertyFile.Add(fileProperty);
                            break;
                        case "BCS CAD Relationship File"://CAD關聯檔案定義


                            if (xmlItem.Elements("bcs_cad_relationship")?.Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                var classRelationship = new ClassRelationship();
                                //序號
                                classRelationship.Order = int.Parse(xmlItem.Elements("sort_order")?.Single()?.Value);

                                //功能名稱
                                classRelationship.FunctionName = xmlItem.Elements("bcs_cad_relationship")?.Single()?.Attribute("keyed_name")?.Value;

                                //CAD關係類型
                                classRelationship.FunctionName = xmlItem.Elements("bcs_cad_relationship")?.Single()?.Attribute("name")?.Value;

                                CsRelationship.Add(classRelationship);

                            }

                            break;

                        case "BCS CAD Class Template File"://範本檔案
                                                           //related_id,sort_order,name,bcs_ref_related_filename
                            if (xmlItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                var classTemplateFile = new ClassTemplateFile();
                                //序號
                                classTemplateFile.Order = int.Parse(xmlItem.Elements("sort_order")?.Single()?.Value);

                                //名稱
                                classTemplateFile.Name = xmlItem.Elements("name")?.Single()?.Value;

                                //檔案名稱
                                classTemplateFile.FileName = xmlItem.Elements("related_id").Single()?.Attribute("keyed_name")?.Value;

                                //檔案
                                classTemplateFile.FileId = xmlItem.Elements("related_id").Single()?.Elements("Item").Single()?.Attribute("id")?.Value;


                                classTemplateFile.ClassName = Name;

                                classTemplateFile.FileClassName = FileClassName;

                                CsTemplateFile.Add(classTemplateFile);
                            }
                             break;

                        case "BCS CAD Plug in Module"://外掛模組
                            

                            //外部組件
                            if (xmlItem.Elements("related_id")?.Single()?.Attribute("keyed_name")?.Value != null)
                            {
                                var classPlugin = new ClassPlugin();

                                //序號
                                classPlugin.Order = int.Parse(xmlItem.Elements("sort_order").Single()?.Value);

                                //名稱
                                classPlugin.Name = xmlItem.Elements("name").Single()?.Value;

                                //提示
                                classPlugin.Tips = xmlItem.Elements("bcs_tips").Single()?.Value;

                                //圖片
                                classPlugin.Image = xmlItem.Elements("bcs_image").Single()?.Value;


                                //外部組件:組鍵名稱
                                classPlugin.ComponentAssemblyName = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_assembly_name").Single()?.Value;
                                //外部組件:類別名稱
                                classPlugin.ComponentClassName = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("bcs_class_name").Single()?.Value;

                                if (xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("related_id").Single()?.Attribute("keyed_name")?.Value != null)
                                {
                                    //外部組件:檔案名稱
                                    classPlugin.ComponentFileName = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("related_id").Single()?.Attribute("keyed_name")?.Value;

                                    //外部組件:檔案
                                    classPlugin.ComponentFileId = xmlItem.Elements("related_id")?.Single()?.Elements("Item").Single()?.Elements("related_id").Single()?.Elements("Item").Single()?.Attribute("id")?.Value;

                                }
                                CsPlugin.Add(classPlugin);
                            }

                            break;
                        default:
                            //Console.WriteLine("Default case");
                            break;
                    }
                }

                if (_xmlRequired != null)
                {
                    //必填屬性
                    foreach (XElement xmlItem in _xmlRequired?.Elements("Item"))
                    {
                        List<PLMProperty> plmProperties = CsProperties.Where(x => x.Name == xmlItem.Elements("name")?.Single()?.Value).ToList();
                        if (plmProperties.Count() < 1)
                        {
                            var property = new PLMProperty();

                            //PLM屬性名稱
                            property.Name = xmlItem.Elements("name")?.Single()?.Value;

                            //PLM資料類型
                            property.DataType = xmlItem.Elements("data_type")?.Single()?.Value;

                            //PLMCAD資料來源
                            property.DataSource = xmlItem.Elements("data_source")?.Single()?.Attribute("keyed_name")?.Value;

                            //PLM資料樣式
                            property.Pattern = xmlItem.Elements("pattern")?.Single()?.Value;

                            //必填
                            property.IsRequired = Convert.ToBoolean(int.Parse(xmlItem.Elements("is_required").Single()?.Value));

                            //PLM屬性標籤名稱
                            property.Label = xmlItem.Elements("label")?.Single()?.Value;

                            foreach (XElement xmltag in xmlItem.Elements().Where(x => x.Name.ToString() == @"{http://www.aras.com/I18N}label"))
                            {
                                //string name = xmltag.Name.ToString();
                                string value = (xmltag.Attribute("is_null")?.Value == "1") ? xmltag.FirstAttribute.NextAttribute.Value : xmltag.FirstAttribute.Value;
                                //標籤其他語系
                                property.Labels.Add(value, xmltag.Value);
                            }
                            CsProperties.Add(property);
                        }

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
