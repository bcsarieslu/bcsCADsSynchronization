
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
    /// CAD類別特定屬性值
    /// </summary>
    public class PLMKeys : IDefaultElements, ICloneable
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
        /// PLM屬性名稱
        /// </summary>
        [XmlSettingAttributeName("keyed_name")]
        public string Name { get; set; }

        /// <summary>
        /// PLM屬性值
        /// </summary>
        [XmlSettingTagNameAttribute("bcs_value")]
        public string Value { get; set; }


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
