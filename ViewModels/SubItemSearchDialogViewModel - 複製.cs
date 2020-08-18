#region "                   名稱空間"
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BCS.CADs.Synchronization.Models;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


#endregion

namespace BCS.CADs.Synchronization.ViewModels
{
    public class SubItemSearchDialogViewModel : NotifyPropertyBase
    {

        #region "                   宣告區
            private ItemSearch _ItemSearchView = null;
            private dynamic _view;// SubItemSearchDialog _view;
        #endregion

        #region "                   進入區
        public SubItemSearchDialogViewModel()
        {

        }

        //public SubItemSearchDialogViewModel(SubItemSearchDialog view, string itemType)
        //{

        //    _vmMain = new MainWindowViewModel(view);
        //    _view = view;
        //    ClsSynchronizer.SubDialogReturnValue = "";
        //    ClsSynchronizer.SubDialogReturnKeyedName = "";
        //    ClsSynchronizer.SyncSubDialogView = _view;
        //    LoadFromPLMView((Window)_view, true, itemType, false);
        //}


        //public SubItemSearchDialogViewModel(dynamic view, string itemType, bool isSub)
        //{
        //    _view = view;

        //    //_vmMain = new MainWindowViewModel(view);

        //    ClsSynchronizer.IsActiveSubDialogView = isSub;
        //    if (isSub)
        //    {
        //        ClsSynchronizer.SubDialogReturnValue = "";
        //        ClsSynchronizer.SubDialogReturnKeyedName = "";
        //        ClsSynchronizer.SyncSubDialogView = _view;
        //    }
        //    else
        //    {
        //        ClsSynchronizer.DialogReturnValue = "";
        //        ClsSynchronizer.DialogReturnKeyedName = "";
        //        ClsSynchronizer.SyncDialogView = _view;
        //    }
        //    CreateConnPLM();
        //    LoadFromPLMView((Window)_view, true, itemType, isSub, false);
        //}


        #endregion

        #region "                   屬性區


        public dynamic SetView
        {
            set
            {
                ClsSynchronizer.SyncSubDialogView = (dynamic)value;
                _view = ClsSynchronizer.SyncSubDialogView;

            }
        }

        /// <summary>
        /// 被選到的項目
        /// </summary>
        private SearchItem _selectedSearchItem;
        public SearchItem SelectedSearchItem
        {
            get { return _selectedSearchItem; }
            //set { SetProperty(ref _selectedListItem, value); }
            set { SetProperty(ref _selectedSearchItem, value, nameof(SelectedSearchItem)); }
        }

        public ObservableCollection<SearchItem> ObsSearchItems
        {
            get { return ClsSynchronizer._vmObsSearchItems; }
            set
            {
                //SetProperty(ref ClsSynchronizer._vmObsSearchItems, value);
                SetProperty(ref ClsSynchronizer._vmObsSearchItems, value, nameof(ObsSearchItems));
            }
        }

        #endregion

        #region "                   方法區

        public void ShowSearchDialog(string itemType)
        {
            ClsSynchronizer.SubDialogReturnValue = "";
            ClsSynchronizer.SubDialogReturnKeyedName = "";
            LoadFromPLMView((Window)_view, true, itemType, false);
        }





        private void LoadFromPLMView(Window win, bool isDialog, string itemType, bool isCopyToAdd)
        {
            if (String.IsNullOrWhiteSpace(itemType)) itemType = "CAD";

            string displayName = "LoadFromPLMView";
            //string displayKey = "loadFromPLM";
            //_vmMain.OperationStart(displayName);

            ItemType itemTypeItem = ClsSynchronizer.VmSyncCADs.GetItemType(itemType, true);

            ObsSearchItems = new ObservableCollection<SearchItem>();
            if (_ItemSearchView == null) _ItemSearchView = new ItemSearch();

            ClsSynchronizer.SyncSubListView = _ItemSearchView;
            ((TextBox)_ItemSearchView.FindName("CADdirectory")).Visibility = Visibility.Hidden;
            ((Button)_ItemSearchView.FindName("selectedDirectory")).Visibility = Visibility.Hidden;

            DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");

            int j = 0;
            foreach (PLMProperties plmProperty in itemTypeItem.CsProperties)
            {

                //if (plmProperty.Label != "")
                if (String.IsNullOrWhiteSpace(plmProperty.Label) == false)
                {
                    //要排除預設的屬性
                    //_vmMain.AddDataGridColumn(gridSelectedItems, plmProperty, j);
                }
                j++;
            }

            SelectedSearchItem = new SearchItem();
            SelectedSearchItem.ClassName = itemType;


            ObservableCollection<PLMProperties> newProperties = new ObservableCollection<PLMProperties>();
            foreach (PLMProperties property in itemTypeItem.CsProperties)
            {
                PLMProperties newProperty = property.Clone() as PLMProperties;
                // if (newProperty.Name != "")
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
            ObsSearchItems.Move(ObsSearchItems.IndexOf(SelectedSearchItem), 0);

            if (ClsSynchronizer.IsActiveSubDialogView)
                ClsSynchronizer.NewSubSearchItem = SelectedSearchItem;
            else
                ClsSynchronizer.NewSearchItem = SelectedSearchItem;

            gridSelectedItems.ItemsSource = ObsSearchItems;


            //_vmMain.DefaultSystemItemsDisplay();


            ClsSynchronizer.ActiveWindow = win;
            var viewPage = (Frame)win.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_ItemSearchView);
            ClsSynchronizer.VmMessages.Status = "End";
        }

        //public void LoadFromPLMView(Window win, bool isDialog, string itemType, bool isSub, bool isCopyToAdd)
        //{
        //    //MainWindow = win;
        //    //MessageBox.Show("LoadFromPLMView");

        //    //if (itemType == "") itemType = "CAD";
        //    if (String.IsNullOrWhiteSpace(itemType)) itemType = "CAD";

        //    string displayName = isCopyToAdd ? "CopyToAddView" : "LoadFromPLMView";
        //    string displayKey = isCopyToAdd ? "copyToAdd" : "loadFromPLM";
        //    OperationStart(displayName);
        //    //OperationStart("LoadFromPLMView");

        //    ItemType itemTypeItem = ClsSynchronizer.VmSyncCADs.GetItemType(itemType, true);
        //    //List<SearchItem> searchItems = null;

        //    ObsSearchItems = new ObservableCollection<SearchItem>();

        //    if (_ItemSearchView == null) _ItemSearchView = new ItemSearch();

        //    if (ClsSynchronizer.IsActiveSubDialogView)
        //        ClsSynchronizer.SyncSubListView = _ItemSearchView;
        //    else
        //        ClsSynchronizer.SyncListView = _ItemSearchView;

        //    if (isDialog == true)
        //    {
        //        ((TextBox)_ItemSearchView.FindName("CADdirectory")).Visibility = Visibility.Hidden;
        //        ((Button)_ItemSearchView.FindName("selectedDirectory")).Visibility = Visibility.Hidden;
        //    }
        //    else
        //    {
        //        ((Button)_ItemSearchView.FindName("doneSelecting")).Visibility = Visibility.Hidden;
        //        ((Button)_ItemSearchView.FindName("closeDialogWindow")).Visibility = Visibility.Hidden;

        //    }


        //    DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");

        //    int j = 0;
        //    foreach (PLMProperties plmProperty in itemTypeItem.CsProperties)
        //    {

        //        //if (plmProperty.Label != "")
        //        if (String.IsNullOrWhiteSpace(plmProperty.Label) == false)
        //        {
        //            //要排除預設的屬性
        //            AddDataGridColumn(gridSelectedItems, plmProperty, j);
        //        }
        //        j++;
        //    }

        //    //ClsSynchronizer.NewSearchItem = new SearchItem();
        //    //ClsSynchronizer.NewSearchItem.ClassName = "CAD";
        //    SelectedSearchItem = new SearchItem();
        //    SelectedSearchItem.ClassName = itemType;


        //    ObservableCollection<PLMProperties> newProperties = new ObservableCollection<PLMProperties>();
        //    foreach (PLMProperties property in itemTypeItem.CsProperties)
        //    {
        //        PLMProperties newProperty = property.Clone() as PLMProperties;
        //        // if (newProperty.Name != "")
        //        if (String.IsNullOrWhiteSpace(newProperty.Name) == false)
        //        {
        //            newProperty.IsInitial = false;
        //            newProperty.IsExist = true;
        //        }
        //        newProperty.SoruceSearchItem = SelectedSearchItem;
        //        newProperties.Add(newProperty);
        //    }

        //    SelectedSearchItem.PlmProperties = newProperties;
        //    ObsSearchItems.Add(SelectedSearchItem);
        //    ObsSearchItems.Move(ObsSearchItems.IndexOf(SelectedSearchItem), 0);

        //    if (ClsSynchronizer.IsActiveSubDialogView)
        //        ClsSynchronizer.NewSubSearchItem = SelectedSearchItem;
        //    else
        //        ClsSynchronizer.NewSearchItem = SelectedSearchItem;

        //    gridSelectedItems.ItemsSource = ObsSearchItems;

        //    if (isDialog == false)
        //    {
        //        Button btn = (Button)MyMainWindow.FindName(displayKey);
        //        SetMenuButtonColor(displayKey);
        //        Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
        //        rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName(String.Format("{0}_SP", displayKey))));
        //    }

        //    DefaultSystemItemsDisplay();


        //    ClsSynchronizer.ActiveWindow = (isDialog == true) ? win : MyMainWindow;
        //    var viewPage = (isDialog == true) ? (Frame)win.FindName("viewPage") : (Frame)MyMainWindow.FindName("viewPage");
        //    viewPage.Visibility = Visibility.Visible;
        //    viewPage.Navigate(_ItemSearchView);


        //    ClsSynchronizer.VmMessages.Status = "End";
        //}
        #endregion


    }


}
