#region "                   名稱空間"
using BCS.CADs.Synchronization.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text.RegularExpressions;
using BCS.CADs.Synchronization.Models;
using System.Windows.Data;
using BCS.CADs.Synchronization.Views;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

#endregion " 

namespace BCS.CADs.Synchronization.ViewModels
{
    public class PartsLibrarySearchViewModel : NotifyPropertyBase
    {
        #region "                   宣告區

        #endregion


        #region "                   進入區
        public PartsLibrarySearchViewModel()
        {
            //ListLibraryPaths = ClsSynchronizer.VmPartsLibrary.Paths;
            ListLibraryPaths = ClsSynchronizer.VmLibraryPaths;
        }
        #endregion " 

        #region "                   屬性區
        //public bool IsPage { get; set; } = false ;

        public dynamic SetView
        {
            set
            {
                ClsSynchronizer.SyncCommonPageView = (dynamic)value;
            }

        }

        private ObservableCollection<LibraryPath> _listLibraryPaths;
        public ObservableCollection<LibraryPath> ListLibraryPaths
        {
            get { return _listLibraryPaths; }
            set
            {
                SetProperty(ref _listLibraryPaths, value);
            }
        }


        private LibraryPath _selectedLibraryPath;

        public LibraryPath SelectedLibraryPath
        {
            get { return _selectedLibraryPath; }
            set
            {
                SetProperty(ref _selectedLibraryPath, value);
                SelectedLibraryPath_SelectionChanged(value);
            }
        }

        private void SelectedLibraryPath_SelectionChanged(LibraryPath value)
        {
            //MessageBox.Show(value.Path);

            if (value == null) return;
            if (value.FileItems.Count() == 0)
            {
                List<ClassItem> classItems = ClsSynchronizer.VmSyncCADs.GetClassItems();           
                string[] extensions=  classItems.Where(x => x.Name == ClassName.Assembly.ToString() || x.Name == ClassName.Part.ToString()).Select(x=>x.Extension).ToArray();
                string filters = String.Join("|", extensions);

                var searchPattern = new Regex($@"$(?<=\.({filters}))", RegexOptions.IgnoreCase);
                string[] files = Directory.GetFiles(value.Path).Where(x => searchPattern.IsMatch(x)).ToArray();
                foreach (var fileItem in files)
                {
                    string extension = Path.GetExtension(fileItem);
                    ClassItem classItem = classItems.Where(x => (x.Name == ClassName.Assembly.ToString() || x.Name == ClassName.Part.ToString()) && x.Extension == extension.Substring(1).ToLower()).FirstOrDefault();
                    value.FileItems.Add(new LibraryFileInfo(Path.GetFileName(fileItem), classItem.Name, extension));
                }
                ClsSynchronizer.VmSyncCADs.UpdateLibraryPathFiles(value);

            }

            ObsFileItems = value.FileItems;
        }

        private ObservableCollection<LibraryFileInfo> _obsFileItems;
        public ObservableCollection<LibraryFileInfo> ObsFileItems
        {
            get { return _obsFileItems; }
            set
            {
                SetProperty(ref _obsFileItems, value);
                
            }
        }

        /// <summary>
        /// 被選到的項目
        /// </summary>
        private LibraryFileInfo _selectedFileItem;
        public LibraryFileInfo SelectedFileItem
        {
            get { return _selectedFileItem; }
            set { SetProperty(ref _selectedFileItem, value, nameof(SelectedFileItem)); }
        }



        private ICommand _closeDialogWindow { get; set; }
        public ICommand CloseDialogWindow
        {
            get
            {
                _closeDialogWindow = new RelayCommand((x) =>
                {
                    //Window win = (Window)ClsSynchronizer.SyncCommonPageView;
                    Window win = (ClsSynchronizer.IsSyncCommonPageView) ? (Window)ClsSynchronizer.SyncDialogView : (Window)ClsSynchronizer.SyncCommonPageView;
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


                    TextBox txtSelectedPath = (ClsSynchronizer.IsSyncCommonPageView) ? (TextBox)((Page)ClsSynchronizer.SyncCommonPageView).FindName("selectedPath"): (TextBox)((Window)ClsSynchronizer.SyncCommonPageView).FindName("selectedPath");
                    TextBox txtSelectedfile = (ClsSynchronizer.IsSyncCommonPageView) ? (TextBox)((Page)ClsSynchronizer.SyncCommonPageView).FindName("selectedfile") : (TextBox)((Window)ClsSynchronizer.SyncCommonPageView).FindName("selectedfile");
                    TextBox txtSelectedItemId = (ClsSynchronizer.IsSyncCommonPageView) ? (TextBox)((Page)ClsSynchronizer.SyncCommonPageView).FindName("selectedItemId") : (TextBox)((Window)ClsSynchronizer.SyncCommonPageView).FindName("selectedItemId");

                    if (String.IsNullOrWhiteSpace(txtSelectedfile.Text))
                    {
                        MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected")); return;
                    }

                    //ClsSynchronizer.DialogReturnValue = $"{Path.Combine(txtSelectedPath.Text, txtSelectedfile.Text)},{txtSelectedItemId.Text}";
                    ClsSynchronizer.DialogReturnValue = $"{txtSelectedItemId.Text},{Path.Combine(txtSelectedPath.Text, txtSelectedfile.Text)}";
                    Window win = (ClsSynchronizer.IsSyncCommonPageView) ? (Window)ClsSynchronizer.SyncDialogView : (Window)ClsSynchronizer.SyncCommonPageView;
                    win.Close();

                });
                return _done;
            }
        }

        private ICommand _showCommand;
        public ICommand GridFieldClickedCommand
        {
            get
            {

                _showCommand = _showCommand ?? new RelayCommand((x) =>
                {

                    //Window win = (Window)ClsSynchronizer.SyncCommonPageView;
                    Page page = null;
                    Window win = null;
                    if (ClsSynchronizer.IsSyncCommonPageView)
                        page = (Page)ClsSynchronizer.SyncCommonPageView;
                    else
                        win = (Window)ClsSynchronizer.SyncCommonPageView;

                    LibraryFileInfo libraryFileInfo = x as LibraryFileInfo;
                    if (libraryFileInfo != null)
                    {
                        

                        ClsSynchronizer.ViewFilePath = ClsSynchronizer.VmSyncCADs.GetImageFullName(libraryFileInfo.Thumbnail);
                        if (String.IsNullOrWhiteSpace(ClsSynchronizer.ViewFilePath)) ClsSynchronizer.ViewFilePath = @"pack://application:,,,/BCS.CADs.Synchronization;component/Images/White.bmp";

                        Canvas canvas = (ClsSynchronizer.IsSyncCommonPageView) ? (Canvas)page.FindName("DialogCanvasViewFile"): (Canvas)win.FindName("DialogCanvasViewFile");
                        TextBlock positionUse = (ClsSynchronizer.IsSyncCommonPageView) ? (TextBlock)page.FindName("positionUse"): (TextBlock)win.FindName("positionUse");

                        canvas.Visibility = Visibility.Visible;
                        ViewFile viewFile = (ViewFile)canvas.FindName("viewFile");

                        Label lb = (Label)ClsSynchronizer.MainWindows.FindName("scaleLabel");
                        Point pointToWindow = Mouse.GetPosition(positionUse);

                        double size = (double.Parse(lb.Content.ToString()) / 100);
                        Canvas.SetLeft(viewFile, pointToWindow.X + (30 * size));
                        Canvas.SetTop(viewFile, pointToWindow.Y - (80 * size));
                        if (viewFile != null)
                        {
                            Image image = (Image)viewFile.FindName("imageFile");
                            if (image != null) image.Source = new BitmapImage(new Uri(ClsSynchronizer.ViewFilePath));
                        }
                        Storyboard myStoryboard = new Storyboard();
                        myStoryboard.Children.Add((ClsSynchronizer.IsSyncCommonPageView) ? (Storyboard)page.Resources["showMe"] : (Storyboard)win.Resources["showMe"]);
                        myStoryboard.Begin(viewFile);
                    }
                });


                return _showCommand;
            }
        }



        #endregion " 

        #region "                   方法區
        public void ShowSearchDialog()
        {
            ClsSynchronizer.DialogReturnValue = "";
            ClsSynchronizer.DialogReturnKeyedName = "";
            //ListLibraryPaths = ClsSynchronizer.VmPartsLibrary.Paths;
            //ListLibraryPaths = ClsSynchronizer.VmLibraryPaths;
            //ListLibraryPaths = ClsSynchronizer.VmPartsLibrary.Paths;
        }



        #endregion





    }

    public class PartsLibraryStringImageToResource : IValueConverter
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

}
