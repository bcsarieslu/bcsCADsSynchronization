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
    /// SubItemSearchDialog.xaml 的互動邏輯
    /// </summary>
    public partial class SubItemSearchDialog : Window
    {
        public SubItemSearchDialog(string itemType)
        {
            InitializeComponent();
            //System.Diagnostics.Debugger.Break();
            DataContext = new MainWindowViewModel(this, itemType,true );
            //DataContext = new SubItemSearchDialogViewModel(this, itemType);
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
