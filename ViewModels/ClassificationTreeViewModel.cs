
#region "                   名稱空間"
//using Microsoft.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
#endregion

namespace BCS.CADs.Synchronization.ViewModels
{
    public class ClassificationTreeViewModel: NotifyPropertyBase
    {

        #region "                   宣告區

        #endregion

        #region "                   進入區
        public ClassificationTreeViewModel()
        {

        }

        #endregion

        #region "                   屬性區

        public dynamic SetView
        {
            set
            {
                ClsSynchronizer.SyncCommonTreeView = (dynamic)value;
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

                    ClsSynchronizer.DialogReturnValue = txtSelectedItemId.Tag.ToString();
                    //ClsSynchronizer.DialogReturnDisplayValue = txtSelectedItemId.Tag.ToString();
                    ClsSynchronizer.DialogReturnDisplayValue = txtSelectedItemId.Text;
                    win.Close();

                });
                return _done;
            }
        }

        

        private ObservableCollection<ClassStructureItem> _classStructureItems;
        public ObservableCollection<ClassStructureItem> ClassStructureItems
        {
            get { return _classStructureItems; }
            set { SetProperty(ref _classStructureItems, value); }
        }

        private ClassStructureItem DefaultSelectedClassStructureItem { get; set; } = null;


        #endregion

        #region "                   方法區

        public void ShowClassificationItems(string itemType,string selectedValue)
        {

            //System.Diagnostics.Debugger.Break();
            XDocument xmlDoc = ClsSynchronizer.VmSyncCADs.GetClassification(itemType);

            ClassStructureItems = new ObservableCollection<ClassStructureItem>();

            if (xmlDoc == null) return;
            XElement xElement = xmlDoc.Elements("class").SingleOrDefault();
            if (xElement == null) return;

            ClassStructureItem classItem = new ClassStructureItem();
            classItem.Name = itemType;
            classItem.Id = xElement.Attribute("id")?.Value;
            //classItem.Index = index;
            classItem.Value = "";
            ClassStructureItems.Add(classItem);      
            RevSonClassificationItems(xElement, classItem,"", selectedValue);

            Window win = (Window)ClsSynchronizer.SyncCommonTreeView;
            TreeView treeView = (TreeView)win.FindName("treeStructureItems");
            treeView.ItemsSource = ClassStructureItems;
            if (DefaultSelectedClassStructureItem != null)
            {

                TextBox txtSelectedItemId = (TextBox)win.FindName("selectedItemId");
                txtSelectedItemId.Text = DefaultSelectedClassStructureItem.Name;
                txtSelectedItemId.Tag = DefaultSelectedClassStructureItem.Value;
            }
        }

        #endregion

        #region "                   方法區 (內部)

        private void RevSonClassificationItems(XElement xParElement, ClassStructureItem parClassItem, string value, string selectedValue)
        {
            foreach (XElement xElement in xParElement.Elements("class").OrderBy(x => x.Attribute("name").Value))
            {
                ClassStructureItem classItem = new ClassStructureItem();
                classItem.Name = xElement.Attribute("name")?.Value;
                classItem.Id = xElement.Attribute("id")?.Value;
                classItem.Value  = (value == "") ? classItem.Name : $"{value}/{classItem.Name}";
                //index++;
                //classItem.Index = index;
                if (selectedValue == classItem.Value) DefaultSelectedClassStructureItem = classItem;
                classItem.IsSelected = (selectedValue == classItem.Value)? true : false ;
                parClassItem.ClassItems.Add(classItem);               
                RevSonClassificationItems(xElement, classItem, classItem.Value, selectedValue);
            }
        }

        #endregion
    }


    public class ClassStructureItem : NotifyPropertyBase
    {


        public ClassStructureItem()
        {
            ClassItems = new ObservableCollection<ClassStructureItem>();
        }


        private ObservableCollection<ClassStructureItem> _classItems;
        public ObservableCollection<ClassStructureItem> ClassItems
        {
            get { return _classItems; }
            set { SetProperty(ref _classItems, value); }
        }



        //public string Id { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }


        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private bool  _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }



    }
    
}
