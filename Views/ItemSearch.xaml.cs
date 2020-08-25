using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// ItemSearch.xaml 的互動邏輯
    /// </summary>
    public partial class ItemSearch : Page
    {
        public ItemSearch()
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);

        }

        public ItemSearch(string itemType)
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);

            //ItemSearchViewModel DataContext = new ItemSearchViewModel();
            //DataContext.SetView = this;
            //ClsSynchronizer.IsSyncCommonPageView = true;
            //DataContext.ShowSearchDialog(itemType);
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {

            ////openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //string filter =String.Format ("{0} files (*.sldasm)|*.SLDASM|(*.sldpart)|*.SLDPRT", ClsSynchronizer.VmSyncCADs.CADSoftware);
            //if (ClsSynchronizer.VmFunction == SyncType.CopyToAdd)
            //    //"txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //    ClsSynchronizer.VmSyncCADs.CopyToAddSaveFile(CADdirectory.Text, filter);
            //    else
            //    ClsSynchronizer.VmSyncCADs.SelectFolder(CADdirectory.Text);

            ClsSynchronizer.VmSyncCADs.SelectFolder(CADdirectory.Text);
            CADdirectory.Text = ClsSynchronizer.VmDirectory;
        }
    }
}
