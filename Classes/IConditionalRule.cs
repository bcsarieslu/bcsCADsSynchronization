
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
    interface IConditionalRule : IDefaultElements
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 屬性名稱
        /// </summary>
        string PrepertyName { get; set; }


        /// <summary>
        /// 條件
        /// </summary>
        string Condition { get; set; }





        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion
    }
}
