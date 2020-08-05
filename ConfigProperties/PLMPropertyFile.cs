
#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace BCS.CADs.Synchronization.ConfigProperties
{

    /// <summary>
    ///  	CAD關聯檔案定義
    /// </summary>
    public class PLMPropertyFile : IPLMPropertyFile
    {

        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 序號
        /// </summary>
        [XmlSettingTagNameAttribute("sort_order")]
        public int Order { get; set; }

        /// <summary>
        /// 功能名稱 : (例如 : native_property,viewable_property,thumbnail_property)
        /// </summary>
        [XmlSettingTagNameAttribute("name")]
        public string FunctionName { get; set; }


        /// <summary>
        /// 資料類型
        /// </summary>
        [XmlSettingTagNameAttribute("data_type")]
        public string DataType { get; set; } = "";

        /// <summary>
        /// 屬性名稱
        /// </summary>
        [XmlSettingAttributeName("bcs_property_name")]
        public string Name { get; set; }



        /// <summary>
        /// 檔案名稱 (實際同步到PLM時)
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// 檔案路徑 (實際同步到PLM時)
        /// </summary>
        public string FilePath { get; set; } = "";


        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
