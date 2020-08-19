
#region "                   名稱空間"
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BCS.CADs.Synchronization.ViewModels



#endregion


{
    public class SubItemSearchDialogViewModel : NotifyPropertyBase
    {
        #region "                   宣告區
        private ItemSearch _ItemSearchView = null;
        #endregion


        #region "                   進入區
        public SubItemSearchDialogViewModel()
        {

        }
        #endregion " 


        #region "                   屬性區
        public dynamic SetView
        {
            set
            {
                int index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
                ClsSynchronizer.SyncSubDialogView[index.ToString()] = (dynamic)value;
            }
        }

        /// <summary>
        /// 被選到的項目
        /// </summary>
        private SearchItem _selectedSearchItem;
        public SearchItem SelectedSearchItem
        {
            get { return _selectedSearchItem; }
            set { SetProperty(ref _selectedSearchItem, value, nameof(SelectedSearchItem)); }
        }

        public ObservableCollection<SearchItem> ObsSearchItems
        {
            get { return ClsSynchronizer._vmObsSearchItems; }
            set
            {
                SetProperty(ref ClsSynchronizer._vmObsSearchItems, value, nameof(ObsSearchItems));
            }
        }

        /// <summary>
        /// ItemSearch 查詢
        /// </summary>
        private ICommand _syncItemSearch { get; set; }
        public ICommand SyncItemSearch
        {
            get
            {
                _syncItemSearch = new RelayCommand((x) =>
                {

                    try
                    {
                        StartStopWait(true);
                        SyncItemTypeSearch();
                    }
                    catch (Exception ex)
                    {
                        StartStopWait(false);
                    }

                });
                return _syncItemSearch;
            }
        }

        private ICommand _doneSelecting { get; set; }
        public ICommand DoneSelecting
        {
            get
            {
                _doneSelecting = new RelayCommand((x) =>
                {

                    Window win;

                    if (_ItemSearchView == null) { MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected")); return; }
                    TextBox txtSelectedItemId = (TextBox)_ItemSearchView.FindName("selectedItemId");

                    ClsSynchronizer.SubDialogReturnValue = txtSelectedItemId.Text;
                    if (String.IsNullOrWhiteSpace(ClsSynchronizer.SubDialogReturnValue)) { MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected")); return; }
                    ClsSynchronizer.SubDialogReturnKeyedName = txtSelectedItemId.Tag.ToString();

                    int index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
                    win = (Window)ClsSynchronizer.SyncSubDialogView[index.ToString()];
                    win.Close();

                });
                return _doneSelecting;
            }
        }


        private ICommand _closeDialogWindow { get; set; }
        public ICommand CloseDialogWindow
        {
            get
            {
                _closeDialogWindow = new RelayCommand((x) =>
                {
                    Window win;

                    ClsSynchronizer.SubDialogReturnValue = "";
                    ClsSynchronizer.SubDialogReturnKeyedName = "";

                    int index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
                    win = (Window)ClsSynchronizer.SyncSubDialogView[index.ToString()];
                    win.Close();

                });
                return _closeDialogWindow;
            }
        }

        public ICommand ClearSearchConditions
        {
            get
            {
                _syncItemSearch = new RelayCommand((x) =>
                {
                    int index = (ClsSynchronizer.NewSubSearchItem == null) ? -1 : ClsSynchronizer.NewSubSearchItem.Count() - 1;
                    SearchItem searchItemType = ClsSynchronizer.NewSubSearchItem[index.ToString()] ;
                    _ItemSearchView = (ItemSearch)ClsSynchronizer.SyncSubListView[index.ToString()];

                    if (searchItemType == null || _ItemSearchView == null) return;
                    DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");
                    if (gridSelectedItems == null) return;
                    ClsSynchronizer.VmSyncCADs.ClearSearchConditions(gridSelectedItems, searchItemType);
                });
                return _syncItemSearch;
            }
        }


        
        #endregion " 

        #region "                   方法區

        public void ShowSearchDialog(string itemType)
        {
            ClsSynchronizer.SubDialogReturnValue = "";
            ClsSynchronizer.SubDialogReturnKeyedName = "";
            int index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
            LoadFromPLMView((Window)ClsSynchronizer.SyncSubDialogView[index.ToString()], itemType);
        }


        #endregion "

        #region "                   方法區(內部)


        private void LoadFromPLMView(Window win, string itemType)
        {
            if (String.IsNullOrWhiteSpace(itemType)) itemType = ItemTypeName.CAD.ToString();

            ItemType itemTypeItem = ClsSynchronizer.VmSyncCADs.GetItemType(itemType, SearchType.Search);
            ObsSearchItems = new ObservableCollection<SearchItem>();


            bool isNew = false;
            if (_ItemSearchView == null)
            {
                _ItemSearchView = new ItemSearch();
                _ItemSearchView.DataContext = new SubItemSearchDialogViewModel();
                isNew = true;
            }

            int index = ClsSynchronizer.SyncSubListView.Count() - 1;
            if (index > -1) ClsSynchronizer.SyncSubListView[index.ToString()] = _ItemSearchView;

            ((TextBox)_ItemSearchView.FindName("CADdirectory")).Visibility = Visibility.Hidden;
            ((Button)_ItemSearchView.FindName("selectedDirectory")).Visibility = Visibility.Hidden;

            DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");
            if (isNew) AddDataGridHeaderColumn(itemTypeItem, gridSelectedItems);


            SelectedSearchItem = new SearchItem();
            SelectedSearchItem.ItemType = itemType;


            ObservableCollection<PLMProperty> newProperties = new ObservableCollection<PLMProperty>();
            foreach (PLMProperty property in itemTypeItem.PlmProperties)
            {
                PLMProperty newProperty = property.Clone() as PLMProperty;
                if (String.IsNullOrWhiteSpace(newProperty.Name) == false)
                {
                    newProperty.IsInitial = false;
                    newProperty.IsExist = true;
                }
                newProperty.SoruceSearchItem = SelectedSearchItem;
                newProperties.Add(newProperty);
            }

            SelectedSearchItem.PlmProperties = newProperties;
            ObsSearchItems.Add(SelectedSearchItem);

            index = (ClsSynchronizer.NewSubSearchItem == null) ? -1 : ClsSynchronizer.NewSubSearchItem.Count() - 1;
            ClsSynchronizer.NewSubSearchItem[index.ToString()] = SelectedSearchItem;

            ClsSynchronizer.RowStyle = gridSelectedItems.RowStyle;
            gridSelectedItems.RowStyle = RowStyleHeightzero();

            var viewPage = (Frame)win.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_ItemSearchView);
        }


        private void AddDataGridHeaderColumn(ItemType itemTypeItem, DataGrid gridSelectedItems)
        {
            int j = 0;
            foreach (PLMProperty plmProperty in itemTypeItem.PlmProperties)
            {

                if (String.IsNullOrWhiteSpace(plmProperty.Label) == false)
                {
                    //要排除預設的屬性
                    ClsSynchronizer.VmCommon.AddDataGridHeaderColumn(gridSelectedItems, plmProperty, j);
                }
                j++;
            }
        }
        private Style RowStyleHeightzero()
        {
            Style style = new Style();
            style.Setters.Add(new Setter(property: FrameworkElement.HeightProperty, value: 0d));
            return style;
        }



        private void SyncItemTypeSearch()
        {

            int index = (ClsSynchronizer.NewSubSearchItem == null) ? -1 : ClsSynchronizer.NewSubSearchItem.Count() - 1;
            SearchItem searchItemType = ClsSynchronizer.NewSubSearchItem[index.ToString()];

            UpdateSearchItemType(searchItemType);
            bool ret = false;
            var task = Task.Factory.StartNew(() => ret = SyncItemTypeSearch(searchItemType));
            task.ContinueWith((y) =>
            {

                if (ObsSearchItems == null) ObsSearchItems = new ObservableCollection<SearchItem>();
                ClsSynchronizer.SearchItemsCollection = ObsSearchItems;

                _ItemSearchView = (ItemSearch)ClsSynchronizer.SyncSubListView[index.ToString()];
                if (_ItemSearchView == null) _ItemSearchView = new ItemSearch();
                DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");

                StartStopWait(false);

                if (ObsSearchItems.Count == 0)
                {
                    LoadFromPLMView((Window)ClsSynchronizer.SyncSubDialogView[index.ToString()], ClsSynchronizer.ShowDialogItemType);
                    return;
                }
                else
                    gridSelectedItems.RowStyle = ClsSynchronizer.RowStyle;

                gridSelectedItems.ItemsSource = ObsSearchItems;
                gridSelectedItems.RowStyle = ClsSynchronizer.RowStyle;

                index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
                Window win = (Window)ClsSynchronizer.SyncSubDialogView[index.ToString()];

                Frame viewPage = (Frame)win.FindName("viewPage");
                viewPage.Visibility = Visibility.Visible;
                viewPage.Navigate(_ItemSearchView);

            }, TaskScheduler.FromCurrentSynchronizationContext());

        }


        private bool SyncItemTypeSearch(SearchItem searchItemType)
        {

            ObsSearchItems = ClsSynchronizer.VmSyncCADs.SyncItemTypeSearch(searchItemType);
            return true;
        }

        private void UpdateSearchItemType(SearchItem searchItemType)
        {
            int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
            _ItemSearchView = (ItemSearch)ClsSynchronizer.SyncSubListView[index.ToString()];
            if (_ItemSearchView == null) _ItemSearchView = new ItemSearch();

            DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");
            ClsSynchronizer.VmSyncCADs.UpdateSearchItemType(gridSelectedItems, searchItemType);

        }

        private void StartStopWait(bool isStart)
        {
            if (isStart) ClsSynchronizer.VmSyncCADs.IsActiveCAD();
            int index = ClsSynchronizer.SyncSubDialogLoadingAdorner.Count() - 1;
            AdornedControl.AdornedControl loadingAdorner = ClsSynchronizer.SyncSubDialogLoadingAdorner[index.ToString()];
            loadingAdorner.IsAdornerVisible = isStart;
            loadingAdorner.Visibility = (loadingAdorner.IsAdornerVisible) ? Visibility.Visible : Visibility.Collapsed;

        }

        #endregion

    }
}
