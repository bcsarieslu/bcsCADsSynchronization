#region "                   名稱空間"
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
    public class SearchItemStructureViewModel : NotifyPropertyBase
    {
        #region "                   宣告區

        #endregion

        #region "                   進入區
        public SearchItemStructureViewModel()
        {

        }

        #endregion

        #region "                   屬性區

        public dynamic SetView
        {
            set
            {
                //ClsSynchronizer.SyncCommonTreeView = (dynamic)value;
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





        #endregion

        #region "                   方法區

        public void ShowSearchItems(string itemType, string selectedValue)
        {
            
        }

        #endregion

        #region "                   方法區 (內部)


        #endregion
    }
}
