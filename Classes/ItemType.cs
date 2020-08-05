#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
#endregion

namespace BCS.CADs.Synchronization.Classes
{

    public class ItemType
    {
            
        #region "                   宣告區"
        /// <summary>
        /// 傳入的XML設定檔
        /// </summary>
        private XElement _xmlSetting = null;
        #endregion

        #region "                   屬性"
        public List<PLMProperty> CsProperties { get; set; } = new List<PLMProperty>();

        /// <summary>
        /// 類別名稱
        /// </summary>
        [XmlSettingTagNameAttribute("name")]
        public string Name { get; set; }

        #endregion


        #region "                   事件"

        #endregion

        #region "                   方法"
        /// <summary>
        /// 外部程式進入點
        /// </summary>
        /// <param name="xd"></param>
        public ItemType(XElement xd, string itemtype)
        {
            _xmlSetting = xd;
            BuildProperties(itemtype);
        }



        #endregion

        #region "                   方法(內部)"

        private void BuildProperties(string itemtype)
        {
            try
            {

                PLMProperty property;
                bool isAddRevison = false;

                //"name", "id", "data_type", "data_source", "is_required", "label" ,"keyed_name" ,"related_id","sort_order"
                foreach (XElement xmlItem in _xmlSetting.Elements("Item").Where(x=>x.Attribute("type")?.Value == "Property"))
                {
                    //type =" Property " 
                    property = new PLMProperty();

                    //序號
                    string order = xmlItem.Elements("sort_order")?.Single()?.Value;
                    if (string.IsNullOrWhiteSpace(order) == false) property.Order = int.Parse(xmlItem.Elements("sort_order")?.Single()?.Value);

                    //PLM屬性名稱
                    property.Name = xmlItem.Elements("name")?.Single()?.Value;

                    //PLM資料類型
                    property.DataType = xmlItem.Elements("data_type")?.Single()?.Value;

                    //PLMCAD資料來源
                    property.DataSource = xmlItem.Elements("data_source")?.Single()?.Attribute("keyed_name")?.Value;

                    //PLM樣式
                    property.Pattern = xmlItem.Elements("pattern")?.Single()?.Value;

                    //PLM屬性標籤名稱
                    property.Label = xmlItem.Elements("label")?.Single()?.Value;

                    //欄位寬度
                    property.ColumnWidth = (xmlItem.Elements("column_width").Single()?.Value != "") ? int.Parse(xmlItem.Elements("column_width")?.Single()?.Value) :0;


                    foreach (XElement xmltag in xmlItem.Elements().Where(x => x.Name.ToString() == @"{http://www.aras.com/I18N}label"))
                    {
                        //string name = xmltag.Name.ToString();
                        string value = (xmltag.Attribute("is_null")?.Value == "1") ? xmltag.FirstAttribute.NextAttribute.Value : xmltag.FirstAttribute.Value;
                        //標籤其他語系
                        property.Labels.Add(value, xmltag.Value);
                    }

                    if (property.DataType == "image")
                    {
                        CsProperties.Add(property);
                        if (isAddRevison==false)AddRevisionProperty(itemtype);
                    }
                    else
                    {
                        if (isAddRevison == false) AddRevisionProperty(itemtype);
                        CsProperties.Add(property);
                    }
                    isAddRevison = true;
                }


                //CsProperties.
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void AddRevisionProperty(string itemtype)
        {

            if (itemtype != "CAD") return;
            PLMProperty property = new PLMProperty();
            property.Name = "sys_revision";
            property.DataType = "revision";
            property.Label = "Revision";
            CsProperties.Add(property);
        }

        #endregion
    }
}
