using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

    }
}
