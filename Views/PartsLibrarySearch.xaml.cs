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
    /// PartsLibrarySearch.xaml 的互動邏輯
    /// </summary>
    public partial class PartsLibrarySearch : Page
    {
        public PartsLibrarySearch()
        {
            InitializeComponent();

            //System.Diagnostics.Debugger.Break();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
            PartsLibrarySearchViewModel DataContext = new PartsLibrarySearchViewModel();
            DataContext.SetView = this;
            ClsSynchronizer.IsSyncCommonPageView = true;
            DataContext.ShowSearchDialog();
        }
    }




}
