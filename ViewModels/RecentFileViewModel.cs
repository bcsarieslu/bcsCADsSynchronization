using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BCS.CADs.Synchronization.ViewModels
{
    public class RecentFileViewModel : NotifyPropertyBase
    {
        private MainWindow _myMainWindow;
        public MainWindow MyMainWindow { get { return (MainWindow)MyCache.CacheInstance["MainWindow"]; } set { _myMainWindow = value; } }

        private static ObservableCollection<RecentFileProperties> _recentFile;
        public ObservableCollection<RecentFileProperties> RecentFile
        {
            get { return _recentFile; }
            set
            {
                SetProperty(ref _recentFile, value, nameof(RecentFile));
            }
        }

        private static RecentFileProperties _selected;
        public RecentFileProperties Selected
        {
            get { return _selected; }
            set
            {
                SetProperty(ref _selected, value, nameof(Selected));
                if (Selected != null)
                {
                    ClsSynchronizer.VmSyncCADs.OpenFile(Selected.FilePath, Selected.FileName);
                    MyMainWindow.Hide();
                }
            }
        }

    }
    
    public static class Converter
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
    }
}
