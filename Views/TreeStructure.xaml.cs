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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// TreeView.xaml 的互動邏輯
    /// </summary>
    public partial class TreeStructure : Page// UserControl
    {
        public TreeStructure()
        {
            InitializeComponent();
            //MessageBox.Show ("TreeStructure");
            //System.Diagnostics.Debugger.Break();
        }


        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = e.OriginalSource as DependencyObject;
            TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;


            BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel searchItem = (BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel)item.Header;
            if (ClsSynchronizer.VmFunction == SyncType.SyncFromPLM && searchItem.IsChecked==true && (searchItem.IsInsert == true|| searchItem.IsReplacement == true))
            {
                ContextMenu menu = new ContextMenu() { };
                menu.FontSize = 14;
                if (searchItem.IsInsert == true) AddMenuItem(menu, searchItem, "插入分件", true, false, false,false);
                if (searchItem.IsInsertSaveAs == true) AddMenuItem(menu, searchItem, "插入另存分件", false, false, false, true);
                if (searchItem.IsReplacement == true)
                {
                    AddMenuItem(menu, searchItem, "替換分件(單筆)", false,false,false, false);
                    AddMenuItem(menu, searchItem, "替換分件(全部)", false,true,false, false);
                }
                if (searchItem.IsCopyToAdd == true) AddMenuItem(menu, searchItem, "複製轉新增", false, false, true,false);
                (sender as TreeViewItem).ContextMenu = menu;
                return;
            }

            (sender as TreeViewItem).ContextMenu = null;
   
        }



        private static DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            var parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }


        //private void Page_Loaded(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("Page_Loaded");
        //}

        private void AddMenuItem(ContextMenu menu, BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel searchItem, string display,bool isInsert,bool isReplaceAll,bool isCopyToAdd, bool isInsertSaveAs)
        {

            BCS.CADs.Synchronization.ViewModels.MainWindowViewModel mainVM = new BCS.CADs.Synchronization.ViewModels.MainWindowViewModel();
            //System.Diagnostics.Debugger.Break();
            MenuItem mnuItem = new MenuItem();
            mnuItem.Header = display;
            //Image image = new System.Windows.Controls.Image();
            //image.Source = new BitmapImage(new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Images/Calculator_Icon.png"));

            string imageFileName = (isInsert) ? "Insert.bmp" : (isInsertSaveAs) ? "Insert.bmp" :(isCopyToAdd)? "CopyToAdd.bmp" : "ReplaceRevision.bmp";

            mnuItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri($"pack://application:,,,/BCS.CADs.Synchronization;Component/Images/{imageFileName}")),
                Width = 24,
                Height = 24
            };

            mnuItem.Command = (isInsert) ? mainVM.SyncInsertItem : (isCopyToAdd) ? mainVM.SyncCopyToAdd :(isInsertSaveAs) ? mainVM.SyncInsertSaveAs : (isReplaceAll) ? mainVM.SyncReplaceAllItems : mainVM.SyncReplaceItem;
            mnuItem.CommandParameter = searchItem;
            menu.Items.Add(mnuItem);

        }
    }
}
