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
using System.Windows.Shapes;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// ItemSearchDialog.xaml 的互動邏輯
    /// </summary>
    public partial class ItemSearchDialog : Window
    {
        bool window_size_max = true;

        public ItemSearchDialog()
        {
            InitializeComponent();
            ItemSearchLoadingAdorner();

            DataContext = new MainWindowViewModel(this,"",false );
            

            //DataContext = new ViewModel(this);
            //ClsSynchronizer.VmFunction = SyncType.LoadFromPLM;
            //LoadFromPLMView((Window)x);
        }
        public ItemSearchDialog(string itemType)
        {
            InitializeComponent();
            ItemSearchLoadingAdorner();
            DataContext = new MainWindowViewModel(this, itemType,false );


            //DataContext = new ViewModel(this);
            //ClsSynchronizer.VmFunction = SyncType.LoadFromPLM;
            //LoadFromPLMView((Window)x);
        }

        private void ItemSearchLoadingAdorner()
        {
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);

            var cache = MyCache.CacheInstance;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now.AddMinutes(30d);
            var loadingAdorner = new CacheItem("ItemSearchLoadingAdorner", LoadingAdorner);
            cache.Add(loadingAdorner, null);
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

        private void btnActionMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnActionMaximize_OnClick(object sender, RoutedEventArgs e)
        {
            if (window_size_max)
            {
                WindowState = WindowState.Maximized;
                window_size_max = false;
            }
            else
            {
                WindowState = WindowState.Normal;
                window_size_max = true;
            }
        }
    }
}
