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
            this.NewFileName = newFileName;
            //LinkPLM = new ConnPLM();
            //LinkPLM.NewFileName = newFileName;

        }

        //public dynamic SetView
        //{
        //    set
        //    {
        //        ClsSynchronizer.SyncCommonPageView = (dynamic)value;
        //    }

        //}

        #endregion

        #region "                   屬性區

        /// <summary>
        /// 新的圖檔名稱
        /// </summary>
        //public string Url { get; set; }
        private string _newFileName = "";
        public string NewFileName
        {
            get { return _newFileName; }
            set
            {
                SetProperty(ref _newFileName, value, nameof(NewFileName));
            }
        }

        /*
        private ConnPLM _linkPLM;
        public ConnPLM LinkPLM
        {
            get { return _linkPLM; }
            //set { SetProperty(ref _Plm, value); }
            set { SetProperty(ref _linkPLM, value, nameof(LinkPLM)); }
        }
        */

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
                    //ClsSynchronizer.DialogReturnValue = PLM.NewFileName;
                    //ClsSynchronizer.SubDialogReturnValue = LinkPLM.NewFileName;
                    ClsSynchronizer.DialogReturnValue = this.NewFileName;
                    win.Close();

                });
                return _done;
            }
        }
        #endregion





    }


}
