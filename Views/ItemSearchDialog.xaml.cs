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
            DataContext = new MainWindowViewModel(this,"",false );
            

            //DataContext = new ViewModel(this);
            //ClsSynchronizer.VmFunction = SyncType.LoadFromPLM;
            //LoadFromPLMView((Window)x);
        }
        public ItemSearchDialog(string itemType)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this, itemType,false );


            //DataContext = new ViewModel(this);
            //ClsSynchronizer.VmFunction = SyncType.LoadFromPLM;
            //LoadFromPLMView((Window)x);
        }
    }
}
