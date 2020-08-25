using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// SubItemSearchDialog.xaml 的互動邏輯
    /// </summary>
    public partial class SubItemSearchDialog : Window
    {

        public SubItemSearchDialog()
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
        }

        public SubItemSearchDialog(string itemType)
        {
            InitializeComponent();
            

            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
            int index = ClsSynchronizer.SyncSubDialogLoadingAdorner.Count() - 1;
            ClsSynchronizer.SyncSubDialogLoadingAdorner[index.ToString()] = LoadingAdorner;

            /*
            DataContext = new MainWindowViewModel(this, itemType,true );
            //DataContext = new SubItemSearchDialogViewModel(this, itemType);
            */

            //System.Diagnostics.Debugger.Break();
            SubItemSearchViewModel DataContext = new SubItemSearchViewModel();
            DataContext.SetView = this;
            DataContext.ShowSearchDialog(itemType);
            
            
        }
        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }
        private void btnActionClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


       
    }
}
