#region "                   名稱空間"
using BCS.CADs.Synchronization.CommandModel;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Xml.Linq;
#endregion

namespace BCS.CADs.Synchronization.Services
{
    public class SelectLangs
    {

        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 元件路徑
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        #endregion

        #region "                   方法"
        public static ObservableCollection<LanguageList> GetAllLang()
        {

            //var resourceDictionary = new  Uri("pack://application:,,,/BCS.CADs.Synchronization;Component/Lang/zh-CN.xaml",UriKind.Relative);
            string strAssemblyPath = AssemblyDirectory;
            var _DefaultPath = Path.Combine(strAssemblyPath, "Lang");
            ////string path = Assembly.GetExecutingAssembly().Location;
            //var _DefaultPath = Path.Combine(Path.GetDirectoryName(path), "Lang");
            string[] files = Directory.GetFiles(_DefaultPath);
            var s = files.Select(a => new LanguageList { Value = a.Substring(a.LastIndexOf(@"\") + 1, a.LastIndexOf(".") - a.LastIndexOf(@"\") - 1), Label = new CultureInfo(a.Substring(a.LastIndexOf(@"\") + 1, a.LastIndexOf(".") - a.LastIndexOf(@"\") - 1)).DisplayName });
            var langs = new ObservableCollection<LanguageList>(s);

            return langs;
        }
        #endregion
    }
}
