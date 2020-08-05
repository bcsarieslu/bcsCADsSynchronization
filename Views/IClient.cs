
#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion


namespace BCS.CADs.Synchronization.Views
{
    interface IClient
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        bool CADFiles { get; set; }
        bool CADStructure { get; set; }

        bool CADProperties { get; set; }


        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"
        void SaveFile();

        void ExportFile(string format);

        #endregion

        #region "                   方法(內部)"

        #endregion



    }
}
