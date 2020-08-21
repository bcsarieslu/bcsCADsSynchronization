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
#endregion

namespace BCS.CADs.Synchronization.ViewModels
{
    public class ItemSearchDialogViewModel : NotifyPropertyBase
    {
        #region "                   宣告區
        private ItemSearch _ItemSearchView = null;
        #endregion


        #region "                   進入區
        public ItemSearchDialogViewModel()
        {

        }
        #endregion " 

        #region "                   屬性區
        public dynamic SetView
        {
            set
            {

                ClsSynchronizer.SyncDialogView = (dynamic)value;
            }

        }

        // <summary>
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


        #endregion " 

        #region "                   方法區
        public void ShowSearchDialog(string itemType)
        {
            ClsSynchronizer.DialogReturnValue = "";
            ClsSynchronizer.DialogReturnKeyedName = "";
            LoadFromPLMView((Window)ClsSynchronizer.SyncDialogView, itemType);
        }
        #endregion


        #region "                   方法區(內部)
        //public void LoadFromPLMView(Window win, bool isDialog, string itemType, bool isSub, bool isCopyToAdd)
        public void LoadFromPLMView(Window win, string itemType)
        {
            if (String.IsNullOrWhiteSpace(itemType)) itemType = ItemTypeName.CAD.ToString();
            ItemType itemTypeItem = ClsSynchronizer.VmSyncCADs.GetItemType(itemType, SearchType.Search);
            ObsSearchItems = new ObservableCollection<SearchItem>();
            
            bool isNew = false;
            if (_ItemSearchView == null)
            {
                _ItemSearchView = new ItemSearch();
                _ItemSearchView.DataContext = new ItemSearchDialogViewModel();
                isNew = true;
            }

            ClsSynchronizer.SyncListView = _ItemSearchView;
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
            ClsSynchronizer.NewSearchItem = SelectedSearchItem;
            ClsSynchronizer.RowStyle = gridSelectedItems.RowStyle;
            gridSelectedItems.RowStyle = RowStyleHeightzero();


            //DefaultSystemItemsDisplay();

            //ClsSynchronizer.ActiveWindow = (isDialog == true) ? win : MyMainWindow;
            //var viewPage = (isDialog == true) ? (Frame)win.FindName("viewPage") : (Frame)MyMainWindow.FindName("viewPage");
            //viewPage.Visibility = Visibility.Visible;
            //viewPage.Navigate(_ItemSearchView);


            ClsSynchronizer.VmMessages.Status = "End";
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

        /*
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
        */
        #endregion " 

    }
    
}
