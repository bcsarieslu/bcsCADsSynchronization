using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.ViewModels;
using BCS.CADs.Synchronization.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BCS.CADs.Synchronization
{
    public class SyncView
    {

        #region "                   宣告區"
        private MainWindow _syncMain;
        private string _cadSoftware;
        #endregion

        #region "                   進入點"

        #endregion

        #region "                   屬性"
        /// <summary>
        /// CAD軟體名稱
        /// </summary>
        public string CADSoftware {
            set{ _cadSoftware = value;}
        }

        #endregion

        #region "                   事件"
        
        #endregion

        #region "                   方法"

        /// <summary>
        /// 顯示主畫面
        /// </summary>
        public void MainWindow()
        {
            try
            {
                var cache = MyCache.CacheInstance;

                if (cache["MainWindow"] == null)
                    _syncMain = new MainWindow();
                else
                    _syncMain = (MainWindow)cache["MainWindow"];
                    //System.Diagnostics.Debugger.Break();
                    dynamic DataContext = _syncMain.DataContext;

                if (DataContext != null)
                {
                    DataContext.ResetMainWindowDefaultValues();
                    DataContext.RecentFileVM = new RecentFileViewModel();
                    DataContext.CheckBoxWPVisibility = Visibility.Collapsed;
                    DataContext.RecentFileViewVisibility = Visibility.Visible;
                    DataContext.RecentFileVM.RecentFile = DataContext.RecentFile.ReadRecentFile();
                }
                


                _syncMain.Topmost = true;
                _syncMain.CADSoftware = _cadSoftware;
                _syncMain.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                _syncMain.MinHeight = 700;
                _syncMain.MinWidth = 1200;
                _syncMain.WindowState = WindowState.Normal;
                _syncMain.ShowDialog();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        #endregion

        #region "                   方法 (內部)"


        #endregion
    }
}
