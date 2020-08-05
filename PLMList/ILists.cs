#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.PLMList
{
    interface ILists
    {
        #region "                   屬性"
        /// <summary>
        ///  列表名稱
        /// </summary>
        string Name { get; set; }//data_source

        /// <summary>
        ///  列表參考來源名稱
        /// </summary>
        string RefDataSource { get; set; }


        /// <summary>
        /// 列表id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 列表項目
        /// </summary>0
        ObservableCollection<PLMListItem> ListItems { get; set; }
        #endregion
    }
}
