

#region "                   名稱空間"
using BCS.CADs.Synchronization.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Classes
{
    internal class ClassItemProperties
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"


        /// <summary>
        /// 名稱
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// PLM物件(透過查詢取到實體相關屬性)
        /// </summary>
        SearchItem SearchItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> SynPoperty { get; set; } = null;

        

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
