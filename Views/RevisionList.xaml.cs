using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// RevisionList.xaml 的互動邏輯
    /// </summary>
    public partial class RevisionList : Window
    {
        public RevisionList()
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
        }

        public RevisionList(string itemType,string itemId)
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
            RevisionListViewModel DataContext = new RevisionListViewModel();
            DataContext.SetView = this;
            DataContext.ShowAllRevisions(itemType,itemId);
        }

    }
}
