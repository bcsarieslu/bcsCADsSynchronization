
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
    /// 條件規則
    /// </summary>
    internal class ConditionalRule : IConditionalRule
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
        [XmlSettingTagNameAttribute("bcs_name")]
        public string Name { get; set; }

        /// <summary>
        /// 屬性名稱
        /// </summary>
        [XmlSettingTagNameAttribute("preperty.name")]
        public string PrepertyName { get; set; }


        /// <summary>
        /// 條件
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_condition")]
        public string Condition { get; set; } = "";

        /// <summary>
        /// 條件值
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_value")]
        public string Value { get; set; } = "";
        


        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion
    }
}
