
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
    /// 操作選項
    /// </summary>
    internal class OperationOption : IDefaultElements
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
        /// 作業
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_operation")]
        public string Name { get; set; }

        /// <summary>
        /// 檢查
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_is_checked")]
        public bool IsChecked { get; set; } =false;

        /// <summary>
        /// 可見
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_is_visible")]
        public bool IsVisible { get; set; } = false;

        /// <summary>
        /// 啟用
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_is_enabled")]
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// 方法
        /// </summary>
        [XmlSettingAttributeName("bcs_method")]
        public string MethodName { get; set; } = "";

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
