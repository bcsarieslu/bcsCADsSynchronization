using BCS.CADs.Synchronization.Entities;
using BCS.CADs.Synchronization.ViewModels;
using Microsoft.Win32;
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

using System.Xml;
using System.Xml.Linq;

namespace BCS.CADs.Synchronization.Views
{
    /// <summary>
    /// ItemsMessage.xaml 的互動邏輯
    /// </summary>
    public partial class ItemsMessage : Page
    {
        public ItemsMessage()
        {
            InitializeComponent();
            ClsSynchronizer.VmSyncCADs.LoadLanguage(this);
        }

        private void SaveMessages_Click(object sender, RoutedEventArgs e)
        {

            //MessageBox.Show ("SaveMessages_Click");
            //System.Diagnostics.Debugger.Break();

            //openFileDialog.InitialDirectory = "c:\\";
            //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML Data|*.xml";
            saveFileDialog1.Title = "Save an XML File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            
            //if (saveFileDialog1.FileName != "")
            if (String.IsNullOrWhiteSpace(saveFileDialog1.FileName) ==false)
            {
                XDocument doc =
                    new XDocument(
                        new XElement("sections",
                            new XElement("item", new XAttribute("Function", ClsSynchronizer.VmMessages.Function), 
                                                 new XAttribute("Operation", ClsSynchronizer.VmMessages.Operation),
                                                 new XAttribute("Status", ClsSynchronizer.VmMessages.Status),
                                                 new XAttribute("Name", ClsSynchronizer.VmMessages.Name),
                                                 new XAttribute("Value", ClsSynchronizer.VmMessages.Value)
                                                 ),
                            new XElement("section",
                                ClsSynchronizer.VmMessages.ItemMessages.Select(x => new XElement("item", new XAttribute("Time", x.Time),
                                                                                    new XAttribute("IsError", x.IsError),
                                                                                    new XAttribute("Status", x.Status),
                                                                                    new XAttribute("Name", x.Name),
                                                                                    new XAttribute("Value", x.Value ),
                                                                                    new XAttribute("Detail", x.Detail)
                                          )
                                )
                        )
                    )
                );

                doc.Save(saveFileDialog1.FileName);

            }
        }
    }
}
