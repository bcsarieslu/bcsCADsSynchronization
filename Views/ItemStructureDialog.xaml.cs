﻿using BCS.CADs.Synchronization.ViewModels;
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
using System.Windows.Shapes;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// SearchItemStructureDialog.xaml 的互動邏輯
    /// </summary>
    public partial class ItemStructureDialog : Window
    {
        bool window_size_max = true;

        public ItemStructureDialog()
        {
            InitializeComponent();
        }

        public ItemStructureDialog(string itemType, string id)
        {
            InitializeComponent();
            //System.Diagnostics.Debugger.Break();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
            ItemStructureViewModel DataContext = new ItemStructureViewModel();
            DataContext.SetView = this;
            //DataContext.ShowClassificationItems(itemType, "Electronic/Manufacturing");
            DataContext.ShowSearchItems(itemType, id);
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
