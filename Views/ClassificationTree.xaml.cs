using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// ClassificationTree.xaml 的互動邏輯
    /// </summary>
    public partial class ClassificationTree : Window//Page
    {

        bool window_size_max = true;

        public ClassificationTree()
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
        }

        public ClassificationTree(string itemType,string value)
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
            ClassificationTreeViewModel DataContext = new ClassificationTreeViewModel();
            DataContext.SetView = this;
            //DataContext.ShowClassificationItems(itemType, "Electronic/Manufacturing");
            DataContext.ShowClassificationItems(itemType, value);
        }


        private void treeViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //System.Diagnostics.Debugger.Break();
                TreeViewItem item = sender as TreeViewItem;
                TextBlock txtSelectedItem = e.OriginalSource as TextBlock;
                if (txtSelectedItem == null)
                {
                    Image imgSelectedItem = e.OriginalSource as Image;
                    selectedItemId.Tag = imgSelectedItem.Tag;
                    string[] arry = selectedItemId.Tag.ToString().Split((char)47);
                    selectedItemId.Text = arry[arry.Length - 1];
                    return;
                }
                selectedItemId.Text = txtSelectedItem.Text;
                selectedItemId.Tag = txtSelectedItem.Tag;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
                //throw ex;
            }
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
