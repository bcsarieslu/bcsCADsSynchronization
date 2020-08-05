
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Classes
{
    /// <summary>
    /// 外掛模組
    /// </summary>
    public class ClassPlugin: IClassPlugin
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
        /// 名稱
        /// </summary>
        [XmlSettingTagNameAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_tips")]
        public string Tips { get; set; }

        /// <summary>
        /// 圖片
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_image")]
        public string Image { get; set; }

        /// <summary>
        /// 外部組件檔案:組鍵名稱
        /// </summary>
        [XmlSettingTagNameAttribute("BCS External Component File.bcs_assembly_name")]
        public string ComponentAssemblyName { get; set; } = "";

        /// <summary>
        /// 外部組件檔案:類別名稱
        /// </summary>
        [XmlSettingTagNameAttribute("BCS External Component File.bcs_class_name")]
        public string ComponentClassName { get; set; } = "";

        /// <summary>
        /// 外部組件檔案"檔案名稱
        /// </summary>
        [XmlSettingAttributeName("BCS External Component File.filename")]
        public string ComponentFileName { get; set; } = "";

        /// <summary>
        /// 外部組件檔案:檔案
        /// </summary>
        [XmlSettingAttributeName("BCS External Component File.fileid")]
        public string ComponentFileId { get; set; } = "";


        /// <summary>
        /// 保留,尚未使用
        /// </summary>
        public string Value { get; set; }
        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
