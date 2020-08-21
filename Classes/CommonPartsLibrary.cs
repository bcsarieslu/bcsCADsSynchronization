#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#endregion

namespace BCS.CADs.Synchronization.Classes
{
    public class CommonPartsLibrary
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"
        /// <summary>
        /// 庫路徑
        /// </summary>
        //public Dictionary<string,string> Paths { get; set; } = new Dictionary<string, string>();

        public ObservableCollection<LibraryPath> Paths { get; set; } = new ObservableCollection <LibraryPath>();

        #endregion

    }
}
