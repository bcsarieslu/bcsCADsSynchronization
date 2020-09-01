using BCS.CADs.Synchronization.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace BCS.CADs.Synchronization.ViewModels
{
    public class RecentFileViewModel : NotifyPropertyBase
    {
        private static ObservableCollection<RecentFileProperties> _recentFile;
        public ObservableCollection<RecentFileProperties> RecentFile
        {
            get { return _recentFile; }
            set
            {
                SetProperty(ref _recentFile, value, "RecentFile");
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
