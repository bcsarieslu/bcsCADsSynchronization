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
#endregion " 

namespace BCS.CADs.Synchronization.ViewModels
{
    class PartsLibrarySearchDialogViewModel : NotifyPropertyBase
    {
        #region "                   宣告區

        #endregion


        #region "                   進入區
        public PartsLibrarySearchDialogViewModel()
        {
            ListLibraryPaths = ClsSynchronizer.VmPartsLibrary.Paths;
        }
        #endregion " 

        #region "                   屬性區
        public dynamic SetView
        {
            set
            {
                ClsSynchronizer.SyncCommonDialogView = (dynamic)value;
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
            MessageBox.Show(value.Path);
            //throw new NotImplementedException();
        }

        private ICommand _closeDialogWindow { get; set; }
        public ICommand CloseDialogWindow
        {
            get
            {
                _closeDialogWindow = new RelayCommand((x) =>
                {
                    Window win = (Window)ClsSynchronizer.SyncCommonDialogView;
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

                    Window win = (Window)ClsSynchronizer.SyncCommonDialogView;

                    //TextBox txtSelectedItemId = (TextBox)win.FindName("selectedItemId");
                    //if (String.IsNullOrWhiteSpace(txtSelectedItemId.Text))
                    //{
                    //    MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected")); return;
                    //}

                    //ClsSynchronizer.DialogReturnValue = txtSelectedItemId.Text;
                    win.Close();

                });
                return _done;
            }
        }





        #endregion " 

        #region "                   方法區
        public void ShowSearchDialog()
        {
            ClsSynchronizer.DialogReturnValue = "";
            ClsSynchronizer.DialogReturnKeyedName = "";
            //ListLibraryPaths = ClsSynchronizer.VmPartsLibrary.Paths;
        }
        #endregion





    }
}
