
#region "                   名稱空間"

using BCS.CADs.Synchronization.ConfigProperties;
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
    ///類別組成
    /// </summary>
    internal class ClassComposition
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 父階類別鍵名稱
        /// </summary>
        [XmlSettingAttributeName("keyed_name")]
        public string ParentKeyClass { get; set; }

        /// <summary>
        /// 子階類別鍵名稱
        /// </summary>
        [XmlSettingAttributeName("keyed_name")]
        public string SonKeyClass { get; set; }

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
