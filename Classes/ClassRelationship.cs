
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
    /// CAD關聯檔案定義
    /// </summary>
    public class ClassRelationship : IClassRelationship
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
        /// 功能名稱
        /// </summary>
        [XmlSettingAttributeName("name")]
        public string FunctionName { get; set; }

        /// <summary>
        /// CAD關係類型
        /// </summary>
        [XmlSettingAttributeName("bcs_cad_relationshi")]
        public string RelationshipName { get; set; }


        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
