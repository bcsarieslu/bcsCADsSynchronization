﻿using BCS.CADs.Synchronization.Classes;
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

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataGrid gridSelectedItems = (DataGrid)this.FindName("gridSelectedItems");
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(gridSelectedItems.ItemsSource);
            view.Filter = DataFilter;
        }

        private bool DataFilter(object item)
        {
            if (String.IsNullOrEmpty(searchTextBox.Text))
                return true;
            else
                return ((item as LibraryFileInfo).Name.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }




}
