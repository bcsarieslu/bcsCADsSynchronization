using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Entities;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.Services;
using BCS.CADs.Synchronization.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


using AdornedControl;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Controls.Primitives;

namespace BCS.CADs.Synchronization.ViewModels
{
    public class ViewModelBase 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class MainWindowViewModel : NotifyPropertyBase
    {

        #region "                   宣告區
        private AddFromTemplate _addFromTemplateView = null;
        private ItemsMessage _addItemsMessageView = null;

        private ItemFilterSearch _itemFilterSearchView = null;
        private SyncCADsList _syncCADsListDataGrid = null;
        private EditProperties _editPropertiesDataGrid = null;
        private TreeStructure _treeStructure = null;
        private PlugInFuncs _plugInFuncsView = null;
        private ItemSearch _ItemSearchView = null;
        private PartsLibrarySearch _partsLibrarySearch = null;

        private RecentFileViewModel recentFileVM=null;
        public RecentFileViewModel RecentFileVM
        {
            get
            {
                if (recentFileVM == null)
                {
                    return new RecentFileViewModel();
                }
                return recentFileVM;
            }
        }

        private MainWindow _myMainWindow;
        public MainWindow MyMainWindow { get { return (MainWindow)MyCache.CacheInstance["MainWindow"]; } set { _myMainWindow = value; } }

        private ConnPLM _Plm;

        private readonly Window _view;
        public MainWindowViewModel()
        {
            CreateConnPLM();
        }

        public MainWindowViewModel(Window view)
        {
            _view = view;
        }

        public MainWindowViewModel(Window view,string itemType,bool isSub)
        {
            _view = view;

            ClsSynchronizer.IsActiveSubDialogView = isSub;
            if (isSub) { 
                ClsSynchronizer.SubDialogReturnValue = "";
                ClsSynchronizer.SubDialogReturnKeyedName = "";

                int index = ClsSynchronizer.SyncSubDialogView.Count()-1;
                ClsSynchronizer.SyncSubDialogView[index.ToString()] =_view;
            }
            else { 
                ClsSynchronizer.DialogReturnValue = "";
                ClsSynchronizer.DialogReturnKeyedName = "";
                ClsSynchronizer.SyncDialogView = _view;
            }
            CreateConnPLM();
            LoadFromPLMView((Window)_view, true, itemType, isSub,false);
            
        }


        public ICommand GridFieldClickedCommand
        {
            get
            {
                
                _showCommand = _showCommand ?? new RelayCommand((x) =>
                {
                    SearchItem searchItem = x as SearchItem;
                    if (searchItem != null)
                    {
                        ThumbnailImage thumbnailImage = new ThumbnailImage();
                        thumbnailImage.GetThumbnailImagePath(searchItem);

                        Canvas canvas = (Canvas)MyMainWindow.FindName("CanvasViewFile");
                        TextBlock positionUse = (TextBlock)MyMainWindow.FindName("positionUse");

                        if (ClsSynchronizer.IsActiveSubDialogView == true)
                        {
                            //Window win = (Window)ClsSynchronizer.SyncSubDialogView;
                            int index = ClsSynchronizer.SyncSubDialogView.Count()-1;
                            Window win = (Window)ClsSynchronizer.SyncSubDialogView[index.ToString()];

                            canvas = ((Canvas)(win.FindName("SubDialogCanvasViewFile")) != null) ? (Canvas)(win.FindName("SubDialogCanvasViewFile")) : canvas;
                            positionUse = ((Canvas)(win.FindName("SubDialogCanvasViewFile")) != null) ? (TextBlock)win.FindName("positionUse") : positionUse;
                        }
                        else
                        {
                            if (ClsSynchronizer.SyncDialogView != null)
                            {
                                Window win = (Window)ClsSynchronizer.SyncDialogView;
                                canvas = ((Canvas)(win.FindName("DialogCanvasViewFile")) != null) ? (Canvas)(win.FindName("DialogCanvasViewFile")) : canvas;
                                positionUse = ((Canvas)(win.FindName("DialogCanvasViewFile")) != null) ? (TextBlock)win.FindName("positionUse") : positionUse;
                            }
                        }


                        canvas.Visibility = Visibility.Visible;
                        ViewFile viewFile = (ViewFile)canvas.FindName("viewFile");
                       
                        Label lb = (Label)MyMainWindow.FindName("scaleLabel");
                        Point pointToWindow = Mouse.GetPosition(positionUse);

                        double size = (double.Parse(lb.Content.ToString()) / 100);
                        if (ClsSynchronizer.CurrentDialog != "ItemSearchDialog" && ClsSynchronizer.IsActiveSubDialogView == false)
                        {                 
                            Canvas.SetLeft(viewFile, pointToWindow.X - (55 * size));
                            Canvas.SetTop(viewFile, pointToWindow.Y - (48 * size));
                        }else
                        {
                            Canvas.SetLeft(viewFile, pointToWindow.X + (350 * size));
                            Canvas.SetTop(viewFile, pointToWindow.Y - (250 * size));
                        }

                        if (viewFile != null)
                        {
                            Image image = (Image)viewFile.FindName("imageFile");
                            if (image != null) image.Source = new BitmapImage(new Uri(ClsSynchronizer.ViewFilePath));
                        }
                        Storyboard myStoryboard = new Storyboard();
                        myStoryboard.Children.Add((Storyboard)MyMainWindow.Resources["showMe"]);
                        myStoryboard.Begin(viewFile);
                    }
                });


                return _showCommand;
            }
        }

        private decimal _setScale = 1;
        public decimal SetScale
        {
            get { return _setScale; }
            set
            {
                SetProperty(ref _setScale, value, "SetScale");
            }
        }

        private string _scaleValue = "100";
        public string ScaleValue
        {
            get { return _scaleValue; }
            set
            {
                SetProperty(ref _scaleValue, value, "ScaleValue");
            }
        }

        private ICommand _subScale;
        public ICommand SubScale
        {
            get
            {
                _subScale = new RelayCommand(
                    x =>
                    {
                        decimal size = decimal.Parse(ScaleValue);

                        size = size - 10;
                        ScaleValue = size.ToString();
                        size /= 100;
                        SetScale = size;
                    });
                return _subScale;
            }
        }

        private ICommand _addScale;
        public ICommand AddScale
        {
            get
            {
                _addScale = new RelayCommand(
                    x =>
                    {
                        decimal size = decimal.Parse(ScaleValue);

                        size = size + 10;
                        ScaleValue = size.ToString();
                        size /= 100;
                        SetScale = size;
                    });
                return _addScale;
            }
        }

        private ICommand _resetScale;
        public ICommand ResetScale
        {
            get
            {
                _resetScale = new RelayCommand(
                    x =>
                    {
                        SetScale = 1;
                        ScaleValue = "100";
                    });
                return _resetScale;
            }
        }
        
        private ICommand _refushUserInformation;
        public ICommand RefushUserInformation
        {
            get
            {
                _refushUserInformation = new RelayCommand(
                    x =>
                    {
                        if(ClsSynchronizer.UserImageSource!=null)
                            UserToggleButtonSource = ClsSynchronizer.UserImageSource;
                    });
                return _refushUserInformation;
            }
        }

        private string _userToggleButtonSource = "pack://application:,,,/BCS.CADs.Synchronization;component/images/DefaultAvatar.png";
        public string UserToggleButtonSource
        {
            get { return _userToggleButtonSource; }
            set
            {
                SetProperty(ref _userToggleButtonSource, value, "UserToggleButtonSource");
            }
        }

        private Visibility _showLine;
        public Visibility ShowLine
        {
            get { return _showLine; }
            set
            {
                SetProperty(ref _showLine, value, "ShowLine");
                OnPropertyChanged("ShowLine");
            }
        }

        public ConnPLM PLM
        {
            get { return _Plm; }
            set { SetProperty(ref _Plm, value, nameof(PLM)); }
        }

        private ICommand _showCommand;

        public List<SearchItem> _searchItems;
        #endregion

        /// <summary>
        /// CAD軟體名稱
        /// </summary>
        public string CADSoftware { get; set; }

        /// <summary>
        /// 使用者是否登錄
        /// </summary>
        public bool IsActiveLogin { get; set; } = false;

        public List<SearchItem> GetSearchItems
        {
            get
            {
                return _searchItems;
            }
        }
        public List<SearchItem> ListSearchItems
        {
            get { return _searchItems; }
            set
            {
                SetProperty(ref _searchItems, value, nameof(ListSearchItems));
                
            }
        }
        private LanguageList _selectedLang;
        public LanguageList SelectedLang
        {
            get { return _selectedLang; }
            set { SetProperty(ref _selectedLang, value); }
        }

        public int changeview { get; set; }

        public ICommand RemoveCommand { get; }

        private void CreateConnPLM()
        {
            _Plm = new ConnPLM();

        }

        //private ICommand _leftButtonDownCommand;

        //public ICommand LeftMouseButtonDown
        //{
        //    get
        //    {
        //        return _leftButtonDownCommand ?? (_leftButtonDownCommand = new RelayCommand(
        //           x =>
        //           {
        //               MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_responding_left_mouse_event"));
        //           }));
        //    }
        //}

        private bool _checkAllIsCheck = false;
        public bool CheckAllIsCheck
        {
            get { return _checkAllIsCheck; }
            set
            {
                SetProperty(ref _checkAllIsCheck, value, "CheckAllIsCheck");
            }
        }
        
        private Visibility _checkBoxWPVisibility = Visibility.Collapsed;
        public Visibility CheckBoxWPVisibility
        {
            get { return _checkBoxWPVisibility; }
            set
            {
                SetProperty(ref _checkBoxWPVisibility, value, "CheckBoxWPVisibility");
            }
        }

        private Visibility _selectedDirectoryGridVisibility = Visibility.Collapsed;
        public Visibility SelectedDirectoryGridVisibility
        {
            get { return _selectedDirectoryGridVisibility; }
            set
            {
                SetProperty(ref _selectedDirectoryGridVisibility, value, "SelectedDirectoryGridVisibility");
            }
        }


        private bool _URLConnection;
        public bool URLConnection
        {
            get { return _URLConnection; }
            set
            {
                try
                {
                    SetProperty(ref _URLConnection, value);
                    if (value)
                    {
                        if (PLM.ListItems.Count == 0)
                        {
                            StartStopWait(true);
                            ResetDBList();
                        }
                    }
                   
                }
                catch(Exception ex)
                {
                    string message = ex.Message;
                    StartStopWait(false);
                }
            }
        }

        public ICommand ShowCommand
        {
            get
            {

                _showCommand = _showCommand ?? new RelayCommand((x) =>
                {
                    var button = x as Button;
                    var parent = VisualTreeHelper.GetParent(button);

                    RecentFile rf = new RecentFile();
                    //rf.AddRecentFile("test001","");
                    var s = rf.ReadRecentFile();

                    //讀取最近前10筆的資料
                    RecentFileVM.RecentFile = rf.ReadRecentFile();
                    //資料來源修改時，重置資料
                    OnPropertyChanged("RecentFileVM");

                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Collapsed;
                    ResetOperationButtons(Visibility.Visible, Visibility.Visible , Visibility.Visible, Visibility.Visible, false );
                    Frame viewPage = (Frame)MyMainWindow.FindName("viewPage");
                    if (viewPage != null) viewPage.Visibility = Visibility.Hidden;
                    if (button != null)
                    {
                        //StartStopWait(true);
                        switch (button.Name)
                        {
                            case "showLogin":
                                Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
                                rect.SetValue(Grid.RowProperty, Grid.GetRow(button));
                                ShowLogIn();
                                break;                     
                        }
                        //StartStopWait(false);
                    }

                });


                return _showCommand;
            }
        }


        private void StartStopWait(bool isStart)
        {
            if (isStart) ClsSynchronizer.VmSyncCADs.IsActiveCAD();

            int index = ClsSynchronizer.SyncSubDialogLoadingAdorner.Count() - 1;
            AdornedControl.AdornedControl loadingAdorner =(ClsSynchronizer.IsShowDialog==true)?((index > -1)) ? ClsSynchronizer.SyncSubDialogLoadingAdorner[index.ToString()] : (AdornedControl.AdornedControl)MyCache.CacheInstance["ItemSearchLoadingAdorner"] : (AdornedControl.AdornedControl)MyCache.CacheInstance["LoadingAdorner"];

            loadingAdorner.IsAdornerVisible = isStart;
            loadingAdorner.Visibility = (loadingAdorner.IsAdornerVisible) ? Visibility.Visible : Visibility.Collapsed;   
              
        }

        private ICommand _StartWaitting;
        public ICommand StartWaitting
        {
            get
            {

                _StartWaitting = _StartWaitting ?? new RelayCommand((x) =>
                {
                    StartStopWait(true);

                });


                return _StartWaitting;
            }
        }



        private void ShowLogIn()
        {
            Frame LoginView = (Frame)MyMainWindow.FindName("LoginView");
            Frame SystemView = (Frame)MyMainWindow.FindName("SystemView");

            SetMenuButtonColor("showLogin");
            SystemView.Visibility = Visibility.Collapsed;
            LoginView.Visibility = Visibility.Visible;
        }

        private void SetMenuButtonColor(string btnName)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Styles/MainStyle.xaml");
            Dictionary<string,Button> btn_List = new Dictionary<string, Button>();

            btn_List.Add("showLogin", (Button)MyMainWindow.FindName("showLogin"));
            btn_List.Add("newFormTemplateFile",(Button)MyMainWindow.FindName("newFormTemplateFile"));
            btn_List.Add("syncFromPLM",(Button)MyMainWindow.FindName("syncFromPLM"));
            btn_List.Add("syncToPLM",(Button)MyMainWindow.FindName("syncToPLM"));
            btn_List.Add("loadFromPLM",(Button)MyMainWindow.FindName("loadFromPLM"));
            btn_List.Add("flaggedBy",(Button)MyMainWindow.FindName("flaggedBy"));
            btn_List.Add("PlugIn",(Button)MyMainWindow.FindName("PlugIn"));
            btn_List.Add("systemSetting",(Button)MyMainWindow.FindName("systemSetting"));
            btn_List.Add("about",(Button)MyMainWindow.FindName("about")); 
            btn_List.Add("copyToAdd", (Button)MyMainWindow.FindName("copyToAdd"));

            foreach (var btn in btn_List)
            {
                if (btn.Key == btnName)
                {
                    btn.Value.Background = (SolidColorBrush)resourceDictionary["ButtonTriggerDefaultColor"];
                }
                else
                {
                    btn.Value.Background = (SolidColorBrush)resourceDictionary["LeftDefaultColor"];
                }
            }
        }

        public Login LoginWindow
        {
            get
            {
                var cache = MyCache.CacheInstance;
                return (Login)cache["Login"];
            }
        }
        public bool IsLogin { get; set; }

        public string _userLoginName;
        public string UserLoginName
        {
            get { return _userLoginName; }
            set
            {
                SetProperty(ref _userLoginName, value);
                OnPropertyChanged("userName2");
                OnPropertyChanged("userName");
            }
        }

        private ICommand _userInformationLogin { get; set; }
        public ICommand UserInformationLogin
        {
            get
            {
                _userInformationLogin = new RelayCommand((x) =>
                {
                    var cache = MyCache.CacheInstance;
                    PasswordBox passwordBox = (PasswordBox)LoginWindow.FindName("PasswordBox");
                    PLM.Password = passwordBox.Password;
                    UserLogin();
                });
                return _userInformationLogin;
            }
        }

        private ICommand _userInformationLogout { get; set; }
        public ICommand UserInformationLogout
        {
            get
            {
                _userInformationLogout = new RelayCommand((x) =>
                {
                    UserLogout();
                    UserToggleButtonSource = "pack://application:,,,/BCS.CADs.Synchronization;component/images/DefaultAvatar.png";
                });
                return _userInformationLogout;
            }
        }

        /// <summary>
        /// 重新配置是否顯示功能Buttons
        /// </summary>
        private void ResetFunctionButtons()
        {
            if (ClsSynchronizer.VmSyncCADs.IsActiveLogin == true)
            {
                Visibility visibility = (ClsSynchronizer.VmSyncCADs.IsActiveCAD()) ? Visibility.Visible : Visibility.Collapsed;
                ResetFunctionButtons(visibility, visibility, visibility, visibility);
                
            }
        }

        public void ResetMainWindowDefaultValues()
        {
            ClsSynchronizer.VmDirectory = "";
            ClsSynchronizer.Status = "";
            ClsSynchronizer.ClassPlugins = null;
            ClsSynchronizer.SearchItemsList = null;
            ClsSynchronizer.SearchItemsCollection = null;
            ClsSynchronizer.DialogSearchItemsCollection = null;
            ClsSynchronizer.TreeSearchItemsCollection = null;
            ClsSynchronizer.ActiveSearchItem = null;
            ClsSynchronizer.NewSearchItem = null;

            ClsSynchronizer.ItemStructureChanges = null;
            ClsSynchronizer.ClassTemplateFiles = null;
            if (ClsSynchronizer.SyncCurrentObsSearchItems.Count() < 1) ClsSynchronizer.IsActiveSubDialogView = false;
            var viewPage = (Frame)MyMainWindow.FindName("viewPage");
            viewPage.Visibility = Visibility.Hidden ;
            ResetFunctionButtons();

        }


        private ICommand _userInfoBtn { get; set; }
        public ICommand UserInfoBtn
        {
            get
            {
                _userInfoBtn = new RelayCommand((x) =>
                {
                    Border bor = (Border)MyMainWindow.FindName("userInfo");

                    if (bor.Visibility == Visibility.Collapsed || bor.Visibility == Visibility.Hidden)
                        bor.Visibility = Visibility.Visible;
                    else
                        bor.Visibility = Visibility.Collapsed;
                });
                return _userInfoBtn;
            }
        }

        private ICommand _hiddenUserinformation { get; set; }
        public ICommand HiddenUserinformation
        {
            get
            {
                _hiddenUserinformation = _hiddenUserinformation ?? new RelayCommand((x) =>
                {
                    Border userinformation = (Border)x;
                    if (userinformation != null)
                    {
                        userinformation.Visibility = Visibility.Hidden;
                    }
                });


                return _hiddenUserinformation;
            }
        }

        private ICommand _checkall { get; set; }
        public ICommand CheckAll
        {
            get
            {
                _checkall = new RelayCommand((x) =>
                {
                    var CheckBox_All = (CheckBox)MyMainWindow.FindName("cb_all");
                    if (CheckBox_All.IsChecked == true)
                    {
                        GetSearchItems.ForEach(a => a.IsViewSelected = true);
                    }
                    else
                    {
                        GetSearchItems.ForEach(a => a.IsViewSelected = false);

                    }

                ObsSearchItems = new ObservableCollection<SearchItem>(GetSearchItems.Where(y => String.IsNullOrWhiteSpace(y.FileName) == false));
                    if (_syncCADsListDataGrid == null) _syncCADsListDataGrid = new SyncCADsList();
                    ClsSynchronizer.SyncListView = _syncCADsListDataGrid;

                    DataGrid gridSelectedItems = (DataGrid)_syncCADsListDataGrid.FindName("gridSelectedItems");
                    gridSelectedItems.ItemsSource = ObsSearchItems;



                    var viewPage = (Frame)MyMainWindow.FindName("viewPage");
                    viewPage.Visibility = Visibility.Visible;
                    viewPage.Navigate(_syncCADsListDataGrid);
                });
                return _checkall;
            }
        }

        private ICommand _selectexecute { get; set; }
        public ICommand SelectExecute
        {
            get
            {
                _selectexecute = new RelayCommand((x) =>
                {
                    var CheckBox_All = (Button)MyMainWindow.FindName("btn_Execute");
                    if (CheckBox_All.IsFocused)
                    {
                        ObsSearchItems = new ObservableCollection<SearchItem>(_searchItems.Where(y => String.IsNullOrWhiteSpace(y.FileName) ==false && y.IsViewSelected == true));
                        if (_editPropertiesDataGrid == null) _editPropertiesDataGrid = new EditProperties();
                        ClsSynchronizer.EditPropertiesView = _editPropertiesDataGrid;

                        DataGrid gridDetails = (DataGrid)_editPropertiesDataGrid.FindName("gridDetail");
                        gridDetails.ItemsSource = ObsSearchItems;

                        var viewPage = (Frame)MyMainWindow.FindName("viewPage");
                        viewPage.Navigate(_editPropertiesDataGrid);
                    }
                });
                return _selectexecute;
            }
        }


        /// <summary>
        /// SyncFromPLM :List Active All CADs 
        /// </summary>
        private ICommand _syncFromPLMCADsList { get; set; }
        public ICommand SyncFromPLMCADsList
        {
            get
            {
                _syncFromPLMCADsList = new RelayCommand((x) =>
                {
                    ClsSynchronizer.ItemStructureChanges = new List<StructuralChange>();
                    Button btn = (Button)MyMainWindow.FindName("syncFromPLM");

                    SetMenuButtonColor("syncFromPLM");
                    CheckBoxWPVisibility = Visibility.Visible;
                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
                    rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName("syncFromPLM_SP")));

                    ResetOperationButtons(Visibility.Visible, Visibility.Visible, Visibility.Visible, Visibility.Visible, true);
                    //設定目前操作選項 : ClsSynchronizer.VmOperation
                    ClsSynchronizer.VmFunction = SyncType.SyncFromPLM;
                    ClsSynchronizer.VmOperation = SyncOperation.QueryListItems;
                    try
                    {
                        StartStopWait(true);
                        SetSyncCADsListView(ClsSynchronizer.VmFunction);
                    }
                    catch (Exception ex)
                    {
                        StartStopWait(false);
                    }
                    //CheckBox_AllIsChecked(false);
                });
                return _syncFromPLMCADsList;
            }
        }


        /// <summary>
        /// 從範本新增
        /// </summary>
        private ICommand _newFormTemplateFileList { get; set; }
        public ICommand NewFormTemplateFileList
        {
            get
            {
                _newFormTemplateFileList = new RelayCommand((x) =>
                {
                    SelectedDirectoryGridVisibility = Visibility.Visible;
                    CheckBoxWPVisibility = Visibility.Collapsed;
                    //設定目前操作選項 : ClsSynchronizer.VmOperation
                    ResetOperationButtons(Visibility.Visible, Visibility.Visible, Visibility.Collapsed, Visibility.Visible,true );
                    
                    ClsSynchronizer.VmFunction = SyncType.NewFormTemplateFile;
                    SetMenuButtonColor("newFormTemplateFile");
                    AddFromTemplatesView();
                    CheckBox_AllIsChecked(false);
                });
                return _newFormTemplateFileList;
            }
        }

        private void ResetOperationButtons(Visibility visibilitySearch, Visibility visibilityEdit, Visibility visibilityTreeView, Visibility visibilityExecute, bool isEnable)
        {
            ClsSynchronizer.Status = "";
            Button btn = (Button)MyMainWindow.FindName("btn_Search");
            btn.Visibility = visibilitySearch;
            btn.IsEnabled = isEnable;
            btn = (Button)MyMainWindow.FindName("btn_Edit");
            btn.Visibility = visibilityEdit;
            btn.IsEnabled = isEnable;
            btn = (Button)MyMainWindow.FindName("btn_TreeView");
            btn.Visibility = visibilityTreeView;
            btn.IsEnabled = isEnable;
            btn = (Button)MyMainWindow.FindName("btn_Execute");
            btn.Visibility = visibilityExecute;
            btn.IsEnabled = isEnable;


        }

        private void ResetFunctionButtons(Visibility visibilitySyncFromPLM, Visibility visibilitySyncToPLM, Visibility visibilityFlaggedBy, Visibility visibilityPlugIn)
        {
            Button btn = (Button)MyMainWindow.FindName("syncFromPLM");
            btn.Visibility = visibilitySyncFromPLM;
            btn = (Button)MyMainWindow.FindName("syncToPLM");
            btn.Visibility = visibilitySyncToPLM;
            btn = (Button)MyMainWindow.FindName("flaggedBy");
            btn.Visibility = visibilityFlaggedBy;
            btn = (Button)MyMainWindow.FindName("PlugIn");
            btn.Visibility = visibilityPlugIn;
            TextBlock txt = (TextBlock)MyMainWindow.FindName("showStatus");
            txt.Text = txt.Text.Split((char)58)[0];

            if (String.IsNullOrWhiteSpace(txt.Text) == false ) txt.Text += " : ";

        }

        private void UpdateStatus( string status)
        {
            TextBlock txt = (TextBlock)MyMainWindow.FindName("showStatus");
            string value = txt.Text.Split((char)58)[0];
            value = (String.IsNullOrWhiteSpace(value) == false) ? value += " : " + status : status;
            txt.Text = value;

            //StartStopWait(false);
        }



        /// <summary>
        /// SyncToPLM :List Active All CADs 
        /// </summary>
        private ICommand _syncToPLMCADsList { get; set; }
        public ICommand SyncToPLMCADsList
        {
            get
            {
                _syncToPLMCADsList = new RelayCommand((x) =>
                {
                    Button btn = (Button)MyMainWindow.FindName("syncToPLM");
                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Visible;
                    ResetOperationButtons(Visibility.Visible, Visibility.Visible, Visibility.Visible, Visibility.Visible,true );
                    if (btn.IsFocused)
                    {
                    	SetMenuButtonColor("syncToPLM");
                    	Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
                    	rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName("syncToPLM_SP")));

						//設定目前操作選項 : ClsSynchronizer.VmOperation
                        ClsSynchronizer.VmFunction = SyncType.SyncToPLM;
                        ClsSynchronizer.VmOperation = SyncOperation.QueryListItems;

                        try
                        {
                            StartStopWait(true);
                            SetSyncCADsListView(ClsSynchronizer.VmFunction);
                        }
                        catch (Exception ex)
                        {
                            StartStopWait(false);
                        }


                        //CheckBox_AllIsChecked(false);
                    }

                });
                return _syncToPLMCADsList;
            }
        }

        /// <summary>
        /// List Active All CADs : for execute SetSyncCADsListView
        /// </summary>
        private ICommand _syncCADsListView { get; set; }
        public ICommand SyncCADsListView
        {
            get
            {
                _syncCADsListView = new RelayCommand((x) =>
                {
                    ClsSynchronizer.Status = "";
                    Frame viewPage;
                    switch (ClsSynchronizer.VmFunction)
                    {
                        case SyncType.NewFormTemplateFile:
                            ClsSynchronizer.VmOperation = SyncOperation.AddTemplates;
                            viewPage = (Frame)MyMainWindow.FindName("viewPage");
                            viewPage.Visibility = Visibility.Visible;
                            viewPage.Navigate(_addFromTemplateView);
                            break;

                        default:
                            ClsSynchronizer.VmOperation = SyncOperation.QueryListItems;
                            if (_syncCADsListDataGrid == null) _syncCADsListDataGrid = new SyncCADsList();
                            DataGrid gridSelectedItems = (DataGrid)_syncCADsListDataGrid.FindName("gridSelectedItems");

                            viewPage = (Frame)MyMainWindow.FindName("viewPage");
                            viewPage.Visibility = Visibility.Visible;
                            viewPage.Navigate(_syncCADsListDataGrid);

                            break;
                    }



                });
                return _syncCADsListView;
            }
        }

        public ICommand ClearSearchConditions
        {
            get
            {
                _syncItemSearch = new RelayCommand((x) =>
                {

                    int index = (ClsSynchronizer.NewSubSearchItem==null)? -1: ClsSynchronizer.NewSubSearchItem.Count() - 1;
                    SearchItem searchItemType = (ClsSynchronizer.IsActiveSubDialogView == true) ? ClsSynchronizer.NewSubSearchItem[index.ToString()] : ClsSynchronizer.NewSearchItem;
                    _ItemSearchView = (ClsSynchronizer.IsActiveSubDialogView == true) ? (ItemSearch)ClsSynchronizer.SyncSubListView[index.ToString()] : (ItemSearch)ClsSynchronizer.SyncListView;

                    if (searchItemType == null || _ItemSearchView==null ) return;
                    DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");
                    if (gridSelectedItems == null) return;
                    ClsSynchronizer.VmSyncCADs.ClearSearchConditions(gridSelectedItems, searchItemType);
                });
                return _syncItemSearch;
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

        private void SyncItemTypeSearch()
        {

            int index = (ClsSynchronizer.NewSubSearchItem == null) ? -1 : ClsSynchronizer.NewSubSearchItem.Count() - 1;
            SearchItem searchItemType = (ClsSynchronizer.IsActiveSubDialogView == true) ? ClsSynchronizer.NewSubSearchItem[index.ToString()] : ClsSynchronizer.NewSearchItem;

            UpdateSearchItemType(searchItemType);
            bool ret = false;
            var task = Task.Factory.StartNew(() => ret = SyncItemTypeSearch(searchItemType));
            task.ContinueWith((y) =>
            {

                if (ObsSearchItems == null) ObsSearchItems = new ObservableCollection<SearchItem>();
                ClsSynchronizer.SearchItemsCollection = ObsSearchItems;
                _ItemSearchView = (ClsSynchronizer.IsActiveSubDialogView == true) ? (ItemSearch)ClsSynchronizer.SyncSubListView[index.ToString()] : (ItemSearch)ClsSynchronizer.SyncListView;
                if (_ItemSearchView == null) _ItemSearchView = new ItemSearch();

                DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");

                StartStopWait(false);

                if (ObsSearchItems.Count == 0)
                {
                    Boolean isCopyToAdd = (ClsSynchronizer.VmFunction == SyncType.CopyToAddSearch);
                    LoadFromPLMView(ClsSynchronizer.ActiveWindow, ClsSynchronizer.IsShowDialog, ClsSynchronizer.ShowDialogItemType, false, isCopyToAdd);
                    return;
                }
                else
                    gridSelectedItems.RowStyle = ClsSynchronizer.RowStyle;

                gridSelectedItems.ItemsSource = ObsSearchItems;
                gridSelectedItems.RowStyle = ClsSynchronizer.RowStyle;
        
                Window win = ClsSynchronizer.ActiveWindow;
                if (ClsSynchronizer.IsActiveSubDialogView == true)
                {
                    index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
                    win = (Window)ClsSynchronizer.SyncSubDialogView[index.ToString()];
                }
                else
                {
                    if (ClsSynchronizer.SyncDialogView != null) win = (Window)ClsSynchronizer.SyncDialogView;
                }

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


        /// <summary>
        /// 更新輸入查詢條件值
        /// </summary>
        /// <param name="searchItemType"></param>
        private void UpdateSearchItemType(SearchItem searchItemType)
        {
            int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
            _ItemSearchView = (ClsSynchronizer.IsActiveSubDialogView == true) ? (ItemSearch)ClsSynchronizer.SyncSubListView[index.ToString()] : (ItemSearch)ClsSynchronizer.SyncListView;
            if (_ItemSearchView == null) _ItemSearchView = new ItemSearch();

            DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");
            ClsSynchronizer.VmSyncCADs.UpdateSearchItemType(gridSelectedItems, searchItemType);
            
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
                    
                    if (ClsSynchronizer.IsActiveSubDialogView == true)
                    {
                        ClsSynchronizer.SubDialogReturnValue = txtSelectedItemId.Text;
                        if (String.IsNullOrWhiteSpace(ClsSynchronizer.SubDialogReturnValue)) { MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected")); return; }
                        ClsSynchronizer.SubDialogReturnKeyedName = txtSelectedItemId.Tag.ToString();

                        int index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
                        win = (Window)ClsSynchronizer.SyncSubDialogView[index.ToString()];

                    }
                    else
                    {
                        ClsSynchronizer.DialogReturnValue = txtSelectedItemId.Text;
                        if (String.IsNullOrWhiteSpace(ClsSynchronizer.DialogReturnValue)) { MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected")); return; }
                        ClsSynchronizer.DialogReturnKeyedName = txtSelectedItemId.Tag.ToString();

                        win = (Window)ClsSynchronizer.SyncDialogView;
                    }
                 
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
                    if (ClsSynchronizer.IsActiveSubDialogView == true) { 
                        ClsSynchronizer.SubDialogReturnValue = "";
                        ClsSynchronizer.SubDialogReturnKeyedName = "";

                        int index = ClsSynchronizer.SyncSubDialogView.Count() - 1;
                        win = (Window)ClsSynchronizer.SyncSubDialogView[index.ToString()];

                    }
                    else
                    {
                        ClsSynchronizer.DialogReturnValue = "";
                        ClsSynchronizer.DialogReturnKeyedName = "";
                        win = (Window)ClsSynchronizer.SyncDialogView;
                    }

                    win.Close();

                });
                return _closeDialogWindow;
            }
        }


        private ICommand _showItemSearch { get; set; }
        public ICommand ShowItemSearch
        {
            get
            {
                _showItemSearch = new RelayCommand((x) =>
                {
                    Window win = (Window)ClsSynchronizer.SyncDialogView;
                    ShowSearchItems(win,"CAD");
                    //_ItemSearchView = new ItemSearch();
                    //var viewPage = (Frame)win.FindName("viewPage");
                    //viewPage.Visibility = Visibility.Visible;
                    //viewPage.Navigate(_ItemSearchView);

                });
                return _showItemSearch;
            }
        }

        private ICommand _showPartsLibrary { get; set; }
        public ICommand ShowPartsLibrary
        {
            get
            {
                _showPartsLibrary = new RelayCommand((x) =>
                {

                    _partsLibrarySearch = new PartsLibrarySearch();
                    Window win = (Window)ClsSynchronizer.SyncDialogView;
                    var viewPage = (Frame)win.FindName("viewPage");
                    viewPage.Visibility = Visibility.Visible;
                    viewPage.Navigate(_partsLibrarySearch);

                });
                return _showPartsLibrary;
            }
        }
        private void ShowSearchItems(Window win, string itemType)
        {
            if (String.IsNullOrWhiteSpace(itemType)) itemType = ItemTypeName.CAD.ToString();
            ItemType itemTypeItem = ClsSynchronizer.VmSyncCADs.GetItemType(itemType, SearchType.Search);
            ObsSearchItems = new ObservableCollection<SearchItem>();

            //_ItemSearchView = new ItemSearch();
            //_ItemSearchView.DataContext = new ItemSearchViewModel();
            _ItemSearchView = ClsSynchronizer.SyncListView ;

            ClsSynchronizer.SyncListView = _ItemSearchView;
            ((TextBox)_ItemSearchView.FindName("CADdirectory")).Visibility = Visibility.Hidden;
            ((Button)_ItemSearchView.FindName("selectedDirectory")).Visibility = Visibility.Hidden;


            DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");
            AddDataGridHeaderColumn(itemTypeItem, gridSelectedItems);

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


            DefaultSystemItemsDisplay();
            //ClsSynchronizer.ActiveWindow = (isDialog == true) ? win : MyMainWindow;
            var viewPage = (Frame)win.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_ItemSearchView);


            ClsSynchronizer.VmMessages.Status = "End";
        }



        /// <summary>
        /// 顯示範本
        /// </summary>
        private ICommand _templateFilesView { get; set; }
        public ICommand TemplateFilesView
        {
            get
            {
                _templateFilesView = new RelayCommand((x) =>
                {


                });
                return _templateFilesView;
            }
        }
        
        /// <summary>
        /// CAD屬性編輯
        /// </summary>
        private ICommand _editPropertiesView { get; set; }
        public ICommand EditPropertiesView
        {
            get
            {
                
                _editPropertiesView = new RelayCommand((x) =>
                {

                    if (ClsSynchronizer.VmFunction == SyncType.NewFormTemplateFile)
                    {
                        if (ListTemplates.Where(y => y.IsSelected == true).Count() < 1) return;

                        ClsSynchronizer.VmOperation = SyncOperation.AddTemplates;
                        ObservableCollection<ClassTemplateFile> obslistTemplates = (ClsSynchronizer.Status != "Error") ? new ObservableCollection<ClassTemplateFile>(ListTemplates.Where(y => y.IsSelected == true)): ClsSynchronizer.ClassTemplateFiles;
                        ClsSynchronizer.ClassTemplateFiles = obslistTemplates;
                        ObsSearchItems = (ClsSynchronizer.Status != "Error") ? ClsSynchronizer.VmSyncCADs.GetTemplatesProperties(obslistTemplates, ClsSynchronizer.VmDirectory) : ClsSynchronizer.SearchItemsCollection;
                        ClsSynchronizer.SearchItemsCollection = ObsSearchItems;

                    }
                    else
                    {
                        ClsSynchronizer.VmOperation = SyncOperation.EditorProperties;

                        if (ClsSynchronizer.VmFunction == SyncType.CopyToAdd)
                            ClsSynchronizer.VmSyncCADs.SearchItemsCopyFileNameProperty(_searchItems);


                        ObsSearchItems = new ObservableCollection<SearchItem>(_searchItems.Where(y => String.IsNullOrWhiteSpace(y.FileName) ==false  && y.IsViewSelected == true));
                    }

                    OperationStart("EditPropertiesView");

                    if (ClsSynchronizer.VmFunction != SyncType.NewFormTemplateFile)
                    {
                        Button btn_TreeView = (Button)MyMainWindow.FindName("btn_TreeView");

                        btn_TreeView.IsEnabled = true;
                    }


                    if (ClsSynchronizer.VmFunction == SyncType.SyncFromPLM)
                    {
                        //SyncFromPLMItemSearchView(win, false);
                        ItemFilterSearchView();
                    }
                    else
                    {
                        RefreshEditPropertiesView();
                    }
                    ClsSynchronizer.VmMessages.Status = "End";
                });
                return _editPropertiesView;
            }
        }


        private void CheckBox_AllIsChecked(bool isChecked)
        {
            try
            {
                CheckBox CheckBox_All = (CheckBox)MyMainWindow.FindName("cb_all");
                CheckBox_All.IsChecked = isChecked;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        //TreeStructureView
        /// <summary>
        /// Tree Structure View
        /// </summary>
        private ICommand _treeStructureView { get; set; }
        public ICommand TreeStructureView
        {
            get
            {
                
                _treeStructureView = new RelayCommand((x) =>
                {

                    ClsSynchronizer.VmOperation = SyncOperation.CADStructure;

                    OperationStart("TreeStructureView");

                    if (ClsSynchronizer.VmFunction == SyncType.CopyToAdd)
                        ClsSynchronizer.VmSyncCADs.SearchItemsCopyFileNameProperty(_searchItems);

                    if (_treeStructure == null) _treeStructure = new TreeStructure();
                    ShowTreeStructure(_treeStructure);

                    var viewPage = (Frame)MyMainWindow.FindName("viewPage");

                    CheckBoxWPVisibility = Visibility.Collapsed;
					viewPage.Visibility = Visibility.Visible;
                    viewPage.Navigate(_treeStructure);

                    ClsSynchronizer.VmMessages.Status = "End";
                });
                return _treeStructureView;

            }
        }

        private void ShowTreeStructure(TreeStructure treeStructure)
        {

            SearchItem activeItem = ClsSynchronizer.VmSyncCADs.GetActiveSearchItem(_searchItems);

            string filename = "";
            if (activeItem != null)
                filename = activeItem.FileName;


            TreeSearchItems = new ObservableCollection<SearchItemsViewModel>();
            foreach (SearchItem searchItem in _searchItems.Where(y => y.FileName.ToLower() == filename.ToLower()))
            {
                SearchItemsViewModel treeSearchItem = new SearchItemsViewModel();
                treeSearchItem.Name = searchItem.FileName;
                treeSearchItem.NodeSearchItem = searchItem;
                treeSearchItem.ClassName = searchItem.ClassName;
                treeSearchItem.ClassThumbnail = searchItem.ClassThumbnail;
                treeSearchItem.RestrictedStatus = searchItem.RestrictedStatus;
                treeSearchItem.VersionStatus = searchItem.VersionStatus;
                treeSearchItem.AccessRights = searchItem.AccessRights;
                treeSearchItem.Thumbnail = searchItem.Thumbnail;// ClsSynchronizer.VmSyncCADs.GetImageFullName(searchItem.Thumbnail); 
                treeSearchItem.IsChecked = searchItem.IsViewSelected;
                string displayName = System.IO.Path.GetFileNameWithoutExtension(searchItem.FileName);
                if (treeSearchItem.IsChecked == true)
                {

                    treeSearchItem.IsReplacement = false; //Root 沒有替換分件
                    treeSearchItem.IsCopyToAdd = false;//Root 沒有複製轉新增

                    if (treeSearchItem.ClassName != "Assembly")
                    {
                        treeSearchItem.IsInsert = false;
                        treeSearchItem.IsInsertSaveAs = false;
                    }
                    
                    PLMProperty property = searchItem.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) ==false).SingleOrDefault();
                    if (property != null)
                    {
                        displayName = property.DisplayValue;
                        treeSearchItem.OperationType = 4;
                    }
                }
                else
                {
                    treeSearchItem.IsInsert = false;
                    treeSearchItem.IsInsertSaveAs = false;
                    treeSearchItem.IsReplacement = false;
                    treeSearchItem.IsCopyToAdd = false;
                }

                if (displayName!="") treeSearchItem.DisplayName = displayName;
                TreeSearchItems.Add(treeSearchItem);
            }

            ClsSynchronizer.TreeStructureView = treeStructure;
            ClsSynchronizer.TreeSearchItemsCollection = TreeSearchItems;

            TreeView treeView = (TreeView)treeStructure.FindName("treeStructureItems");
            treeView.ItemsSource = TreeSearchItems;
        }

        private ICommand _syncExecute { get; set; }
        public ICommand SyncExecute
        {
            get
            {

                _syncExecute = new RelayCommand((x) =>
                {

                    try
                    {
                        Frame viewPage = (Frame)MyMainWindow.FindName("viewPage");
                        SelectedDirectoryGridVisibility = Visibility.Collapsed;

                        ClsSynchronizer.Status = "";

                        OperationStart("SyncExecute");

                        IEnumerable<SearchItem> checkItems = (ClsSynchronizer.VmOperation == SyncOperation.AddTemplates) ? (IEnumerable<SearchItem>)ObsSearchItems : _searchItems;
                        if (ClsSynchronizer.VmSyncCADs.SyncCheckRules(checkItems, ClsSynchronizer.VmFunction, ClsSynchronizer.VmOperation) == false)
                        {
                            ClsSynchronizer.Status = "Error";
                            UpdateStatus(ClsSynchronizer.Status);
                            ShowMessagesView();
                            return;
                        }

                        try
                        {
                            //bool ret = (ClsSynchronizer.VmFunction == SyncType.CopyToAdd) ? IsExecuteCopyToAdd(viewPage) : IsExecuteOperation(viewPage);
                            bool ret = IsExecuteOperation(viewPage);
                        }
                        catch (Exception ex)
                        {
                            StartStopWait(false);
                        }

                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        ClsSynchronizer.Status = "Error";
                        ShowMessagesView();
                    }
                });
                return _syncExecute;
            }
        }


        private bool IsExecuteOperation(Frame viewPage)
        {

            List<SearchItem> searchItems = new List<SearchItem>();
            SearchItem searchItem = null;

            switch (ClsSynchronizer.VmOperation)
            {
                case SyncOperation.AddTemplates:
                    if (String.IsNullOrWhiteSpace(ClsSynchronizer.VmDirectory))
                    {
                        MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_PleasePickASavedFilePath"));
                        //StartStopWait(false);
                        return false;
                    }
                    break;


                case SyncOperation.CopyToAddSearch:
                case SyncOperation.LoadListItems:
                    //win = ClsSynchronizer.ActiveWindow;
                    if (CheckItemSearchView() == false) return false;

                    if (String.IsNullOrWhiteSpace(ClsSynchronizer.VmSelectedItemId) == true) return true;

                    SearchItem currentSearchItem = ClsSynchronizer.SearchItemsCollection.Where(y => y.ItemId == ClsSynchronizer.VmSelectedItemId).FirstOrDefault();
                    searchItem = ClsSynchronizer.VmSyncCADs.GetVersionSearchItem(currentSearchItem);
                    if (searchItem.FileId == "")
                    {
                        MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_CADFileDoesNotExistPLM"));
                        return false;
                    }

                    searchItem.FileName = ClsSynchronizer.VmSyncCADs.GetFileNameByCADItemId(searchItem.ItemId);
                    if (ClsSynchronizer.VmOperation ==SyncOperation.CopyToAddSearch)
                    {
                        if (ShowNewFileName(ChangeToNewFileName(searchItem.FileName))==false ) return false ;
                        searchItem.FileName = ClsSynchronizer.DialogReturnValue;
                    }


                    //檢查是否為表示為共用件及標準件(需為最新版本,自己鎖定的圖檔,才能下載到本地端,進行修改) ****@@@@@@@***

                    searchItem.IsRoot = true;
                    searchItems.Add(searchItem);

                    break;
                //case SyncOperation.CopyToAddSearch:
                //    if (CheckItemSearchView() == false) return false;

                //    if (String.IsNullOrWhiteSpace(ClsSynchronizer.VmSelectedItemId) == true) return true;

                //    ResetOperationButtons(Visibility.Visible, Visibility.Visible, Visibility.Visible, Visibility.Visible, true);
                //    searchItem = ClsSynchronizer.SearchItemsCollection.Where(y => y.ItemId == ClsSynchronizer.VmSelectedItemId).FirstOrDefault();
                //    searchItems.Add(searchItem);
                //    ClsSynchronizer.VmFunction = SyncType.CopyToAdd;

                    //break;

                case SyncOperation.QueryListItems:

                    if (ClsSynchronizer.VmFunction == SyncType.LockOrUnlock)
                    {
                        _searchItems = ClsSynchronizer.SearchItemsList;
                        _syncCADsListDataGrid = (ClsSynchronizer.IsActiveSubDialogView) ? ClsSynchronizer.SyncSubListView : ClsSynchronizer.SyncListView;
                    }
                    break;

            }

            StartStopWait(true);

            var task = Task.Factory.StartNew(() => ClsSynchronizer.Status = ExecuteOperation(searchItems, searchItem));
            task.ContinueWith((x) =>
            {

                DataGrid gridSelectedItems;
                switch (ClsSynchronizer.VmOperation)
                {
                    case SyncOperation.EditorProperties:

                        UpdateStatus(ClsSynchronizer.Status);
                        ClsSynchronizer.VmOperation = SyncOperation.EditorProperties;
                        ObsSearchItems = new ObservableCollection<SearchItem>(_searchItems.Where(y => String.IsNullOrWhiteSpace(y.FileName) == false && y.IsViewSelected == true));
                        if (ClsSynchronizer.VmFunction != SyncType.SyncFromPLM) RefreshEditPropertiesView();
                        break;

                    case SyncOperation.CADStructure:
                        UpdateStatus(ClsSynchronizer.Status);
                        ClsSynchronizer.ItemStructureChanges = null;

                        if (viewPage != null) viewPage.Visibility = Visibility.Hidden;
                        break;

                    case SyncOperation.AddTemplates:
                        UpdateStatus(ClsSynchronizer.Status);

                        ResetFunctionButtons();
                        if (viewPage != null) viewPage.Visibility = Visibility.Hidden;
                        break;


                    case SyncOperation.CopyToAddSearch:
                    case SyncOperation.LoadListItems:
                        UpdateStatus(ClsSynchronizer.Status);

                        ResetFunctionButtons();
                        if (viewPage != null) viewPage.Visibility = Visibility.Hidden;
                        break;


                    //case SyncOperation.CopyToAddSearch:
                    //    _searchItems = searchItems;
                    //    ClsSynchronizer.SearchItemsList = _searchItems;

                    //    ObsSearchItems = new ObservableCollection<SearchItem>();

                    //    if (_searchItems != null) ObsSearchItems = new ObservableCollection<SearchItem>(_searchItems.Where(y => String.IsNullOrWhiteSpace(y.FileName) == false));

                    //    _syncCADsListDataGrid = new SyncCADsList();
                    //    gridSelectedItems = (DataGrid)_syncCADsListDataGrid.FindName("gridSelectedItems");
                    //    ClsSynchronizer.SyncListView = _syncCADsListDataGrid;

                    //    gridSelectedItems.ItemsSource = ObsSearchItems;
                    //    viewPage.Visibility = Visibility.Visible;
                    //    viewPage.Navigate(_syncCADsListDataGrid);

                    //    ClsSynchronizer.VmMessages.Status = "End";

                    //    break;

                    case SyncOperation.QueryListItems:

                        if (ClsSynchronizer.VmFunction == SyncType.LockOrUnlock)
                        {
                            UpdateStatus(ClsSynchronizer.Status);

                            ClsSynchronizer.SearchItemsList = _searchItems;

                            ObsSearchItems = new ObservableCollection<SearchItem>(_searchItems.Where(y => String.IsNullOrWhiteSpace(y.FileName) == false));

                            gridSelectedItems = (DataGrid)_syncCADsListDataGrid.FindName("gridSelectedItems");
                            gridSelectedItems.ItemsSource = ObsSearchItems;
                            viewPage.Navigate(_syncCADsListDataGrid);
                        }
                        else if (ClsSynchronizer.VmFunction == SyncType.SyncFromPLM || ClsSynchronizer.VmFunction == SyncType.SyncToPLM)
                        {
                            if (ClsSynchronizer.Status != "Error")
                            {
                                ClsSynchronizer.Status = (ClsSynchronizer.VmSyncCADs.SyncCADsFilesStructure(ref _searchItems, ClsSynchronizer.ItemStructureChanges, ClsSynchronizer.VmFunction)) ? "Operation Finish" : "Error";
                            }
                            UpdateStatus(ClsSynchronizer.Status);
                            ClsSynchronizer.ItemStructureChanges = null;

                            if (viewPage != null) viewPage.Visibility = Visibility.Hidden;
                        }
                        break;

                }

                StartStopWait(false);

                ClsSynchronizer.VmMessages.Status = "End";
                ShowMessagesView();
                CheckAllIsCheck = false;

                if (ObsItemsMessage.FirstOrDefault(a => a.IsError == true) == null)
                    MyMainWindow.Hide();


            }, TaskScheduler.FromCurrentSynchronizationContext());

            return true;
        }


        private string ExecuteOperation(List<SearchItem> searchItems, SearchItem searchItem)
        {


            string status = "";
            switch (ClsSynchronizer.VmOperation)
            {
                case SyncOperation.EditorProperties:

                    status = (ClsSynchronizer.VmSyncCADs.SyncCADsProperties(ref _searchItems, ClsSynchronizer.VmFunction)) ? "Operation Finish" : "Error";
                    break;

                case SyncOperation.CADStructure:
                    status = (ClsSynchronizer.VmSyncCADs.SyncCADsFilesStructure(ref _searchItems, ClsSynchronizer.ItemStructureChanges, ClsSynchronizer.VmFunction)) ? "Operation Finish" : "Error";
                    break;

                case SyncOperation.AddTemplates:

                    status = (ClsSynchronizer.VmSyncCADs.AddCADFromTemplates(ObsSearchItems)) ? "Operation Finish" : "Error";
                    break;
                case SyncOperation.LoadListItems:

                    status = (ClsSynchronizer.VmSyncCADs.LoadFromPLM(ClsSynchronizer.VmDirectory, searchItems, ClsSynchronizer.VmFunction) != null) ? "Operation Finish" : "Error";
                    break;

                case SyncOperation.CopyToAddSearch:
                    searchItems = ClsSynchronizer.VmSyncCADs.CADItemStructure(searchItem, ClsSynchronizer.VmFunction);
                    SearchItem activeSearchItem  =   ClsSynchronizer.VmSyncCADs.GetActiveSearchItem(searchItems);
                    activeSearchItem.FileName = searchItem.FileName;
                     status = (ClsSynchronizer.VmSyncCADs.CopyToAdd(searchItems, ClsSynchronizer.VmDirectory, ClsSynchronizer.VmFunction ,false )) ? "Operation Finish" : "Error";
                    break;


                case SyncOperation.QueryListItems:

                    if (ClsSynchronizer.VmFunction == SyncType.LockOrUnlock)
                        status = (ClsSynchronizer.VmSyncCADs.LockOrUnlock(_searchItems)) ? "Operation Finish" : "Error";
                    else if (ClsSynchronizer.VmFunction == SyncType.SyncFromPLM || ClsSynchronizer.VmFunction == SyncType.SyncToPLM)
                        ClsSynchronizer.Status = (ClsSynchronizer.VmSyncCADs.SyncCADsProperties(ref _searchItems, ClsSynchronizer.VmFunction)) ? "Operation Finish" : "Error";

                    break;

            }

            return status;
        }

        /*
        private bool IsExecuteCopyToAdd(Frame viewPage)
        {
            

            List<SearchItem> searchItems = new List<SearchItem>();
            SearchItem searchItem = null;

            if (CheckItemSearchView() == false) return false;

            if (String.IsNullOrWhiteSpace(ClsSynchronizer.VmSelectedItemId) == true) return true;

            ResetOperationButtons(Visibility.Visible, Visibility.Visible, Visibility.Visible, Visibility.Visible, true);
            searchItem = ClsSynchronizer.SearchItemsCollection.Where(y => y.ItemId == ClsSynchronizer.VmSelectedItemId).FirstOrDefault();
            searchItems.Add(searchItem);
            ClsSynchronizer.VmFunction = SyncType.CopyToAdd;

            StartStopWait(true);

            ClsSynchronizer.VmSyncCADs.SearchItemsCopyFileNameProperty(searchItems);
            //ClsSynchronizer.VmSyncCADs.SearchItemsCopyFileNameProperty(_searchItems);
            //List<SearchItem> execSearchItems = _searchItems.Where(x => x.IsViewSelected == true).ToList() as List<SearchItem>;
            bool ret = false;
            var task = Task.Factory.StartNew(() => ret = CopyToAdd(searchItems));
            task.ContinueWith((x) =>
            {
                ClsSynchronizer.Status = (ret) ? "Operation Finish" : "Error";
                UpdateStatus(ClsSynchronizer.Status);
                ResetFunctionButtons();
                if (viewPage != null) viewPage.Visibility = Visibility.Hidden;


                StartStopWait(false);

                ClsSynchronizer.VmMessages.Status = "End";
                ShowMessagesView();
                CheckAllIsCheck = false;

                if (ObsItemsMessage.FirstOrDefault(a => a.IsError == true) == null)
                    MyMainWindow.Hide();


            }, TaskScheduler.FromCurrentSynchronizationContext());

            return true;
        }

        private bool CopyToAdd(List<SearchItem> searchItems)
        {

            bool ret = ClsSynchronizer.VmSyncCADs.CopyToAdd(searchItems, ClsSynchronizer.VmDirectory, ClsSynchronizer.VmFunction);
            return ret;
        }
        */


        private bool CheckItemSearchView()
        {

            ClsSynchronizer.VmSelectedItemId = "";
            int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
            _ItemSearchView = (ClsSynchronizer.IsActiveSubDialogView) ? (ItemSearch)ClsSynchronizer.SyncSubListView[index.ToString()] : (ItemSearch)ClsSynchronizer.SyncListView;
            if (_ItemSearchView != null)
            {
                TextBox txtSelectedItemId = (TextBox)_ItemSearchView.FindName("selectedItemId");
                 
                if (String.IsNullOrWhiteSpace(txtSelectedItemId.Text) || String.IsNullOrWhiteSpace(ClsSynchronizer.VmDirectory))
                {

                    if (String.IsNullOrWhiteSpace(ClsSynchronizer.VmDirectory)) MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_PleasePickASavedFilePath"));
                    if (String.IsNullOrWhiteSpace(txtSelectedItemId.Text)) MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected"));
                    return false ;
                }

                ClsSynchronizer.VmSelectedItemId = txtSelectedItemId.Text;
            }
            return true;
        }

        //showMessagesView
        private ICommand _showMessages { get; set; }
        public ICommand ShowMessages
        {
            get
            {
                _syncExecute = new RelayCommand((x) =>
                {
                    try
                    {
                        ShowMessagesView();
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }
                });
                return _syncExecute;
            }
        }

        public void ShowMessagesView()
        {

            ObsItemsMessage = new ObservableCollection<ItemMessage>(ClsSynchronizer.VmMessages.ItemMessages);
            if (_addItemsMessageView == null) _addItemsMessageView = new ItemsMessage();       

            DataGrid gridMessage = (DataGrid)_addItemsMessageView.FindName("GridMessage");
            ClsSynchronizer.SyncMessagesView = _addItemsMessageView;
            gridMessage.ItemsSource = ObsItemsMessage;

            var viewPage = (Frame)MyMainWindow.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_addItemsMessageView);
            //ShowPartsLibrarySearchDialog();
        }

        private void ShowPartsLibrarySearchDialog()
        {


            PartsLibrarySearchDialog itemSearchDialog = new PartsLibrarySearchDialog();
            ClsSynchronizer.DialogReturnValue = "";
            ClsSynchronizer.IsShowDialog = true;
            itemSearchDialog.Width = 800;
            itemSearchDialog.Height = 600;
            itemSearchDialog.Topmost = true;
            itemSearchDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //ItemSearchViewControl(Visibility.Visible);
            itemSearchDialog.ShowDialog();
            ClsSynchronizer.IsShowDialog = false;
            if (String.IsNullOrWhiteSpace(ClsSynchronizer.DialogReturnValue) == false)
            {
                //MessageBox.Show(ClsSynchronizer.DialogReturnValue);
                string full = ClsSynchronizer.DialogReturnValue.Split((char)44)[0];
                ClsSynchronizer.VmSyncCADs.OpenFile(System.IO.Path.GetDirectoryName(full), System.IO.Path.GetFileName(full));
            }
        }

        /// <summary>
        /// 顯示多版本選取介面
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private Boolean ShowRevisionList(string itemType, string itemId)
        {
            string itemDialogType = ClsSynchronizer.ShowDialogItemType;
            RevisionList revisionDialog = new RevisionList(itemType,itemId);
            ClsSynchronizer.ShowDialogItemType = itemType;
            ClsSynchronizer.IsShowDialog = true;
            revisionDialog.Width = 800;
            revisionDialog.Height = 600;
            revisionDialog.Topmost = true;
            revisionDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            revisionDialog.ShowDialog();
            ClsSynchronizer.IsShowDialog = false;
            ClsSynchronizer.ShowDialogItemType= itemDialogType;
            if (String.IsNullOrWhiteSpace(ClsSynchronizer.DialogReturnValue) == false) return true;
            return false;
        }

        private static ObservableCollection<LanguageList> _allLang;
        public static ObservableCollection<LanguageList> AllLang
        {
            get { return _allLang; }
        }

        private ICommand _systemSetting { get; set; }
        public ICommand SystemSetting
        {
            get
            {
                _systemSetting = new RelayCommand((x) =>
                {
                    CultureInfo culture1 = CultureInfo.CurrentCulture;
                    
                   _allLang = SelectLangs.GetAllLang();
                    SystemSetting sysSet = new SystemSetting();
                    ComboBox langCB = (ComboBox)sysSet.FindName("langComboBox");
                    var getLanguageItem = langCB.Items.SourceCollection.OfType<LanguageList>().FirstOrDefault(a=>a.Label.ToLower() == culture1.DisplayName.ToLower());
                    langCB.SelectedIndex = langCB.Items.IndexOf(getLanguageItem);

                    SetMenuButtonColor("systemSetting");
                    Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
                    rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName("systemSetting_SP")));

                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Collapsed;

                    Frame SystemView = (Frame)MyMainWindow.FindName("SystemView");
                    SystemView.Navigate(sysSet);
                    SystemView.Visibility = Visibility.Visible;
                });
                return _systemSetting;
            }
        }

        private ICommand _about { get; set; }
        public ICommand About
        {
            get
            {
                _about = new RelayCommand((x) =>
                {
                    CultureInfo culture1 = CultureInfo.CurrentCulture;

                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Collapsed;

                    SetMenuButtonColor("about");
                    Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
                    rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName("about_SP")));
                });
                return _about;
            }
        }

        private ICommand _settingOK { get; set; } 
        public ICommand SettingOK
        {
            get
            {
                _settingOK = new RelayCommand((x) =>
                {
                    Button btn_OK = (Button)x;
                    var parent = VisualTreeHelper.GetParent(btn_OK);

                    ClsSynchronizer.Language = SelectedLang.Value;
                    ClsSynchronizer.VmSyncCADs.SetLanguageResources();
                    ResourceDictionary resourceDictionary = ClsSynchronizer.VmSyncCADs.GetLanguageResources();

                    var language = MyMainWindow.Resources.MergedDictionaries.FirstOrDefault(a => ((string)a["isLanguage"])?.ToLower() == "true");
                    MyMainWindow.Resources.MergedDictionaries.Remove(language);
                    MyMainWindow.Resources.MergedDictionaries.Add(resourceDictionary);
                });
                return _settingOK;
            }
        }




        private ICommand _syncPlugIn { get; set; }
        public ICommand SyncPlugIn
        {
            get
            {

                _syncPlugIn = new RelayCommand((x) =>
                {

                    var button = x as Button;
                    ClassPlugin plug = ClsSynchronizer.ClassPlugins.Where(y => y.Name == button.Tag.ToString()).FirstOrDefault();
                    ClsSynchronizer.VmSyncCADs.RunFunction(plug, ClsSynchronizer.ActiveSearchItem);
                });
                return _syncPlugIn;
            }
        }


        private ICommand _showFlaggedBy { get; set; }
        public ICommand ShowFlaggedBy
        {
            get
            {

                _showFlaggedBy = new RelayCommand((x) =>
                {
                    ResetOperationButtons(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible, true);
                    ClsSynchronizer.VmFunction = SyncType.LockOrUnlock;
                    ClsSynchronizer.VmOperation = SyncOperation.QueryListItems;

                    SetMenuButtonColor("flaggedBy");
                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Collapsed;
                    Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
                    rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName("flaggedBy_SP")));

                    try
                    {
                        StartStopWait(true);
                        SetSyncCADsListView(ClsSynchronizer.VmFunction);
                        
                    }
                    catch (Exception ex)
                    {
                        StartStopWait(false);
                    }

                    //CheckBox_AllIsChecked(false);
                });
                return _showFlaggedBy;
            }
        }

        //
        private ICommand _showCopyToAdd { get; set; }
        public ICommand ShowCopyToAdd
        {
            get
            {

                _showCopyToAdd = new RelayCommand((x) =>
                {
                    SetMenuButtonColor("copyToAdd");
                    ResetOperationButtons(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible, true);
                    ClsSynchronizer.VmDirectory = "";
                    ClsSynchronizer.VmSelectedItemId = "";

                    _ItemSearchView = null;
                    CopyToAddView((Window)_view, false);
                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Collapsed;
                    CheckBox_AllIsChecked(false);
                });
                return _showCopyToAdd;
            }
        }

        //PlugInFuncs
        private ICommand _showPlugInFuncs { get; set; }
        public ICommand ShowPlugInFuncs
        {
            get
            {
                _showPlugInFuncs = new RelayCommand((x) =>
                {
					ResetOperationButtons(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed,true );

                    SetMenuButtonColor("PlugIn");
                    ClsSynchronizer.VmFunction = SyncType.PluginModule;
                    PlugInFunctionsView();
                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Collapsed;
                    CheckBox_AllIsChecked(false);
                });
                return _showPlugInFuncs;
            }
        }


        
        private ICommand _loadFromPLM { get; set; }
        public ICommand LoadFromPLM
        {
            get
            {

                _loadFromPLM = new RelayCommand((x) =>
                {
                    ResetOperationButtons(Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible,true );
                    ClsSynchronizer.VmFunction = SyncType.LoadFromPLM ;
                    ClsSynchronizer.VmOperation = SyncOperation.QueryListItems;
                    _ItemSearchView = null;
                    LoadFromPLMView((Window)_view, false);
                    SelectedDirectoryGridVisibility = Visibility.Collapsed;
                    CheckBoxWPVisibility = Visibility.Collapsed;
                    ItemSearchViewControl(Visibility.Collapsed);
                    CheckBox_AllIsChecked(false);
                });
                return _loadFromPLM;
            }
        }

        private void ItemSearchViewControl(Visibility ShowLineVisibility)
        {
            ShowLine = ShowLineVisibility;
        }

        private ICommand _itemPickerImageLeftClick { get; set; }
        public ICommand ItemPickerImageLeftClick
        {
            get
            {
                _itemPickerImageLeftClick = new RelayCommand((x) =>
                {
                    //System.Diagnostics.Debugger.Break();
                    dynamic y = x;
                    TextBox txtProperty =(TextBox) y.TemplatedParent;
                    ShowItemSearchDialog(txtProperty, true);
                    


                });
                return _itemPickerImageLeftClick;
            }
        }


        private ICommand _gridItemPickerImageLeftClick { get; set; }
        public ICommand GridItemPickerImageLeftClick
        {
            get
            {
                _gridItemPickerImageLeftClick = new RelayCommand((x) =>
                {

                    dynamic y = x;
                    TextBox txtProperty = (TextBox)y.TemplatedParent;

                    ShowItemSearchDialog(txtProperty, true);
                    if (ClsSynchronizer.SyncCurrentObsSearchItems.Count() < 1) ClsSynchronizer.IsActiveSubDialogView = false;
                });
                return _gridItemPickerImageLeftClick;
            }
        }

        private void ShowItemSearchDialog(TextBox txtProperty,bool isSubItemSearchDialog)
        {

            string itemType = txtProperty.Tag.ToString();

            int index = ClsSynchronizer.SyncCurrentObsSearchItems.Count();
            ClsSynchronizer.SyncCurrentObsSearchItems.Add(index.ToString(),ObsSearchItems);
            ClsSynchronizer.SyncSubDialogView.Add(index.ToString(),null);
            ClsSynchronizer.NewSubSearchItem.Add(index.ToString(), null);
            ClsSynchronizer.SyncSubListView.Add(index.ToString(), null);
            ClsSynchronizer.SyncSubDialogLoadingAdorner.Add(index.ToString(), null);
            
            //ObservableCollection<SearchItem> currentObsSearchItems = ObsSearchItems;

            ClsSynchronizer.CurrentDialog =(isSubItemSearchDialog)? "SubItemSearchDialog": "ItemSearchDialog";
            string itemDialogType = ClsSynchronizer.ShowDialogItemType;
            ClsSynchronizer.ShowDialogItemType = itemType;
            SubItemSearchDialog itemSearchDialog = new SubItemSearchDialog(itemType);
            ClsSynchronizer.IsShowDialog = true;
            itemSearchDialog.Width = 800;
            itemSearchDialog.Height = 600;
            itemSearchDialog.Topmost = true;
            itemSearchDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ItemSearchViewControl(Visibility.Visible);
            itemSearchDialog.ShowDialog();

            index = ClsSynchronizer.SyncCurrentObsSearchItems.Count()-1;
            ObsSearchItems = ClsSynchronizer.SyncCurrentObsSearchItems[index.ToString ()];

            ClsSynchronizer.SyncCurrentObsSearchItems.Remove(index.ToString());
            ClsSynchronizer.SyncSubDialogView.Remove(index.ToString());
            ClsSynchronizer.NewSubSearchItem.Remove(index.ToString());
            ClsSynchronizer.SyncSubListView.Remove(index.ToString());
            ClsSynchronizer.SyncSubDialogLoadingAdorner.Remove(index.ToString());

            ClsSynchronizer.IsShowDialog = false ;
            ClsSynchronizer.ShowDialogItemType = itemDialogType;
            if (String.IsNullOrWhiteSpace(ClsSynchronizer.SubDialogReturnValue)==false)
            {
                txtProperty.Text = (isSubItemSearchDialog) ? ClsSynchronizer.SubDialogReturnKeyedName: ClsSynchronizer.DialogReturnKeyedName;
            }
        }


        /// <summary>
        /// 更新編輯屬性視窗
        /// </summary>
        /// <param name="win"></param>
        private void RefreshEditPropertiesView()
        {
            try
            {
                _editPropertiesDataGrid = ClsSynchronizer.EditPropertiesView;
                if (_editPropertiesDataGrid == null)
                {
                    _editPropertiesDataGrid = new EditProperties();
                    ClsSynchronizer.EditPropertiesView = _editPropertiesDataGrid;
                }

                ClsSynchronizer.FormGrid = (Grid)_editPropertiesDataGrid.FindName("formContent");
                SetFormContent(ClsSynchronizer.FormGrid);

                DataGrid gridSelectedItems = (DataGrid)_editPropertiesDataGrid.FindName("gridDetail");
                gridSelectedItems.ItemsSource = ObsSearchItems;
                var viewPage = (Frame)MyMainWindow.FindName("viewPage");
                viewPage.Visibility = Visibility.Visible;
                viewPage.Navigate(_editPropertiesDataGrid);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 產生Form表單資料
        /// </summary>
        /// <param name="formGrid">要寫入表單的Grid空間</param>
        /// <param name="plmProp">畫面所選資料</param>
        private void SetFormContent(Grid formGrid, ObservableCollection<PLMProperty> plmProp = null)
        {
            double left = 10, top = 10, right = 0, bottom = 10;

            formGrid.Children.Clear();
            var plmProperties = (plmProp != null) ? plmProp : ObsSearchItems.FirstOrDefault(a => a.PlmProperties != null).PlmProperties;
            foreach (var item in plmProperties)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
                stackPanel.Margin = new Thickness(left, top, right, bottom);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = item.Label;
                stackPanel.Children.Add(textBlock);

                TextBox textBox = new TextBox();
                textBox.Height = 25d;
                textBox.Width = 220d;
                if (plmProp != null)
                {
                    textBox.DataContext = item;
                    Binding binding = new Binding("DisplayValue");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    textBox.SetBinding(TextBox.TextProperty, binding);
                    textBox.SetBinding(TextBox.TagProperty, new Binding("DataSource"));

                    binding = new Binding("IsModify");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Converter = new PLMProperty();
                    textBox.SetBinding(TextBox.ForegroundProperty, binding);

                    binding = new Binding("SyncColorTypeValue");
                    binding.Mode = BindingMode.TwoWay;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    binding.Converter = new PLMProperty();
                    textBox.SetBinding(TextBox.BackgroundProperty, binding);

                    binding = new Binding("DataType");
                    binding.Converter = new StyleConverter();
                    textBox.SetBinding(TextBox.StyleProperty, binding);
                }
                stackPanel.Children.Add(textBox);

                formGrid.Children.Add(stackPanel);
                top = top + 60;
                if (top >= 300)
                {
                    top = 10;
                    left = left + 250;
                }
            }
        }

        private ICommand search_visibility { get; set; }
        
        private void UserLogin()
        {
            try
            {
                var cache = MyCache.CacheInstance;

                if (PLM == null) return;
                string strUrl = PLM.Url;
                if (PLM.SelectedListItem !=null) PLM.Database = PLM.SelectedListItem.Value;
                string strDBName = PLM.Database;
                string strLogin = PLM.LoginName;
                string strPassword = PLM.Password;


                StartStopWait(true);
                bool ret=false;
                Exception exception = null;
                var task = Task.Factory.StartNew(() => ret= ClsSynchronizer.VmSyncCADs.Login(strUrl, strDBName, strLogin, strPassword,ref exception));
                task.ContinueWith((x) =>
                {

                    StartStopWait(false);

                    ResetMainWindowDefaultValues();

                    if (ClsSynchronizer.VmSyncCADs.IsActiveLogin == true)
                    {
                        IsLogin = true;
                        UserLoginName = ConnInnovator.UserName;
                        //ClsSynchronizer.VmPartsLibrary = ClsSynchronizer.VmSyncCADs.GetCommonPartsLibrary();
                        ClsSynchronizer.VmLibraryPaths = ClsSynchronizer.VmSyncCADs.GetCommonPartsLibrary().Paths;
                        DefaultSystemButtonsTextBlocks(UserLoginName, strDBName, true);
                    }
                    //關閉Windows : 要改變UI的畫面是否允許操作功能 @@@@@@

                    ResetFunctionButtons();                   
                    //if (ret == false && exception!=null) throw new Exception(exception.Message);
                    if (ret == false && exception != null) MessageBox.Show(exception.Message);

                }, TaskScheduler.FromCurrentSynchronizationContext());

            }
            catch (Exception ex)
            {
                StartStopWait(false);
                MessageBox.Show(ex.Message);
            }
        }

        private void UserLogout()
        {
            UserLoginName = "";
            DefaultSystemButtonsTextBlocks(UserLoginName,"", false);
            ClsSynchronizer.VmSyncCADs.Logoff();
            IsLogin = false;
            ShowLogIn();
        }

        /// <summary>
        /// 預設ButtonsTextBlocks顯示
        /// </summary>
        /// <param name="userLoginName"></param>
        /// <param name="strDBName"></param>
        /// <param name="isLogin"></param>
        private void DefaultSystemButtonsTextBlocks(string userLoginName,string strDBName,bool isLogin)
        {
            Button newFormTemplateFile = (Button)MyMainWindow.FindName("newFormTemplateFile");
            Button syncFromPLM = (Button)MyMainWindow.FindName("syncFromPLM");
            Button syncToPLM = (Button)MyMainWindow.FindName("syncToPLM");
            Button loadFromPLM = (Button)MyMainWindow.FindName("loadFromPLM");
            Button copyToAdd = (Button)MyMainWindow.FindName("copyToAdd");
            Button flaggedBy = (Button)MyMainWindow.FindName("flaggedBy");
            Button PlugIn = (Button)MyMainWindow.FindName("PlugIn");
            Button login = (Button)LoginWindow.FindName("login");
            Button logout = (Button)LoginWindow.FindName("logout");
            TextBlock userName = (TextBlock)LoginWindow.FindName("userName");
            TextBlock userName2 = (TextBlock)MyMainWindow.FindName("userName2");
            TextBlock dataBaseName = (TextBlock)MyMainWindow.FindName("dataBaseName");
            ToggleButton loginImage = (ToggleButton)MyMainWindow.FindName("LoginImage");
            Button userInfoLogout = (Button)MyMainWindow.FindName("userInfoLogout");

            ChangeLoginImage(loginImage);

            userName.Text = userLoginName;
            userName2.Text = userLoginName;
            dataBaseName.Text = strDBName;


            newFormTemplateFile.IsEnabled = isLogin;//樣板按鈕
            syncFromPLM.IsEnabled = isLogin;//PLM > CAD按鈕
            syncToPLM.IsEnabled = isLogin;//CAD > PLM按鈕
            loadFromPLM.IsEnabled = isLogin;//圖檔下載按鈕
            copyToAdd.IsEnabled = isLogin;
            flaggedBy.IsEnabled = isLogin;//標記按鈕
            PlugIn.IsEnabled = isLogin;//插入分件按鈕

            login.Visibility = (isLogin == true) ? Visibility.Collapsed : Visibility.Visible;
            logout.Visibility = (isLogin == true) ? Visibility.Visible : Visibility.Collapsed;
            userInfoLogout.Visibility = (isLogin == true) ? Visibility.Visible : Visibility.Collapsed;

            if (isLogin == true)
            {
                Frame LoginView = (Frame)MyMainWindow.FindName("LoginView");
                LoginView.Visibility = Visibility.Collapsed;
            }
            else
            {
                Button btn_Search = (Button)MyMainWindow.FindName("btn_Search");
                Button btn_Edit = (Button)MyMainWindow.FindName("btn_Edit");
                Button btn_TreeView = (Button)MyMainWindow.FindName("btn_TreeView");
                btn_Search.IsEnabled = false;//搜尋資料按鈕
                btn_Edit.IsEnabled = false;//顯示已選擇的資料按鈕
                btn_TreeView.IsEnabled = false;//顯示數狀結構按鈕
            }

        }


        public void ChangeLoginImage(ToggleButton btn)
        {
            string uriSource;
            if (IsLogin == true)
            {
                if (!string.IsNullOrEmpty(PLM.Image_ID))
                    uriSource = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + $@"/Broadway/CADImage/LoginImage/{PLM.Image_ID}/{ConnInnovator.Image_Filename}";
                else
                    uriSource = @"pack://application:,,,/BCS.CADs.Synchronization;component/Images/CreatedBy.png";
            }
            else
                uriSource = @"pack://application:,,,/BCS.CADs.Synchronization;component/Images/DefaultAvatar.png";

            Uri uri= new Uri(uriSource,UriKind.RelativeOrAbsolute);
            ControlTemplate loginImage_CT = btn.Template;
            Ellipse LoginImageEllipse = (Ellipse)loginImage_CT.FindName("LoginImageEllipse", btn);
            LoginImageEllipse.Fill = new ImageBrush(new BitmapImage(uri));
            ClsSynchronizer.UserImageSource = uriSource;
        }

        private void ResetDBList()
        {
            try
            {
                var task = Task.Factory.StartNew(() => PLM.ListItems  = ClsSynchronizer.VmSyncCADs.GetDBList(PLM.Url).ListItems);
                task.ContinueWith((x) =>
                {
                    StartStopWait(false);
                }, TaskScheduler.FromCurrentSynchronizationContext()); 
                   
                   
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GridDetailView(string filename)
        {
            try
            {

                ObsSearchItems = new ObservableCollection<SearchItem>(_searchItems.Where(x => String.IsNullOrWhiteSpace(x.FileName) ==false));
                TreeSearchItems = new ObservableCollection<SearchItemsViewModel>();
                foreach (SearchItem searchItem in _searchItems.Where(x => x.FileName.ToLower() == filename.ToLower()))
                {
                    SearchItemsViewModel treeSearchItem = new SearchItemsViewModel();
                    treeSearchItem.Name = searchItem.FileName;
                    treeSearchItem.NodeSearchItem = searchItem;
                    TreeSearchItems.Add(treeSearchItem);

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private CollectionViewSource _listTemplatesView;
        private CollectionViewSource _plugInFunctionsView;


        private ObservableCollection<ClassTemplateFile> _listTemplates;
        public ObservableCollection<ClassTemplateFile> ListTemplates
        {
            get { return _listTemplates; }
            set
            {
                SetProperty(ref _listTemplates, value);
            }
        }


        private ObservableCollection<ClassItem> _listClassItems;
        public ObservableCollection<ClassItem> ListClassItems
        {
            get { return _listClassItems; }
            set
            {
                SetProperty(ref _listClassItems, value);
            }
        }

        private ClassItem _selectedClassItem;

        public ClassItem SelectedClassItem
        {
            get { return _selectedClassItem; }
            set {
                    SetProperty(ref _selectedClassItem, value);
                    ItemFilterSearch_SelectionChanged(value);
            }
        }


        private ObservableCollection<ClassPlugin> _classPlugins;
        public ObservableCollection<ClassPlugin> ClassPlugins
        {
            get { return _classPlugins; }
            set
            {
                SetProperty(ref _classPlugins, value);
            }
        }

        public ObservableCollection<SearchItem> ObsSearchItems
        {
            get { return ClsSynchronizer._vmObsSearchItems; }
            set
            {
                SetProperty(ref ClsSynchronizer._vmObsSearchItems, value, nameof(ObsSearchItems));
            }
        }

        private ObservableCollection<ItemMessage> _obsItemsMessage;
        public ObservableCollection<ItemMessage> ObsItemsMessage
        {
            get { return _obsItemsMessage; }
            set
            {
                SetProperty(ref _obsItemsMessage, value, nameof(ObsItemsMessage));
            }
        }
        
        private ObservableCollection<SearchItem> _obsSearchFilterItems;
        public ObservableCollection<SearchItem> ObsSearchFilterItems
        {
            get { return _obsSearchFilterItems; }
            set
            {
                SetProperty(ref _obsSearchFilterItems, value, nameof(ObsSearchFilterItems));
            }
        }


        private ObservableCollection<CADStructure> _cadStructure;
        public ObservableCollection<CADStructure> CadStructure
        {
            get { return _cadStructure; }
            set
            {
                SetProperty(ref _cadStructure, value, nameof(CadStructure));
            }
        }

        private ObservableCollection<SearchItemsViewModel> _treeSearchItems;
        public ObservableCollection<SearchItemsViewModel> TreeSearchItems
        {
            get { return _treeSearchItems; }
            set { SetProperty(ref _treeSearchItems, value, nameof(TreeSearchItems)); }
        }
        

        /// <summary>
        /// 取得Active CADs 所有結構圖檔
        /// </summary>
        /// <param name="win"></param>
        /// <param name="type"></param>
        public void SetSyncCADsListView(SyncType type)
        {

            OperationStart("SetSyncCADsListView");

            ClsSynchronizer.VmOperation = SyncOperation.QueryListItems;

            bool ret = false;
            var task = Task.Factory.StartNew(() => ret = SetSyncCADsListViewTaskScheduler(type));
            task.ContinueWith((x) =>
            {
                if (_searchItems != null) ObsSearchItems = new ObservableCollection<SearchItem>(_searchItems.Where(y => String.IsNullOrWhiteSpace(y.FileName) == false));
                if (_syncCADsListDataGrid == null) _syncCADsListDataGrid = new SyncCADsList();
                DataGrid gridSelectedItems = (DataGrid)_syncCADsListDataGrid.FindName("gridSelectedItems");

                int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
                if (ClsSynchronizer.IsActiveSubDialogView)
                    ClsSynchronizer.SyncSubListView[index.ToString()] = _syncCADsListDataGrid;
                else
                    ClsSynchronizer.SyncListView = _syncCADsListDataGrid;

                gridSelectedItems.ItemsSource = ObsSearchItems;

                var viewPage = (Frame)MyMainWindow.FindName("viewPage");
                DefaultSystemItemsDisplay();
                Button btn_Search = (Button)MyMainWindow.FindName("btn_Search");
                Button btn_Edit = (Button)MyMainWindow.FindName("btn_Edit");
                btn_Search.IsEnabled = true;
                btn_Edit.IsEnabled = true;

                viewPage.Visibility = Visibility.Visible;
                viewPage.Navigate(_syncCADsListDataGrid);

                ClsSynchronizer.VmMessages.Status = "End";

                CheckBox_AllIsChecked(false);
                StartStopWait(false);

            }, TaskScheduler.FromCurrentSynchronizationContext());



        }

        /// <summary>
        /// 取得Active CADs 所有結構圖檔(Task Scheduler)
        /// </summary>
        /// <param name="type"></param>
        public bool SetSyncCADsListViewTaskScheduler(SyncType type)
        {

            //ClsSynchronizer.DoEvents();

            ClsSynchronizer.VmOperation = SyncOperation.QueryListItems;

            

            List<SearchItem> searchItems = null;
            ClsSynchronizer.VmSyncCADs.GetCADStructure(type, ref searchItems);

            _searchItems = searchItems;
            ClsSynchronizer.SearchItemsList = _searchItems;

            ObsSearchItems = new ObservableCollection<SearchItem>();

            return true;
        }
        
    

        /// <summary>
        /// 預設系統顯示
        /// </summary>
        public void DefaultSystemItemsDisplay()
        {
            var LoginView = (Frame)MyMainWindow.FindName("LoginView");
            var SystemView = (Frame)MyMainWindow.FindName("SystemView");

            LoginView.Visibility = Visibility.Collapsed;
            SystemView.Visibility = Visibility.Collapsed;
            CheckBoxWPVisibility = Visibility.Visible;
        }



        /// <summary>
        /// 取得範本設定
        /// </summary>
        /// <param name="win"></param>
        public void AddFromTemplatesView()
        {


            ClsSynchronizer.VmOperation = SyncOperation.AddTemplates;
            OperationStart("AddFromTemplatesView");

            Dictionary<string, List<ClassTemplateFile>> classesTemplates = ClsSynchronizer.VmSyncCADs.GetClassesTemplates();

            ListTemplates = new ObservableCollection<ClassTemplateFile>();
            if (classesTemplates == null)
            {
                //DisplaySyncMessages(Visibility.Visible);
            }
            else
            {
                foreach (List<ClassTemplateFile> listClassTemplateFile in classesTemplates.Select(x => x.Value))
                {
                    foreach (ClassTemplateFile classTemplateFile in listClassTemplateFile)
                    {
                        //classTemplateFile.SpecialFeatures = SpecialFeatures.Color;
                        classTemplateFile.SpecialFeatures = SpecialFeatures.Color;//.Highlight ;
                        ListTemplates.Add(classTemplateFile);
                    }
                }
            }

            //@@@@@@@@@@@
            if (_addFromTemplateView == null) _addFromTemplateView = new AddFromTemplate();

            ClsSynchronizer.SyncListView = _addFromTemplateView;


            _listTemplatesView = (CollectionViewSource)_addFromTemplateView.FindResource("ListingDataView");
            if (_listTemplatesView.GroupDescriptions.Count() < 1)
            {
                var groupDescription = new PropertyGroupDescription { PropertyName = "ClassName" };
                _listTemplatesView.GroupDescriptions.Add(groupDescription);

            }
            
            _listTemplatesView.Source = ListTemplates;
            SelectedDirectoryGridVisibility = Visibility.Visible;

            Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
            rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName("newFormTemplateFile_SP")));
            var loginView = (Frame)MyMainWindow.FindName("LoginView");
            var systemView = (Frame)MyMainWindow.FindName("SystemView");
            
            systemView.Visibility = Visibility.Collapsed;
            loginView.Visibility = Visibility.Collapsed;


            var viewPage = (Frame)MyMainWindow.FindName("viewPage");
            ClearHistory(viewPage);
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_addFromTemplateView);

            ClsSynchronizer.VmMessages.Status  = "End";
        }


        public void ClearHistory(Frame viewPage)
        {
            if (!viewPage.CanGoBack && !viewPage.CanGoForward)
            {
                return;
            }

            var entry = viewPage.RemoveBackEntry();
            while (entry != null)
            {
                entry = viewPage.RemoveBackEntry();
            }

            viewPage.Navigate(new PageFunction<string>() { RemoveFromJournal = true });
        }


        public void OperationStart(string Operation)
        {
            ClsSynchronizer.VmMessages = new SyncMessages(ClsSynchronizer.VmFunction, ClsSynchronizer.VmOperation, Operation, "","Start");
            UpdateStatus("");

        }

        public void PlugInFunctionsView()
        {


            ClsSynchronizer.VmOperation = SyncOperation.None;

            OperationStart("PlugInFunctionsView");
            
            ClsSynchronizer.ActiveSearchItem = ClsSynchronizer.VmSyncCADs.GetActiveCADDocument();
            ClassPlugins = ClsSynchronizer.VmSyncCADs.GetClassPlugins(ClsSynchronizer.ActiveSearchItem.ClassName);
            ClsSynchronizer.ClassPlugins = ClassPlugins;

            if (_plugInFuncsView == null) _plugInFuncsView = new PlugInFuncs();
            ClsSynchronizer.SyncListView = _plugInFuncsView;

            _plugInFunctionsView = (CollectionViewSource)_plugInFuncsView.FindResource("PlugInFunctions");
            _plugInFunctionsView.Source = ClassPlugins;
            var viewPage = (Frame)MyMainWindow.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;

            Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");

            CheckBoxWPVisibility = Visibility.Collapsed;
            rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName("PlugIn_SP")));

            viewPage.Navigate(_plugInFuncsView);

        }

        public void LoadFromPLMView(Window win, bool isDialog)
        {
            ClsSynchronizer.VmFunction = SyncType.LoadFromPLM;
            ClsSynchronizer.VmOperation = SyncOperation.LoadListItems;

            LoadFromPLMView(win, isDialog,"", false ,false);
        }

        public void CopyToAddView(Window win, bool isDialog)
        {
            //ClsSynchronizer.VmFunction = SyncType.CopyToAddSearch;
            ClsSynchronizer.VmFunction = SyncType.CopyToAdd;
            ClsSynchronizer.VmOperation = SyncOperation.CopyToAddSearch;

            LoadFromPLMView(win, isDialog, "", false,true);
        }


        /// <summary>
        /// Load From PLM,Item Search,Item Search Dialog,Item Filter Search,Copy To Add
        /// </summary>
        /// <param name="win"></param>
        /// <param name="isDialog"></param>
        /// <param name="itemType"></param>
        /// <param name="isSub"></param>
        /// <param name="isCopyToAdd"></param>

        public void LoadFromPLMView(Window win, bool isDialog, string itemType, bool isSub, bool isCopyToAdd)
        {
            if (String.IsNullOrWhiteSpace(itemType)) itemType = ItemTypeName.CAD.ToString();

            string displayName = isCopyToAdd ? "CopyToAddView" : "LoadFromPLMView";
            string displayKey = isCopyToAdd ? "copyToAdd" : "loadFromPLM";
            OperationStart(displayName);


            ItemType itemTypeItem = (isDialog) ? ClsSynchronizer.VmSyncCADs.GetItemType(itemType, SearchType.Search) : ClsSynchronizer.VmSyncCADs.GetItemType(itemType, SearchType.CADRevisionSearch);

            if ((isDialog) == false) ClsSynchronizer.SearchItemTypeItem = itemTypeItem;

            ObsSearchItems = new ObservableCollection<SearchItem>();


            bool isNew = false;
            if (_ItemSearchView == null)
            {
                _ItemSearchView = new ItemSearch();
                isNew = true;
            }


            int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
            if (ClsSynchronizer.IsActiveSubDialogView)
                ClsSynchronizer.SyncSubListView[index.ToString()] = _ItemSearchView;
            //ClsSynchronizer.SyncSubListView = _ItemSearchView;
            else
                ClsSynchronizer.SyncListView = _ItemSearchView;

            if (isDialog == true)
            {
                ((TextBox)_ItemSearchView.FindName("CADdirectory")).Visibility = Visibility.Hidden;
                ((Button)_ItemSearchView.FindName("selectedDirectory")).Visibility = Visibility.Hidden;
            }
            else
            {
                ((Button)_ItemSearchView.FindName("doneSelecting")).Visibility = Visibility.Hidden;
                ((Button)_ItemSearchView.FindName("closeDialogWindow")).Visibility = Visibility.Hidden;

            }


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


            if (ClsSynchronizer.IsActiveSubDialogView) { 
                index = (ClsSynchronizer.NewSubSearchItem == null) ? -1 : ClsSynchronizer.NewSubSearchItem.Count() - 1;
                ClsSynchronizer.NewSubSearchItem[index.ToString()] = SelectedSearchItem;
                //ClsSynchronizer.NewSubSearchItem = SelectedSearchItem;
            }
            else
                ClsSynchronizer.NewSearchItem = SelectedSearchItem;


            ClsSynchronizer.RowStyle = gridSelectedItems.RowStyle;
            gridSelectedItems.RowStyle = RowStyleHeightzero();


            if (isDialog==false)
            {
                Button btn = (Button)MyMainWindow.FindName(displayKey);
                SetMenuButtonColor(displayKey);
                Rectangle rect = (Rectangle)MyMainWindow.FindName("IsCheckMark");
                rect.SetValue(Grid.RowProperty, Grid.GetRow((StackPanel)MyMainWindow.FindName(String.Format ("{0}_SP", displayKey))));
            }

            DefaultSystemItemsDisplay();

            ClsSynchronizer.ActiveWindow = (isDialog == true) ? win : MyMainWindow;
            var viewPage = (isDialog == true) ? (Frame)win.FindName("viewPage") : (Frame)MyMainWindow.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_ItemSearchView);

            if (isDialog == true && ClsSynchronizer.VmLibraryPaths.Count()>0)
            {
                Grid grid = (Grid)win.FindName("GridShowButtons");
                grid.Visibility = Visibility.Visible;
            }

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

        public void SyncFromPLMItemSearchView(Window win, bool isDialog)
        {

            if (_editPropertiesDataGrid == null) _editPropertiesDataGrid = new EditProperties();
            ClsSynchronizer.EditPropertiesView = _editPropertiesDataGrid;
            ClsSynchronizer.VmOperation = SyncOperation.EditorProperties;

            ItemType itemType = ClsSynchronizer.VmSyncCADs.GetItemType(ItemTypeName.CAD.ToString(), SearchType.Search);
            if (_ItemSearchView == null) _ItemSearchView = new ItemSearch();

            int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
            if (ClsSynchronizer.IsActiveSubDialogView)
                ClsSynchronizer.SyncSubListView[index.ToString()] = _ItemSearchView;
                //ClsSynchronizer.SyncSubListView = _ItemSearchView;
            else
                ClsSynchronizer.SyncListView = _ItemSearchView;

            ((TextBox)_ItemSearchView.FindName("CADdirectory")).Visibility = Visibility.Hidden;
            ((Button)_ItemSearchView.FindName("selectedDirectory")).Visibility = Visibility.Hidden;
            ((Button)_ItemSearchView.FindName("doneSelecting")).Visibility = Visibility.Hidden;
            ((Button)_ItemSearchView.FindName("closeDialogWindow")).Visibility = Visibility.Hidden;
            ((Button)_ItemSearchView.FindName("btn_Search")).Visibility = Visibility.Hidden;
            
            DataGrid gridSelectedItems = (DataGrid)_ItemSearchView.FindName("gridSelectedItems");

            int j = 0;
            List<string> checkPropertyList = new List<string>();

            ClassItem classItem = ClsSynchronizer.VmSyncCADs.GetClassItems().Where(x => String.IsNullOrWhiteSpace(x.Name)==false).FirstOrDefault();
            foreach (PLMProperty plmProperty in classItem.CsProperties)
            {
                
                if (String.IsNullOrWhiteSpace(plmProperty.Label)==false && isExistProperty(plmProperty))
                {
                    //要排除預設的屬性
                    ClsSynchronizer.VmCommon.AddDataGridColumn(gridSelectedItems, plmProperty, j);
                }
                j++;
            }

            gridSelectedItems.ItemsSource = ObsSearchItems;
            var viewPage = (Frame)win.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_ItemSearchView);


        }

        public void ItemFilterSearchView()
        {

            ClsSynchronizer.VmOperation = SyncOperation.EditorProperties;
            List<ClassItem> classeItems = ClsSynchronizer.VmSyncCADs.GetClassItems();

            ListClassItems = new ObservableCollection<ClassItem>();

            foreach (ClassItem classItem in classeItems.Where(x=>x.Name !=""))//.Where(x => x.Name)
            {
               SearchItem searchItem = ClsSynchronizer._vmObsSearchItems.Where(x => x.ClassName == classItem.Name && x.IsViewSelected == true).FirstOrDefault();
               if (searchItem != null) ListClassItems.Add(classItem);
            }
            
            ClassItem clsItem = (ListClassItems.Count == 0) ? classeItems.Where(x => String.IsNullOrWhiteSpace(x.Name)==false).FirstOrDefault() : ListClassItems[0];

            if (_itemFilterSearchView == null) _itemFilterSearchView = new ItemFilterSearch();
            DataGrid gridSelectedItems = (DataGrid)_itemFilterSearchView.FindName("gridSelectedItems");
            AddGridSelectedItemsColumn(gridSelectedItems, clsItem);

            ObservableCollection<SearchItem> ObsSearchFilterItems = new ObservableCollection<SearchItem>();
            ClsSynchronizer.ItemFilterSearchView = _itemFilterSearchView;
            ComboBox cboItemSearchClass = (ComboBox)_itemFilterSearchView.FindName("itemSearchClass");
            cboItemSearchClass.ItemsSource = ListClassItems;

            var viewPage = (Frame)MyMainWindow.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_itemFilterSearchView);

        }


        public void ItemFilterSearch_SelectionChanged(ClassItem clsItem)
        {


            ClsSynchronizer.VmOperation = SyncOperation.EditorProperties;
            List<ClassItem> classeItems = ClsSynchronizer.VmSyncCADs.GetClassItems();
            _itemFilterSearchView = ClsSynchronizer.ItemFilterSearchView;

            DataGrid gridSelectedItems = (DataGrid)_itemFilterSearchView.FindName("gridSelectedItems");
            if (clsItem != null) AddGridSelectedItemsColumn(gridSelectedItems, clsItem);

            ObservableCollection<SearchItem> ObsSearchFilterItems = new ObservableCollection<SearchItem>();
            foreach (SearchItem searchItem in ClsSynchronizer._vmObsSearchItems.Where(x => x.ClassName == clsItem.Name && x.IsViewSelected == true))
            {
                ObsSearchFilterItems.Add(searchItem);
            }

            gridSelectedItems.ItemsSource = ObsSearchFilterItems;
            var viewPage = (Frame)MyMainWindow.FindName("viewPage");
            viewPage.Visibility = Visibility.Visible;
            viewPage.Navigate(_itemFilterSearchView);

        }


        private void AddGridSelectedItemsColumn(DataGrid gridSelectedItems, ClassItem clsItem)
        {
            gridSelectedItems.Columns.Clear();
            ClsSynchronizer.VmCommon.AddDataGridImageColumn(gridSelectedItems);
            int j = 0;
            foreach (PLMProperty plmProperty in clsItem.CsProperties)
            {
                
                if (String.IsNullOrWhiteSpace(plmProperty.Label) == false && isExistProperty(plmProperty))
                {
                    //要排除預設的屬性
                    ClsSynchronizer.VmCommon.AddDataGridColumn(gridSelectedItems, plmProperty, j);
                }
                j++;
            }

        }



        private bool isExistProperty(PLMProperty plmProperty)
        {

            if (plmProperty.Name  == ClsSynchronizer.VmSyncCADs.ThumbnailProperty) return true;
            if (ObsSearchItems.Count == 0 && plmProperty.IsSyncCAD) return true;
            
            foreach (ClassItem classItem in ClsSynchronizer.VmSyncCADs.GetClassItems().Where(x => String.IsNullOrWhiteSpace(x.Name) ==false))
            {
                SearchItem searchItem = ObsSearchItems.Where(x => x.ClassName == classItem.Name).FirstOrDefault();
                if (searchItem == null) continue;
                PLMProperty property = classItem.CsProperties.Where(x => x.IsSyncCAD == true && x.Name == plmProperty.Name).FirstOrDefault();
                if (property != null) return true;
            }
            return false ;
        }

        public List<SearchItem> SelectedItems { get; set; } = new List<SearchItem>();
        public ICommand SelectedItemsCommand
        {
            get
            {
                return new GetSelectedItemsCommand(list =>
                {

                });
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

        private SearchItem _editPropertiesSelectedSearchItem;
        public SearchItem EditPropertiesSelectedSearchItem
        {
            get { return _editPropertiesSelectedSearchItem; }
            //set { SetProperty(ref _selectedListItem, value); }
            set {
                SetProperty(ref _editPropertiesSelectedSearchItem, value, nameof(EditPropertiesSelectedSearchItem));
                if (EditPropertiesSelectedSearchItem != null)
                {
                    ThumbnailImage thumbnailImage = new ThumbnailImage();
                    thumbnailImage.GetThumbnailImagePath(_editPropertiesSelectedSearchItem);
                    if (ClsSynchronizer.ViewFilePath != null)
                    {
                        ThumbnailImagePath = ClsSynchronizer.ViewFilePath;
                    }
                    SetFormContent(ClsSynchronizer.FormGrid, EditPropertiesSelectedSearchItem.PlmProperties);
                }

            }
        }

        private string _thumbnailImagePath;
        public string ThumbnailImagePath
        {
            get { return _thumbnailImagePath; }
            //set { SetProperty(ref _selectedListItem, value); }
            set
            {
                SetProperty(ref _thumbnailImagePath, value, nameof(ThumbnailImagePath));
            }
        }

        private ICommand _syncInsertItem { get; set; }
        public ICommand SyncInsertItem
        {
            get
            {
                _syncInsertItem = new RelayCommand((x) =>
                {
                    BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel menuItem = (BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel)x;
                    AddPopupMenuOperation(menuItem, ChangeType.Insert, false );
                });
                return _syncInsertItem;
            }
            private set
            {
                _syncInsertItem = value;
            }
        }

        private ICommand _syncReplaceItem { get; set; }
        public ICommand SyncReplaceItem
        {
            get
            {
                _syncReplaceItem = new RelayCommand((x) =>
                {
                    BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel menuItem = (BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel)x;
                    AddPopupMenuOperation(menuItem, ChangeType.Replacement, false );
                });
                return _syncReplaceItem;
            }
            private set
            {
                _syncInsertItem = value;
            }
        }

        private ICommand _syncReplaceAllItems { get; set; }
        public ICommand SyncReplaceAllItems
        {
            get
            {
                _syncReplaceAllItems = new RelayCommand((x) =>
                {
                    BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel menuItem = (BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel)x;
                    AddPopupMenuOperation(menuItem, ChangeType.Replacement, true );
                });
                return _syncReplaceAllItems;
            }
            private set
            {
                _syncInsertItem = value;
            }
        }

        private ICommand _syncCopyToAdd { get; set; }
        public ICommand SyncCopyToAdd
        {
            get
            {
                _syncCopyToAdd = new RelayCommand((x) =>
                {
                    BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel menuItem = (BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel)x;
                    AddPopupMenuOperation(menuItem, ChangeType.CopyToAdd, false);
                });
                return _syncCopyToAdd;
            }
            private set
            {
                _syncInsertItem = value;
            }
        }

        private ICommand _syncInsertSaveAs { get; set; }
        public ICommand SyncInsertSaveAs
        {
            get
            {
                _syncInsertSaveAs = new RelayCommand((x) =>
                {
                    BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel menuItem = (BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel)x;
                    AddPopupMenuOperation(menuItem, ChangeType.InsertSaveAs, false);
                });
                return _syncInsertSaveAs;
            }
            private set
            {
                _syncInsertItem = value;
            }
        }


        private void AddPopupMenuOperation(BCS.CADs.Synchronization.ViewModels.SearchItemsViewModel menuItem , ChangeType changeType, bool isReplaceAll)
        {
            try
            {
                string fileName = menuItem.Name;
                var vmFunction = ClsSynchronizer.VmFunction;
                var vmOperation = ClsSynchronizer.VmOperation;

                StructuralChange structuralChange = new StructuralChange();
                structuralChange.SourceFileName = fileName;
                structuralChange.TargetItemId = "";
                if (changeType == ChangeType.CopyToAdd)
                {
                    //string newDisplay = fileName.Substring(0, (fileName.Length - System.IO.Path.GetExtension(fileName).Length)) + "(1)";
                    //structuralChange.TargetFileName = newDisplay + System.IO.Path.GetExtension(fileName);
                    structuralChange.TargetFileName = ChangeToNewFileName(fileName);
                }

                structuralChange.Type = changeType;
                structuralChange.IsReplaceAll = isReplaceAll;
                structuralChange.Order = menuItem.Order;
                structuralChange.InstanceId = menuItem.InstanceId;

                ClsSynchronizer.CurrentDialog = "ItemSearchDialog";
                ClsSynchronizer.DialogReturnValue = "";

                bool ret = (changeType == ChangeType.CopyToAdd) ? ShowNewFileName(structuralChange.TargetFileName) : ShowItemSearchDialog();
                UpdateStructuralChangePartsLibrary(structuralChange, ClsSynchronizer.DialogReturnValue);
                if (changeType == ChangeType.InsertSaveAs && ret)
                {
                    //插入另存分件
                    //structuralChange.TargetItemId = ClsSynchronizer.DialogReturnValue;
                    structuralChange.TargetItemId = ClsSynchronizer.DialogReturnValue.Split((char)44)[0];
                    

                    fileName =(structuralChange.TargetItemId=="")? structuralChange.TargetFileName : ClsSynchronizer.VmSyncCADs.GetFileNameByCADItemId(structuralChange.TargetItemId);
                    if (fileName == "")
                    {
                        MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_TheSelectedCADItemHasNoDrawingFile"));
                        ret = false;
                    }else
                    {
                        //string newDisplay = fileName.Substring(0, (fileName.Length - System.IO.Path.GetExtension(fileName).Length)) + "(1)";
                        structuralChange.TargetFileName = ChangeToNewFileName(fileName);//  newDisplay + System.IO.Path.GetExtension(fileName);
                        ret = ShowNewFileName(structuralChange.TargetFileName);
                    }
                }
                ClsSynchronizer.VmFunction = vmFunction;
                ClsSynchronizer.VmOperation = vmOperation;
                if (ret)
                {
                    if (changeType == ChangeType.CopyToAdd|| changeType == ChangeType.InsertSaveAs)
                        structuralChange.TargetFileName = ClsSynchronizer.DialogReturnValue;
                    else
                        structuralChange.TargetItemId = ClsSynchronizer.DialogReturnValue.Split((char)44)[0];


                    _treeStructure = (TreeStructure)ClsSynchronizer.TreeStructureView;
                    _searchItems = ClsSynchronizer.SearchItemsList;

                    ClsSynchronizer.VmSyncCADs.SynStructuralChangeItems(_searchItems, structuralChange);
                    ShowTreeStructure(_treeStructure);
                    ClsSynchronizer.ItemStructureChanges.Add(structuralChange);
                }


                ClsSynchronizer.SyncDialogView = null;
                ClsSynchronizer.CurrentDialog = "";
            }
            catch (Exception ex)
            {
                ClsSynchronizer.SyncDialogView = null;
                ClsSynchronizer.CurrentDialog = "";
                MessageBox.Show (ex.Message);
            }

        }

        private string ChangeToNewFileName(string fileName)
        {

            return $"{fileName.Substring(0, (fileName.Length - System.IO.Path.GetExtension(fileName).Length))}{"(1)"}{System.IO.Path.GetExtension(fileName)}";
        }

        private void UpdateStructuralChangePartsLibrary(StructuralChange structuralChange,string value)
        {
            string[] arryValue =  value.Split((char)44);
            if (arryValue.Count() < 2) return;
            structuralChange.LibraryFileName = System.IO.Path.GetFileName(arryValue[1]);
            structuralChange.TargetFileName = structuralChange.LibraryFileName;
            structuralChange.LibraryPath = System.IO.Path.GetDirectoryName(arryValue[1]);
            structuralChange.TargetFilePath = structuralChange.LibraryPath;
            structuralChange.IsCommonPart = true;
        }


        private bool ShowItemSearchDialog()
        {
            string itemDialogType = ClsSynchronizer.ShowDialogItemType;
            ItemSearchDialog itemSearchDialog = new ItemSearchDialog();
            ClsSynchronizer.IsShowDialog = true;
            itemSearchDialog.Width = 800;
            itemSearchDialog.Height = 600;
            itemSearchDialog.Topmost = true;
            itemSearchDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ClsSynchronizer.DialogReturnValue = "";
            itemSearchDialog.ShowDialog();
            ClsSynchronizer.IsShowDialog = false ;
            ClsSynchronizer.IsSyncCommonPageView = false;
            ClsSynchronizer.ShowDialogItemType = itemDialogType;
            if (String.IsNullOrWhiteSpace(ClsSynchronizer.DialogReturnValue) == false) return true;
            return false;
        }

        private bool ShowNewFileName(string fileName)
        {
            string itemDialogType = ClsSynchronizer.ShowDialogItemType;
            ClsSynchronizer.IsShowDialog = true;
            NewFileName newFileName = new NewFileName(fileName);
            //NewFileName newFileName = new NewFileName();
            newFileName.Width = 400;
            newFileName.Height = 200;
            newFileName.Topmost = true;
            newFileName.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ClsSynchronizer.DialogReturnValue = "";
            newFileName.ShowDialog();
            ClsSynchronizer.IsShowDialog =false ;
            ClsSynchronizer.ShowDialogItemType = itemDialogType;
            if (String.IsNullOrWhiteSpace(ClsSynchronizer.DialogReturnValue) == false) return true;
            return false;
        }



        public class GetSelectedItemsCommand : ICommand
        {
            public GetSelectedItemsCommand(Action<object> action)
            {
                _action = action;
            }

            private readonly Action<object> _action;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                _action(parameter);
            }
        }






        private bool _isPrintPopupOpen;

        public bool IsPrintPopupOpen
        {
            get { return _isPrintPopupOpen; }

            set
            {
                if (_isPrintPopupOpen == value)
                {
                    return;
                }

                _isPrintPopupOpen = value;
                OnPropertyChanged(nameof(IsPrintPopupOpen));
            }
        }

    }


    public class PopupMenuItem : NotifyPropertyBase
    {

        private ICommand _syncInsertItem { get; set; }
        public ICommand SyncInsertItem
        {
            get
            {
                _syncInsertItem = new RelayCommand((x) =>
                {
                    //MessageBox.Show("SyncInsertItem");
                });
                return _syncInsertItem;
            }
        }

    }

    public class StringImageToResource : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            switch (ClsSynchronizer.VmOperation)
            {
                case SyncOperation.CADStructure:
                    TreeStructure treeStructureView = (TreeStructure)ClsSynchronizer.TreeStructureView;
                    string resourceValue = value.ToString();
                    resourceValue = (resourceValue.ToLower()=="true")? "checked" : (resourceValue.ToLower() == "false") ? "unchecked" : resourceValue;
                    return treeStructureView.FindResource(resourceValue);
                case SyncOperation.AddTemplates:
                case SyncOperation.EditorProperties:
                    EditProperties editorPropertiesView = (EditProperties)ClsSynchronizer.EditPropertiesView;
                    return editorPropertiesView.FindResource(value as string);

                default:
                    int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
                    SyncCADsList syncCADsList = (ClsSynchronizer.IsActiveSubDialogView) ? (SyncCADsList)ClsSynchronizer.SyncSubListView[index.ToString()] : (SyncCADsList)ClsSynchronizer.SyncListView;
                    //SyncCADsList syncCADsList = (ClsSynchronizer.IsActiveSubDialogView) ? (SyncCADsList)ClsSynchronizer.SyncSubListView: (SyncCADsList)ClsSynchronizer.SyncListView;
                    return syncCADsList.FindResource(value as string);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ThumbnailToPathConverter : IValueConverter
    {
        public string ImagePath
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            string thumbnail = value.ToString();
            ImagePath = ClsSynchronizer.VmSyncCADs.GetImageFullName(thumbnail);
             
            if (String.IsNullOrWhiteSpace(ImagePath)) ImagePath= @"pack://application:,,,/BCS.CADs.Synchronization;component/Images/White.bmp";
            return ImagePath;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class AddFromTemplateStringImageToResource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ClsSynchronizer.VmSyncCADs.GetClassItems().Where(x => x.Name == value as string).Select(x => x.ThumbnailFullName).FirstOrDefault();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ViewImageFile :  NotifyPropertyBase,IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            string viewFilePath = (ClsSynchronizer.ViewFilePath!="")? ClsSynchronizer.ViewFilePath:@"pack://application:,,,/BCS.CADs.Synchronization;component/Images/Part.png";
            return viewFilePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class ListClassItemsChanged : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    

    public class SyncCADsListStringImageToResource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = (ClsSynchronizer.SyncSubListView == null) ? -1 : ClsSynchronizer.SyncSubListView.Count() - 1;
            SyncCADsList syncCADsList = (ClsSynchronizer.IsActiveSubDialogView) ? (SyncCADsList)ClsSynchronizer.SyncSubListView[index.ToString()] : (SyncCADsList)ClsSynchronizer.SyncListView;
            //SyncCADsList syncCADsList = (ClsSynchronizer.IsActiveSubDialogView) ? (SyncCADsList)ClsSynchronizer.SyncSubListView : (SyncCADsList)ClsSynchronizer.SyncListView;
            return syncCADsList.FindResource(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ItemsMessageStringImageToResource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ItemsMessage syncMessagesView = (ItemsMessage)ClsSynchronizer.SyncMessagesView;
            return syncMessagesView.FindResource(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class BooleanToCollapsedVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            bool input = (bool)value;
            return (input) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class StyleConverter : IValueConverter
    {

        private StylesConverter _stylesConverter = new StylesConverter();

        private Dictionary<string, ResourceDictionary> _resourceDictionary = new Dictionary<string, ResourceDictionary>();

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return _stylesConverter.GetStyle(_resourceDictionary, value);

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public StyleConverter()
        {
            if (_resourceDictionary.Count() == 0)
            {
                ResourceDictionary resourceDictionary = _stylesConverter.StyleGridConverter(_resourceDictionary, false);
            }
        }

    }

    public class StyleGridConverter : IValueConverter
    {
        private StylesConverter _stylesConverter = new StylesConverter();
        private Dictionary<string, ResourceDictionary> _resourceDictionary = new Dictionary<string, ResourceDictionary>();

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return null;

            return _stylesConverter.GetStyle(_resourceDictionary, value);

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public StyleGridConverter()
        {
            if (_resourceDictionary.Count() == 0)
            {

                ResourceDictionary resourceDictionary = _stylesConverter.StyleGridConverter(_resourceDictionary,true);
            }
        }

    }

    public class ItemStyleConverter : IValueConverter
    {
        public ResourceDictionary resourceDictionary
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Utility/ItemPicker.xaml");
            resourceDictionary.MergedDictionaries.Add(resourceDictionary);
            return resourceDictionary;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    internal class StylesConverter
    {

        public Style GetStyle(Dictionary<string, ResourceDictionary> resDictionary, object value)
        {

            string styleValue = "tbListStyle";
            switch (value.ToString())
            {
                case "tbCalendarStyle":
                case "date":
                    styleValue = "tbCalendarStyle";
                    break;
                case "list":
                case "filter list":
                case "tbListStyle":
                    break;
                case "item":
                    styleValue = "tbItemStyle";
                    break;
                case "revision":
                    styleValue = "tbListRevStyle";
                    break;
                default:
                    return null;
            }


            ResourceDictionary resourceDictionary = resDictionary.Where(x => x.Key == styleValue).Select(x => x.Value).Single();
            if (resourceDictionary == null) return null;
            Style newStyle = (Style)resourceDictionary[styleValue];
            return newStyle;
        }

        public ResourceDictionary StyleGridConverter(Dictionary<string, ResourceDictionary> resDictionary,bool isStyleGrid)
        {

            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Utility/ListPicker.xaml");
            resDictionary.Add("tbListStyle", resourceDictionary);

            resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Utility/DatePicker.xaml");
            resDictionary.Add("tbCalendarStyle", resourceDictionary);

            resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = (isStyleGrid) ? new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Utility/GridItemPicker.xaml") : new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Utility/ItemPicker.xaml");
            resDictionary.Add("tbItemStyle", resourceDictionary);

            resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Utility/RevisionListPicker.xaml") ;
            resDictionary.Add("tbListRevStyle", resourceDictionary);
            

            resourceDictionary.MergedDictionaries.Add(resourceDictionary);
            return resourceDictionary;
        }
    }



    public class SearchItemsViewModel : NotifyPropertyBase
    {

        private ObservableCollection<SearchItemsViewModel> _child;
        public ObservableCollection<SearchItemsViewModel> Child
        {
            get { return _child; }
            set { SetProperty(ref _child, value, nameof(Child)); }
            
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set {
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
                    SearchItemsViewModel sonSearchItem = new SearchItemsViewModel();
                    sonSearchItem.Name = cadStructure.Child.FileName;
                    
                    sonSearchItem.Order = cadStructure.Order;
                    sonSearchItem.InstanceId = cadStructure.InstanceId;

                    PLMProperty property = cadStructure.Child.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) == false).SingleOrDefault();
                    string displayName = (property != null) ? property.DisplayValue : System.IO.Path.GetFileNameWithoutExtension(cadStructure.Child.FileName);
                    sonSearchItem.DisplayName = (sonSearchItem.InstanceId != "") ? String.Format (displayName + "<{0}>", sonSearchItem.InstanceId) : displayName;
                    
                    sonSearchItem.ClassName = cadStructure.Child.ClassName;
                    sonSearchItem.ClassThumbnail = cadStructure.Child.ClassThumbnail;
                    sonSearchItem.RestrictedStatus = cadStructure.Child.RestrictedStatus;
                    sonSearchItem.VersionStatus = cadStructure.Child.VersionStatus;
                    sonSearchItem.AccessRights = cadStructure.Child.AccessRights;
                    sonSearchItem.Thumbnail = cadStructure.Child.Thumbnail;// ClsSynchronizer.VmSyncCADs.GetImageFullName(cadStructure.Child.Thumbnail);

                    sonSearchItem.OperationType = cadStructure.OperationType;

                    sonSearchItem.IsChecked = cadStructure.Child.IsViewSelected;
                    if (sonSearchItem.IsChecked == true && sonSearchItem.OperationType==0)
                    {
                        sonSearchItem.IsReplacement = true;
                        sonSearchItem.IsCopyToAdd =true;
                        if (sonSearchItem.ClassName != "Assembly") { sonSearchItem.IsInsert = false; sonSearchItem.IsInsertSaveAs = false; }
                        }
                    else
                    {
                        sonSearchItem.IsInsert = false;
                        sonSearchItem.IsInsertSaveAs = false;
                        sonSearchItem.IsReplacement = false;
                        sonSearchItem.IsCopyToAdd = false;
                    }

                    if (sonSearchItem.IsChecked==true)
                    {
                        
                        property = cadStructure.Child.PlmProperties.Where(x => x.Name == "bcs_added_filename" && String.IsNullOrWhiteSpace(x.DisplayValue) ==false).SingleOrDefault();
                        if (property != null)
                        {
                            
                            sonSearchItem.DisplayName = (String.IsNullOrWhiteSpace(sonSearchItem.InstanceId) ==false ) ? String.Format(property.DisplayValue + "<{0}>", sonSearchItem.InstanceId)  : property.DisplayValue ;
                            sonSearchItem.OperationType = 4;
                        }
                    }
                    

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

        public SearchItemsViewModel()
        {
            Child = new ObservableCollection<SearchItemsViewModel>();
        }
    }



    internal class DataGridDateTimeColumn : DataGridBoundColumn
    {
        public DataGridDateTimeColumn(DataGridBoundColumn column)
        {
            Header = column.Header;
            Binding = (Binding)column.Binding;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock();
            BindingOperations.SetBinding(control, TextBlock.TextProperty, Binding);
            return control;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new DatePicker();
            control.PreviewKeyDown += Control_PreviewKeyDown;
            BindingOperations.SetBinding(control, DatePicker.SelectedDateProperty, Binding);
            BindingOperations.SetBinding(control, DatePicker.DisplayDateProperty, Binding);
            return control;
        }

        private void Control_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                DataGridOwner.CommitEdit();
            }
        }
    }

}
