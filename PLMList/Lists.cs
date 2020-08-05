#region "                   名稱空間"
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace BCS.CADs.Synchronization.PLMList
{
    /// <summary>
    /// 列表
    /// </summary>
    public class Lists: NotifyPropertyBase , ILists
    {
        #region "                   屬性"
        /// <summary>
        ///  列表名稱
        /// </summary>
        public string Name { get; set; } = "";//data_source

        /// <summary>
        ///  列表參考來源名稱
        /// </summary>
        public string RefDataSource { get; set; } = "";//ref_data_source

        /// <summary>
        /// 列表id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 列表項目
        /// </summary>
        //public ObservableCollection<ListItem> listItem { get; set; } = new ObservableCollection<ListItem>();
        private ObservableCollection<PLMListItem> _listItems = new ObservableCollection<PLMListItem>();
        public ObservableCollection<PLMListItem> ListItems
        {
            get { return _listItems; }
            set { SetProperty(ref _listItems, value); }
        }


        #endregion
    }
}
