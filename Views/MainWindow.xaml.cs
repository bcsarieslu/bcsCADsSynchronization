using BCS.CADs.Synchronization.CommandModel;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.ViewModels;
using LoginWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinResizer;


using static BCS.CADs.Synchronization.CADsSynchronizer;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {

        #region "                   宣告區"
        private string _cadSoftware;

        //public SyncEventHandler OpenFilesHandler;
        //public SyncEventHandler AddTemplateHandler;
        //public SyncEventHandler AddFromTemplateHandler;
        //public SyncEventHandler ExportPropertyFileHandler;
        //public SyncEventHandler ItemLockedHandler;
        //public SyncEventHandler LoadFromPLMHandler;
        //public SyncEventHandler LoginHandler;
        //public SyncEventHandler SyncToPLMPropertiesHandler;
        //public SyncEventHandler SyncToPLMStructureHandler;

        //public SyncEventHandler GetActiveCADStructureHandler;
        //public SyncEventHandler GetSelectedCADsPropertiesHandler;
        //public SyncEventHandler UpdateCADsPropertiesHandler;

        ////public SyncEventHandler SyncToPLMStructureHandler;
        //public SyncEventHandler SyncFromPLMPropertiesHandler;
        //public SyncEventHandler SyncFromPLMStructureHandler;

        //bool check_close = true;
        bool window_size_max = true;
        #endregion


        #region "                   進入點"
        public MainWindow()
        {
            InitializeComponent();

            var cache = MyCache.CacheInstance;
            CultureInfo culture1 = CultureInfo.CurrentCulture;
            string languagePath = $"pack://application:,,,/BCS.CADs.Synchronization;Component/Lang/{culture1.Name}.xaml";
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri(languagePath);

            ClsSynchronizer.Language = culture1.Name;

            this.Resources.MergedDictionaries.Add(resourceDictionary);

            ClsSynchronizer.VmSyncCADs.CADSoftware = _cadSoftware;
            //LoadLanguage();
            WindowResizer winr = new WindowResizer(this);
            winr.addResizerLeft(left);
            winr.addResizerLeftDown(leftdown);
            winr.addResizerRightDown(rightdown);
            winr.addResizerRight(right);
            winr.addResizerUp(up);
            winr.addResizerDown(down);
            winr.addResizerRightUp(righttop);
            winr.addResizerLeftUp(leftTop);

            Login login;
            if (cache["Login"] == null)
                login = new Login();
            else
                login = (Login)cache["Login"];

            Panel.SetZIndex(LoginView, 2);
            LoginView.Navigate(login);

            var recentFileView = new RecentFileView();
            recentFileView.DataContext = this.DataContext;
            recentFileView.Resources.MergedDictionaries.Add(resourceDictionary);
            RecentFileView.Navigate(recentFileView);
            Panel.SetZIndex(RecentFileView, 1);

            var mainVM = ((MainWindowViewModel)this.DataContext);
            mainVM.RecentFileVM.RecentFile = mainVM.RecentFile.ReadRecentFile();

            //LoadingAdorner.IsAdornerVisible = true;
            //ClsSynchronizer.LoadingAdornerView = LoadingAdorner;
            //ClsSynchronizer.MainWindows = this;

            dynamic mainwindow = new CacheItem("MainWindow", this);
            var loadingAdorner = new CacheItem("LoadingAdorner", LoadingAdorner);
            var loginWindow = new CacheItem("Login", login);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now.AddMinutes(30d);
            cache.Add(mainwindow, null);
            cache.Add(loadingAdorner, null);
            cache.Add(loginWindow, null);

            ClsSynchronizer.MainWindows = this;
        }



        #endregion


        #region "                   屬性"
        /// <summary>
        /// CAD軟體名稱
        /// </summary>
        public string CADSoftware
        {
            set
            {
                _cadSoftware = value;
                ClsSynchronizer.VmSyncCADs.CADSoftware = _cadSoftware;
            }
        }

        /// <summary>
        /// 使用者是否登錄
        /// </summary>
        //public bool IsActiveLogin {
        //    get {  return ClsSynchronizer.VmSyncCADs.IsActiveLogin;}
        //}

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"
        public void LoadLanguage()
        {
            CultureInfo currentCultureInfo = CultureInfo.CurrentCulture;
            ClsSynchronizer.Language = currentCultureInfo.Name;
            ResourceDictionary langRd = null;
            try
            {
                langRd = Application.LoadComponent(
                new Uri($@"..\Lang\{ClsSynchronizer.Language}.xaml ", UriKind.Relative)) as ResourceDictionary;
            }
            catch
            {
            }

            if (langRd != null)
            {
                this.Resources.MergedDictionaries.Add(langRd);
            }
            else
            {
                ClsSynchronizer.Language = "en-US";
                langRd = Application.LoadComponent(
                new Uri($@"..\Lang\{ClsSynchronizer.Language}.xaml ", UriKind.Relative)) as ResourceDictionary;
                this.Resources.MergedDictionaries.Add(langRd);
                //MessageBox.Show(cultureinfo);
            }
        }
        #endregion

        #region "                   方法(內部)"

        private void Show_LogIn(object sender, RoutedEventArgs e)
        {
            //Login login = new Login();
            //login.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //login.Owner = this;
            //login.ShowDialog();

            //判斷按到的是【是】還是【否】
            //if (login.DialogResult.HasValue && login.DialogResult.Value)
            //    MessageBox.Show("You pressed OK!");
            //else
            //    MessageBox.Show("You pressed Cancel!");
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

        private void btnActionSystemInformation_OnClick(object sender, RoutedEventArgs e)
        {
            var systemInformationWindow = new SystemInformationWindow();
            systemInformationWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            systemInformationWindow.Owner = this;
            systemInformationWindow.ShowDialog();
        }

        private void btnActionClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Thumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.Resources.MergedDictionaries.Remove(this.Resources.MergedDictionaries.Last());
            string lang = ((GetAllLang)e.AddedItems[0]).LangName as string;
            var ci = new CultureInfo(lang, false);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            LoadLanguage();
        }

        #endregion

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            //this.mydatagrid.Navigate(new EditPropertiesView());
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SyncCADsList _syncCADsListDataGrid = new SyncCADsList();
            DataGrid gridSelectedItems = (DataGrid)_syncCADsListDataGrid.FindName("gridSelectedItems");
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(gridSelectedItems.ItemsSource);
            view.Filter = DataFilter;
        }

        private bool DataFilter(object item)
        {
            if (String.IsNullOrEmpty(searchTextBox.Text))
                return true;
            else
                return ((item as SearchItem).FileName.IndexOf(searchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {

            ClsSynchronizer.VmSyncCADs.SelectFolder(CADdirectory.Text);
            CADdirectory.Text = ClsSynchronizer.VmDirectory;
        }
    }
}
