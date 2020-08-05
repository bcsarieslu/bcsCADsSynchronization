using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// AddFromTemplate.xaml 的互動邏輯
    /// </summary>
    public partial class AddFromTemplate : Page
    {
        public AddFromTemplate()
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            

            //Window x = (Window)ClsSynchronizer.MainWindows;
            //Canvas canvas = (Canvas)x.FindName("CanvasViewFile");
            //canvas.Visibility = Visibility.Visible;


            //<Canvas x:Name="CanvasViewFile"
            //CanvasViewFile.Visibility = Visibility.Visible;

            //var dialog = new FolderBrowserDialog();

            ////Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dialog.Description = "選取路徑";
            //dialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            ////Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            //dialog.SelectedPath = (CADdirectory.Text!="")? CADdirectory.Text: Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            //DialogResult result = dialog.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    CADdirectory.Text = dialog.SelectedPath;
            //    ClsSynchronizer.VmDirectory= dialog.SelectedPath;
            //}
        }

    }
}
