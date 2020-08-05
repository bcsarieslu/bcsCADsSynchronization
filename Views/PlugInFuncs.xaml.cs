using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// PlugInFuncs.xaml 的互動邏輯
    /// </summary>
    public partial class PlugInFuncs : Page
    {
        public PlugInFuncs()
        {
            InitializeComponent();
        }

        private void OnWrapPanel(object sender, RoutedEventArgs e)
        {

        }
    }

    //public class PathConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        string path = value.ToString();
    //        if (path.StartsWith("\\") {
    //            path = path.Substring(1);
    //        }

    //        //return Path.Combine("whateveryourbasepathis", path);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
