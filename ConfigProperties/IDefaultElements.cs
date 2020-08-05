
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
    interface IDefaultElements
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"
        /// <summary>
        /// 序號
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        string Value { get; set; }


        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
