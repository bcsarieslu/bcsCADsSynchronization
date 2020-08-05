#region "                   名稱空間"
using BCS.CADs.Synchronization.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;


using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
#endregion


namespace BCS.CADs.Synchronization.ViewModels
{
    public class NewFileNameViewModel : NotifyPropertyBase
    {
        #region "                   宣告區
        private readonly dynamic _view;
        
        #endregion

        #region "                   進入區
        public NewFileNameViewModel()
        {

        }

        public NewFileNameViewModel(dynamic view, string newFileName)
        {
            _view = view;
            PLM = new ConnPLM();
            PLM.NewFileName = newFileName;
        }

        #endregion

        #region "                   屬性區
        private ConnPLM _Plm;
        public ConnPLM PLM
        {
            get { return _Plm; }
            //set { SetProperty(ref _Plm, value); }
            set { SetProperty(ref _Plm, value, nameof(PLM)); }
        }

        private ICommand _closeDialogWindow { get; set; }
        public ICommand CloseDialogWindow
        {
            get
            {
                _closeDialogWindow = new RelayCommand((x) =>
                {
                    Window win = (Window)_view;
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

                    Window win = (Window)_view;
                    ClsSynchronizer.DialogReturnValue = PLM.NewFileName;
                    win.Close();

                });
                return _done;
            }
        }
        #endregion





    }


}
