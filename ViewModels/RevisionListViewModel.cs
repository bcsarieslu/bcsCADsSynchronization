
#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BCS.CADs.Synchronization.Entities;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using BCS.CADs.Synchronization.Search;
using BCS.CADs.Synchronization.Classes;
using System.Windows.Controls;
using BCS.CADs.Synchronization.ConfigProperties;
using System.Windows.Data;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.Views;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
#endregion

namespace BCS.CADs.Synchronization.ViewModels
{
    public class RevisionListViewModel: NotifyPropertyBase
    {
        #region "                   宣告區
        //private dynamic _view;

        private ICommand _showCommand;
        #endregion

        #region "                   進入區
        public RevisionListViewModel()
        {

        }

        #endregion

        #region "                   屬性區

        public dynamic SetView {
            set
            {
                ClsSynchronizer.SyncSubDialogView = (dynamic)value;

            }
        }


        private ObservableCollection<SearchItem> _obsSearchItems;
        public ObservableCollection<SearchItem> ObsSearchItems
        {
            get { return _obsSearchItems; }
            set
            {
                SetProperty(ref _obsSearchItems, value, nameof(ObsSearchItems));
            }
        }

        /// <summary>
        /// 被選到的項目
        /// </summary>
        private SearchItem _selectedSearchItem;
        public SearchItem SelectedSearchItem
        {
            get { return _selectedSearchItem; }
            //set { SetProperty(ref _selectedListItem, value); }
            set { SetProperty(ref _selectedSearchItem, value, nameof(SelectedSearchItem)); }
        }




        private ICommand _closeDialogWindow { get; set; }
        public ICommand CloseDialogWindow
        {
            get
            {
                _closeDialogWindow = new RelayCommand((x) =>
                {
                    Window win = (Window)ClsSynchronizer.SyncSubDialogView;
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

                    Window win = (Window)ClsSynchronizer.SyncSubDialogView;
                    TextBox txtSelectedItemId = (TextBox)win.FindName("selectedItemId");
                    MessageBox.Show(txtSelectedItemId.Text);
                    if (String.IsNullOrWhiteSpace(txtSelectedItemId.Text)) {
                        MessageBox.Show(ClsSynchronizer.VmSyncCADs.GetLanguageByKeyName("msg_NoFilesSelected"));return; }

                    ClsSynchronizer.DialogReturnValue = txtSelectedItemId.Text;
                    win.Close();

                });
                return _done;
            }
        }



        private ICommand _itemPickerImageLeftClick { get; set; }
        public ICommand ItemPickerImageLeftClick
        {
            get
            {
                _itemPickerImageLeftClick = new RelayCommand((x) =>
                {
                    dynamic y = x;
                    TextBox txtProperty = (TextBox)y.TemplatedParent;
                    string itemType = txtProperty.Tag.ToString();
                    //string itemType = txtProperty.Tag.ToString();
                    //ShowAllRevisions("CAD",txtProperty);
                });
                return _itemPickerImageLeftClick;
            }
        }


        #endregion

        #region "                   方法區
        public void ShowAllRevisions(string itemType, string itemId)
        {

            ClsSynchronizer.DialogReturnValue = "";

            ItemType searchItemType = ClsSynchronizer.VmSyncCADs.GetItemType(itemType, SearchType.CADAllRevisionsSearch);

            ObsSearchItems = new ObservableCollection<SearchItem>();

            DataGrid gridSelectedItems = (DataGrid)ClsSynchronizer.SyncSubDialogView.FindName("gridSelectedItems");

            int j = 0;
            foreach (PLMProperty plmProperty in searchItemType.PlmProperties)
            {
                //if (plmProperty.Label != "")
                if (String.IsNullOrWhiteSpace(plmProperty.Label) == false)
                {
                    //要排除預設的屬性
                    
                    switch (plmProperty.DataType)
                    {
                        case "image":
                            AddDataGridImageColumn(gridSelectedItems, plmProperty, j);
                            break;
                        //case "item":
                        //    break;
                        default:
                            AddDataGridTextBlockStyleColumn(gridSelectedItems, plmProperty, j);
                            break;
                    }
                }
                j++;
            }


            SelectedSearchItem = new SearchItem();
            SelectedSearchItem.ClassName = itemType;

            ObsSearchItems = ClsSynchronizer.VmSyncCADs.GetAllRevisions(searchItemType, itemId);
            ObservableCollection<PLMProperty> newProperties = new ObservableCollection<PLMProperty>();
            foreach (PLMProperty property in searchItemType.PlmProperties)
            {
                PLMProperty newProperty = property.Clone() as PLMProperty;
                // if (newProperty.Name != "")
                if (String.IsNullOrWhiteSpace(newProperty.Name) == false)
                {
                    newProperty.IsInitial = false;
                    newProperty.IsExist = true;
                }
                newProperty.SoruceSearchItem = SelectedSearchItem;
                newProperties.Add(newProperty);
            }

            SelectedSearchItem.PlmProperties = newProperties;
            gridSelectedItems.ItemsSource = ObsSearchItems;

        }

        public ICommand GridFieldClickedCommand
        {
            get
            {

                _showCommand = _showCommand ?? new RelayCommand((x) =>
                {
                    Window win = (Window)ClsSynchronizer.SyncSubDialogView;

                    SearchItem searchItem = x as SearchItem;
                    if (searchItem != null)
                    {
                        ClsSynchronizer.ViewFilePath = ClsSynchronizer.VmSyncCADs.GetImageFullName(searchItem.Thumbnail);

                        if (String.IsNullOrWhiteSpace(ClsSynchronizer.ViewFilePath))
                        {
                            PLMProperty thumbnail = searchItem.PlmProperties.Where(y => y.Name == ClsSynchronizer.VmSyncCADs.ThumbnailProperty).FirstOrDefault();
                            if (thumbnail != null) ClsSynchronizer.ViewFilePath = ClsSynchronizer.VmSyncCADs.GetImageFullName(thumbnail.DataValue);
                        }

                        if (String.IsNullOrWhiteSpace(ClsSynchronizer.ViewFilePath)) ClsSynchronizer.ViewFilePath = @"pack://application:,,,/BCS.CADs.Synchronization;component/Images/White.bmp";

                        Canvas canvas = (Canvas)win.FindName("DialogCanvasViewFile");
                        TextBlock positionUse = (TextBlock)win.FindName("positionUse");
                        canvas.Visibility = Visibility.Visible;
                        ViewFile viewFile = (ViewFile)canvas.FindName("viewFile");

                        Label lb = (Label)ClsSynchronizer.MainWindows.FindName("scaleLabel");
                        Point pointToWindow = Mouse.GetPosition(positionUse);

                        double size = (double.Parse(lb.Content.ToString()) / 100);
                        Canvas.SetLeft(viewFile, pointToWindow.X - (20 * size));
                        Canvas.SetTop(viewFile, pointToWindow.Y - (520 * size));

                        if (viewFile != null)
                        {
                            Image image = (Image)viewFile.FindName("imageFile");
                            if (image != null) image.Source = new BitmapImage(new Uri(ClsSynchronizer.ViewFilePath));
                        }
                        Storyboard myStoryboard = new Storyboard();
                        myStoryboard.Children.Add((Storyboard)win.Resources["showMe"]);
                        myStoryboard.Begin(viewFile);
                    }
                });


                return _showCommand;
            }
        }




        #endregion

        #region "                   方法區(內部)


        private void AddDataGridTextBlockStyleColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {

            //System.Diagnostics.Debugger.Break();
            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = plmProperty.Label;
            FrameworkElementFactory txtBlockFactoryElem = new FrameworkElementFactory(typeof(TextBlock));
            AddDataGridTextBlockBinding(txtBlockFactoryElem, index);

            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = txtBlockFactoryElem;
            col.CellTemplate = cellTemplate;

            gridSelectedItems.Columns.Add(col);
        }

        private void AddDataGridImageColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {


            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = (plmProperty != null) ? plmProperty.Label : "";

            FrameworkElementFactory imagePickerFactoryElem = new FrameworkElementFactory(typeof(Image));
            AddDataGridImage(imagePickerFactoryElem, plmProperty, index);


            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = imagePickerFactoryElem;
            col.CellTemplate = cellTemplate;

            gridSelectedItems.Columns.Add(col);
        }


        private void AddDataGridTextBlockBinding(FrameworkElementFactory txtBlockFactoryElem, int index)
        {

            Binding textboxBind = new Binding("PlmProperties[" + index + "]");//DataValue  DisplayValue  (原本有問題是:PlmProperties[" + index + "].DisplayValue)
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtBlockFactoryElem.SetBinding(TextBlock.DataContextProperty, textboxBind);

            textboxBind = new Binding("DisplayValue");//DataValue  DisplayValue
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtBlockFactoryElem.SetBinding(TextBlock.TextProperty, textboxBind);

        }


        private void AddDataGridImage(FrameworkElementFactory imagePickerFactoryElem, PLMProperty plmProperty, int index)
        {

            Binding imageBind = (plmProperty != null) ? new Binding("PlmProperties[" + index + "].DataValue") : new Binding("Thumbnail");
            imageBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            imageBind.Mode = BindingMode.TwoWay;
            imageBind.Converter = new ThumbnailToPathConverter();

            imagePickerFactoryElem.SetValue(Image.SourceProperty, imageBind);

            Style imgStyle = new Style();
            imgStyle.TargetType = typeof(Image);
            for (int i = 0; i < 2; i++)
            {
                Setter imgSetter = new Setter();
                imgSetter.Property = (i == 0) ? Image.WidthProperty : Image.HeightProperty;
                imgSetter.Value = (double)24;
                imgStyle.Setters.Add(imgSetter);
            }
            imagePickerFactoryElem.SetValue(Image.StyleProperty, imgStyle);
        }


        #endregion
    }
}
