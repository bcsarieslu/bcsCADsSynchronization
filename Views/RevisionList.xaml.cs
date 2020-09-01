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
    /// RevisionList.xaml 的互動邏輯
    /// </summary>
    public partial class RevisionList : Window
    {
        bool window_size_max = true;
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
