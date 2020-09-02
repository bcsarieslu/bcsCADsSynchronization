#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endregion


namespace BCS.CADs.Synchronization.ViewModels
{
    public class ItemStructureViewModel : NotifyPropertyBase
    {
        #region "                   宣告區

        #endregion

        #region "                   進入區
        public ItemStructureViewModel()
        {

        }

        #endregion

        #region "                   屬性區

        public dynamic SetView
        {
            set
            {
                ClsSynchronizer.SyncCommonTreeView = (dynamic)value;
            }
        }


        private ICommand _closeDialogWindow { get; set; }
        public ICommand CloseDialogWindow
        {
            get
            {
                _closeDialogWindow = new RelayCommand((x) =>
                {
                    Window win = (Window)ClsSynchronizer.SyncCommonTreeView;
                    win.Close();

                });
                return _closeDialogWindow;
            }
        }


        private ICommand _done { get; set; }
        public ICommand Done
        {
            get
            {
                _done = new RelayCommand((x) =>
                {

                    Window win = (Window)ClsSynchronizer.SyncCommonTreeView;

                    TextBox txtSelectedItemId = (TextBox)win.FindName("selectedItemId");
                    //System.Diagnostics.Debugger.Break();
                    if (String.IsNullOrWhiteSpace(txtSelectedItemId.Tag.ToString()))
                    {
                        MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoItemSelected")); return;
                    }

                    //ClsSynchronizer.DialogReturnValue = txtSelectedItemId.Tag.ToString();
                    //ClsSynchronizer.DialogReturnDisplayValue = txtSelectedItemId.Text;
                    win.Close();

                });
                return _done;
            }
        }


        private ObservableCollection<ItemStructure> _classStructureItems;
        public ObservableCollection<ItemStructure> ClassStructureItems
        {
            get { return _classStructureItems; }
            set { SetProperty(ref _classStructureItems, value, nameof(ClassStructureItems)); }
        }


        //TreeStructureView
        /// <summary>
        /// Tree Structure View
        /// </summary>
        //private ICommand _treeStructureView { get; set; }
        //public ICommand TreeStructureView
        //{
        //    get
        //    {

        //        _treeStructureView = new RelayCommand((x) =>
        //        {

        //            ClsSynchronizer.VmOperation = SyncOperation.CADStructure;

        //            if (_treeStructure == null) _treeStructure = new TreeStructure();
        //            ShowTreeStructure(_treeStructure);

        //            var viewPage = (Frame)MyMainWindow.FindName("viewPage");

        //            CheckBoxWPVisibility = Visibility.Collapsed;
        //            viewPage.Visibility = Visibility.Visible;
        //            viewPage.Navigate(_treeStructure);

        //            ClsSynchronizer.VmMessages.Status = "End";
        //        });
        //        return _treeStructureView;

        //    }
        //}



        #endregion

        #region "                   方法區

        public void ShowSearchItems(string itemType, string id)
        {
            SearchItem searchItem = ClsSynchronizer.VmSyncCADs.GetPLMSearchItem(itemType, id);
            searchItem.IsActiveCAD = true;
            List<SearchItem> searchItems =new List<SearchItem>();
            searchItems.Add(searchItem);
            ClsSynchronizer.VmSyncCADs.GetPLMItemsStructure(searchItems, SyncType.Structure);
            ShowTreeStructure(searchItem);


        }

        #endregion

        #region "                   方法區 (內部)


        


        //private void ShowTreeStructure(TreeStructure treeStructure)
        private void ShowTreeStructure(SearchItem activeItem)
        {

            //SearchItem activeItem = ClsSynchronizer.VmSyncCADs.GetActiveSearchItem(_searchItems);

            string filename = "";
            if (activeItem != null)
                filename = activeItem.FileName;


            ClassStructureItems = new ObservableCollection<ItemStructure>();

            ItemStructure treeSearchItem = new ItemStructure();
            treeSearchItem.Name = activeItem.FileName;
            treeSearchItem.NodeSearchItem = activeItem;
            treeSearchItem.ClassName = activeItem.ClassName;
            treeSearchItem.ClassThumbnail = activeItem.ClassThumbnail;
            treeSearchItem.RestrictedStatus = activeItem.RestrictedStatus;
            treeSearchItem.VersionStatus = activeItem.VersionStatus;
            treeSearchItem.AccessRights = activeItem.AccessRights;
            treeSearchItem.Thumbnail = activeItem.Thumbnail;// ClsSynchronizer.VmSyncCADs.GetImageFullName(searchItem.Thumbnail); 
            //treeSearchItem.IsChecked = searchItem.IsViewSelected;
            treeSearchItem.DisplayName = System.IO.Path.GetFileNameWithoutExtension(activeItem.FileName);
            ClassStructureItems.Add(treeSearchItem);


            //ClsSynchronizer.TreeStructureView = treeStructure;
            //ClsSynchronizer.TreeSearchItemsCollection = TreeSearchItems;

            Window win = (Window)ClsSynchronizer.SyncCommonTreeView;
            TreeView treeView = (TreeView)win.FindName("treeStructureItems");
            treeView.ItemsSource = ClassStructureItems;
        }




        #endregion
    }

    public class ItemStructure : NotifyPropertyBase
    {

        private ObservableCollection<ItemStructure> _child;
        public ObservableCollection<ItemStructure> Child
        {
            get { return _child; }
            set { SetProperty(ref _child, value, nameof(Child)); }

        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value, nameof(Name));
                _displayName = _name;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value, nameof(DisplayName)); }
        }

        private string _className;
        public string ClassName
        {
            get { return _className; }
            set { SetProperty(ref _className, value, nameof(ClassName)); }
        }

        //1111:(8,4,2,1) :InsertSaveAs,CopyToAdd,Replace,Insert
        private int _operationType;
        public int OperationType
        {
            get { return _operationType; }
            set { SetProperty(ref _operationType, value, nameof(OperationType)); }
        }


        private string _classThumbnail;
        public string ClassThumbnail
        {
            get { return _classThumbnail; }
            set { SetProperty(ref _classThumbnail, value, nameof(ClassThumbnail)); }
        }


        private string _thumbnail;
        public string Thumbnail
        {
            get { return _thumbnail; }
            set { SetProperty(ref _thumbnail, value, nameof(Thumbnail)); }
        }


        private string _restrictedStatus;
        public string RestrictedStatus
        {
            get { return _restrictedStatus; }
            set { SetProperty(ref _restrictedStatus, value, nameof(RestrictedStatus)); }
        }

        private string _versionStatus;
        public string VersionStatus
        {
            get { return _versionStatus; }
            set { SetProperty(ref _versionStatus, value, nameof(VersionStatus)); }
        }

        private string _accessRights;
        public string AccessRights
        {
            get { return _accessRights; }
            set { SetProperty(ref _accessRights, value, nameof(AccessRights)); }
        }


        private int _order;
        public int Order
        {
            get { return _order; }
            set { SetProperty(ref _order, value, nameof(Order)); }
        }

        /// <summary>
        /// 副本Id
        /// </summary>
        private string _instanceId;
        public string InstanceId
        {
            get { return _instanceId; }
            set { SetProperty(ref _instanceId, value, nameof(InstanceId)); }
        }


        private Boolean _isInsert = true;//false;
        public Boolean IsInsert
        {
            get { return _isInsert; }
            //set { SetProperty(ref _order, value); }
            set { SetProperty(ref _isInsert, value, nameof(IsInsert)); }
        }

        private Boolean _isReplacement = true;
        public Boolean IsReplacement
        {
            get { return _isReplacement; }
            //set { SetProperty(ref _order, value); }
            set { SetProperty(ref _isReplacement, value, nameof(IsReplacement)); }
        }

        private Boolean _isCopyToAdd = true;
        public Boolean IsCopyToAdd
        {
            get { return _isCopyToAdd; }
            set { SetProperty(ref _isCopyToAdd, value, nameof(IsCopyToAdd)); }
        }

        private Boolean _isInsertSaveAs = true;//false;
        public Boolean IsInsertSaveAs
        {
            get { return _isInsertSaveAs; }
            set { SetProperty(ref _isInsertSaveAs, value, nameof(IsInsertSaveAs)); }
        }


        private SearchItem _nodeSearchItem;

        public SearchItem NodeSearchItem
        {
            get { return _nodeSearchItem; }
            set
            {
                //SetProperty(ref _searchItem, value);
                SetProperty(ref _nodeSearchItem, value, nameof(NodeSearchItem));
                foreach (CADStructure cadStructure in NodeSearchItem.CadStructure)
                {
                    ItemStructure sonSearchItem = new ItemStructure();
                    sonSearchItem.Name = cadStructure.Child.FileName;

                    sonSearchItem.Order = cadStructure.Order;
                    sonSearchItem.InstanceId = cadStructure.InstanceId;

                    PLMProperty property = cadStructure.Child.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) == false).SingleOrDefault();
                    string displayName = (property != null) ? property.DisplayValue : System.IO.Path.GetFileNameWithoutExtension(cadStructure.Child.FileName);
                    sonSearchItem.DisplayName = (sonSearchItem.InstanceId != "") ? String.Format(displayName + "<{0}>", sonSearchItem.InstanceId) : displayName;

                    sonSearchItem.ClassName = cadStructure.Child.ClassName;
                    sonSearchItem.ClassThumbnail = cadStructure.Child.ClassThumbnail;
                    sonSearchItem.RestrictedStatus = cadStructure.Child.RestrictedStatus;
                    sonSearchItem.VersionStatus = cadStructure.Child.VersionStatus;
                    sonSearchItem.AccessRights = cadStructure.Child.AccessRights;
                    sonSearchItem.Thumbnail = cadStructure.Child.Thumbnail;// ClsSynchronizer.VmSyncCADs.GetImageFullName(cadStructure.Child.Thumbnail);

                    sonSearchItem.OperationType = cadStructure.OperationType;

                    //sonSearchItem.IsChecked = cadStructure.Child.IsViewSelected;
                    //if (sonSearchItem.IsChecked == true && sonSearchItem.OperationType == 0)
                    //{
                    //    sonSearchItem.IsReplacement = true;
                    //    sonSearchItem.IsCopyToAdd = true;
                    //    if (sonSearchItem.ClassName != "Assembly") { sonSearchItem.IsInsert = false; sonSearchItem.IsInsertSaveAs = false; }
                    //}
                    //else
                    //{
                    //    sonSearchItem.IsInsert = false;
                    //    sonSearchItem.IsInsertSaveAs = false;
                    //    sonSearchItem.IsReplacement = false;
                    //    sonSearchItem.IsCopyToAdd = false;
                    //}

                    //if (sonSearchItem.IsChecked == true)
                    //{

                    //    property = cadStructure.Child.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) == false).SingleOrDefault();
                    //    if (property != null)
                    //    {

                    //        sonSearchItem.DisplayName = (String.IsNullOrWhiteSpace(sonSearchItem.InstanceId) == false) ? String.Format(property.DisplayValue + "<{0}>", sonSearchItem.InstanceId) : property.DisplayValue;
                    //        sonSearchItem.OperationType = 4;
                    //    }
                    //}


                    Child.Add(sonSearchItem);
                    sonSearchItem.NodeSearchItem = cadStructure.Child;
                }

            }
        }



        bool? _isChecked = false;
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, false, false); }
            //set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
            {
                Child.ToList().ForEach(c => c.SetIsChecked(_isChecked, true, false));
            }

            this.OnPropertyChanged("IsChecked");
        }

        public ItemStructure()
        {
            Child = new ObservableCollection<ItemStructure>();
        }
    }


}
