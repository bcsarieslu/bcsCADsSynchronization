#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#endregion

namespace BCS.CADs.Synchronization.ViewModels
{
    public class VMCommon
    {
        #region "                   宣告區"

        #endregion

        #region "                   方法"


        #region "                          DataGrid"

       



        public void AddDataGridHeaderColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {

            AddDataGridHeaderTextStyleColumn(gridSelectedItems, plmProperty, index);

        }

        public void AddDataGridHeaderTextStyleColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {


            gridSelectedItems.ColumnHeaderHeight = 65;

            DataGridTemplateColumn col = new DataGridTemplateColumn();

            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            AddDataGridHeaderSearchControls(stackPanel, plmProperty);

            if (plmProperty.DataType == "image" || plmProperty.DataType == "revision")
            {
                stackPanel.AppendChild(CreateEmptyBorder());
            }

            else
            {
                FrameworkElementFactory txtBox = new FrameworkElementFactory(typeof(TextBox));
                AddDataGridTextStyleBinding(txtBox, plmProperty);
                txtBox.SetValue(TextBox.HeightProperty, 25d);  //TextBox高度                                                               //txtBox.SetValue(TextBox.HeightProperty, 25d);
                txtBox.SetValue(TextBox.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D3FDFF")));
                stackPanel.AppendChild(txtBox);
            }

            DataTemplate headerTemplate = new DataTemplate();
            headerTemplate.VisualTree = stackPanel;
            col.HeaderTemplate = headerTemplate;

            DataTemplate cellTemplate = new DataTemplate();
            if (plmProperty.DataType == "image")
            {
                FrameworkElementFactory imagePickerFactoryElem = new FrameworkElementFactory(typeof(Image));

                AddDataGridImage(imagePickerFactoryElem, plmProperty, index);
                cellTemplate.VisualTree = imagePickerFactoryElem;
            }
            else if (plmProperty.DataType == "revision")
            {
                FrameworkElementFactory txtboxFactoryElem = new FrameworkElementFactory(typeof(TextBox));
                
                AddDataGridTextStyleBinding(txtboxFactoryElem, index, plmProperty.DataType);
                cellTemplate.VisualTree = txtboxFactoryElem;
            }
            else
            {
                //Update
                //FrameworkElementFactory txtboxFactoryElem = new FrameworkElementFactory(typeof(TextBox));
                //AddDataGridTextStyleBinding(txtboxFactoryElem, index);
                //cellTemplate.VisualTree = txtboxFactoryElem;

                //Readonly
                FrameworkElementFactory txtBlockFactoryElem = new FrameworkElementFactory(typeof(TextBlock));
                AddDataGridTextBlockBinding(txtBlockFactoryElem, index);
                cellTemplate.VisualTree = txtBlockFactoryElem;
            }

            col.CellTemplate = cellTemplate;
            gridSelectedItems.Columns.Add(col);
            gridSelectedItems.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

        }

        private FrameworkElementFactory CreateEmptyBorder()
        {
            FrameworkElementFactory border1 = new FrameworkElementFactory(typeof(Border));
            FrameworkElementFactory border2 = new FrameworkElementFactory(typeof(Border));
            //AddDataGridTextBlockStyleBinding(txtBox, plmProperty);
            border1.SetValue(Border.HeightProperty, 25d);  //Border高度
            border1.SetValue(Border.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D3FDFF")));
            border1.SetValue(Border.BorderThicknessProperty, new Thickness(0, 1, 0, 0));
            border1.SetValue(Border.BorderBrushProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B4B4B4")));

            border2.SetValue(Border.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D3FDFF")));
            border2.SetValue(Border.BorderThicknessProperty, new Thickness(0.8, 0, 0.8, 0));
            border2.SetValue(Border.BorderBrushProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD")));
            border1.AppendChild(border2);

            return border1;
        }

        public void AddDataGridHeaderSearchControls(FrameworkElementFactory stackPanel, PLMProperty plmProperty)
        {
            VerticalAlignment verticalAlignment = new VerticalAlignment();
            stackPanel.SetValue(StackPanel.VerticalAlignmentProperty, verticalAlignment);

            //設定Header中的TextBlock樣式
            Thickness marginThickness = new Thickness();
            marginThickness.Right = 5;
            marginThickness.Left = 5;
            marginThickness.Bottom = 10;
            FrameworkElementFactory txtBlock = new FrameworkElementFactory(typeof(TextBlock));
            txtBlock.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);  //水平置中
            txtBlock.SetValue(TextBlock.TextProperty, plmProperty.Label);  //文字內容
            txtBlock.SetValue(TextBox.BackgroundProperty, Brushes.Transparent);  //背景透明
            txtBlock.SetValue(StackPanel.MarginProperty, marginThickness); //設定上下左右間隔
            stackPanel.AppendChild(txtBlock);
        }

        public void AddDataGridHeaderTextStyleBinding(FrameworkElementFactory txtboxFactoryElem, int index)
        {

            Binding textboxBind = new Binding("PlmProperties[" + index + "]");//DataValue  DisplayValue  (原本有問題是:PlmProperties[" + index + "].DisplayValue)
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.DataContextProperty, textboxBind);

            textboxBind.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
            textboxBind.Path = new PropertyPath("DisplayValue");
            txtboxFactoryElem.SetBinding(TextBox.TextProperty, textboxBind);


            textboxBind.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
            textboxBind.Path = new PropertyPath("DataSource");
            txtboxFactoryElem.SetBinding(TextBox.TagProperty, textboxBind);

            textboxBind = new Binding("DataType");
            textboxBind.Converter = new StyleGridConverter();
            txtboxFactoryElem.SetValue(TextBox.StyleProperty, textboxBind);

        }




        public void AddDataGridColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {

            switch (plmProperty.DataType)
            {

                case "image":
                    AddDataGridImageColumn(gridSelectedItems, plmProperty, index);
                    break;
                default:
                    AddDataGridTextColumn(gridSelectedItems, plmProperty, index);
                    break;
            }

        }


        public void AddDataGridTextBlockStyleColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
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


        public void AddDataGridTextBlockBinding(FrameworkElementFactory txtBlockFactoryElem, int index)
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


        public void AddDataGridTextColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {
            DataGridTextColumn textColumn = new DataGridTextColumn();
            textColumn.Header = plmProperty.Label;
            textColumn.Binding = new Binding("PlmProperties[" + index + "].DisplayValue");
            if (plmProperty.ColumnWidth > 0) textColumn.Width = plmProperty.ColumnWidth;
            gridSelectedItems.Columns.Add(textColumn);
        }


        public void AddDataGridTextStyleColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {

            //System.Diagnostics.Debugger.Break();
            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = plmProperty.Label;
            FrameworkElementFactory txtboxFactoryElem = new FrameworkElementFactory(typeof(TextBox));
            AddDataGridTextStyleBinding(txtboxFactoryElem, index, plmProperty.DataType);

            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = txtboxFactoryElem;
            col.CellTemplate = cellTemplate;

            gridSelectedItems.Columns.Add(col);

        }

        public void AddDataGridTextStyleBinding(FrameworkElementFactory txtboxFactoryElem, int index, string type)
        {

            Binding textboxBind = new Binding("PlmProperties[" + index + "]");//DataValue  DisplayValue  (原本有問題是:PlmProperties[" + index + "].DisplayValue)
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.DataContextProperty, textboxBind);

            textboxBind = new Binding("DisplayValue");//DataValue  DisplayValue
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TextProperty, textboxBind);

            textboxBind = (type == "revision") ? new Binding("SoruceSearchItem.ItemId") : new Binding("DataSource");
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TagProperty, textboxBind);


            if (type == "revision")
            {
                textboxBind = new Binding("DataValue");//DataValue  DisplayValue
                textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                textboxBind.Mode = BindingMode.OneWay;
                txtboxFactoryElem.SetBinding(TextBox.ToolTipProperty, textboxBind);
                //System.Diagnostics.Debugger.Break();
                //txtboxFactoryElem.SetValue(TextBox.IsReadOnlyProperty, true);
                //txtboxFactoryElem.SetValue(TextBox.BackgroundProperty, Brushes.Yellow);
            }

            textboxBind = new Binding("DataType");
            textboxBind.Converter = new StyleGridConverter();
            txtboxFactoryElem.SetValue(TextBox.StyleProperty, textboxBind);

        }

        //public void AddDataGridTextBolckStyleBinding(FrameworkElementFactory txtboxFactoryElem, int index, string type)
        //{

        //    Binding textBolckBind = new Binding("PlmProperties[" + index + "]");//DataValue  DisplayValue  (原本有問題是:PlmProperties[" + index + "].DisplayValue)
        //    textBolckBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    textBolckBind.Mode = BindingMode.TwoWay;
        //    txtboxFactoryElem.SetBinding(TextBlock.DataContextProperty, textBolckBind);

        //    textBolckBind = new Binding("DisplayValue");//DataValue  DisplayValue
        //    textBolckBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    textBolckBind.Mode = BindingMode.TwoWay;
        //    txtboxFactoryElem.SetBinding(TextBlock.TextProperty, textBolckBind);

        //    textBolckBind = (type == "revision") ? new Binding("SoruceSearchItem.ItemId") : new Binding("DataSource");
        //    textBolckBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    textBolckBind.Mode = BindingMode.TwoWay;
        //    txtboxFactoryElem.SetBinding(TextBlock.TagProperty, textBolckBind);


        //    if (type == "revision")
        //    {
        //        textBolckBind = new Binding("DataValue");//DataValue  DisplayValue
        //        textBolckBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //        textBolckBind.Mode = BindingMode.OneWay;
        //        txtboxFactoryElem.SetBinding(TextBlock.ToolTipProperty, textBolckBind);
        //    }

        //    textBolckBind = new Binding("DataType");
        //    textBolckBind.Converter = new StyleGridConverter();
        //    txtboxFactoryElem.SetValue(TextBlock.StyleProperty, textBolckBind);

        //}


        public void AddDataGridTextStyleBinding(FrameworkElementFactory txtboxFactoryElem, PLMProperty plmProperty)
        {

            txtboxFactoryElem.SetValue(TextBox.DataContextProperty, plmProperty);

            Binding textboxBind = new Binding("DisplayValue");//DataValue  DisplayValue
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TextProperty, textboxBind);

            textboxBind = new Binding("DataSource");
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TagProperty, textboxBind);

            if (plmProperty.DataType == "image" || plmProperty.DataType == "revision") return;

            textboxBind = new Binding("DataType");
            textboxBind.Converter = new StyleGridConverter();
            txtboxFactoryElem.SetValue(TextBox.StyleProperty, textboxBind);
        }

        public void AddDataGridTextBlockStyleBinding(FrameworkElementFactory txtBlockFactoryElem, PLMProperty plmProperty)
        {

            txtBlockFactoryElem.SetValue(TextBlock.DataContextProperty, plmProperty);

            Binding textBlockBind = new Binding("DisplayValue");//DataValue  DisplayValue
            textBlockBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textBlockBind.Mode = BindingMode.TwoWay;
            txtBlockFactoryElem.SetBinding(TextBlock.TextProperty, textBlockBind);

            textBlockBind = new Binding("DataSource");
            textBlockBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textBlockBind.Mode = BindingMode.TwoWay;
            txtBlockFactoryElem.SetBinding(TextBlock.TagProperty, textBlockBind);

            if (plmProperty.DataType == "image" || plmProperty.DataType == "revision") return;

            textBlockBind = new Binding("DataType");
            textBlockBind.Converter = new StyleGridConverter();
            txtBlockFactoryElem.SetValue(TextBlock.StyleProperty, textBlockBind);

            Style textBlockStyle = new Style();
            textBlockStyle.TargetType = typeof(TextBlock);
            textBlockStyle.Setters.Add(new Setter(Border.BorderBrushProperty, Brushes.Black));
            textBlockStyle.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(4)));
            textBlockStyle.Setters.Add(new Setter(Border.OpacityProperty, 1.0));

            txtBlockFactoryElem.SetValue(TextBlock.StyleProperty, textBlockStyle);

        }



        public void AddDataGridImageColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
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


        public void AddDataGridImage(FrameworkElementFactory imagePickerFactoryElem, PLMProperty plmProperty, int index)
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



        public void AddDataGridImageColumn(DataGrid gridSelectedItems)
        {
            AddDataGridImageColumn(gridSelectedItems, null, -1);
        }



        public void AddDataGridDateColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {
            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = plmProperty.Label;
            FrameworkElementFactory datePickerFactoryElem = new FrameworkElementFactory(typeof(DatePicker));
            Binding dateBind = new Binding("PlmProperties[" + index + "].DisplayValue");
            dateBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            dateBind.Mode = BindingMode.TwoWay;
            datePickerFactoryElem.SetValue(DatePicker.SelectedDateProperty, dateBind);
            datePickerFactoryElem.SetValue(DatePicker.DisplayDateProperty, dateBind);

            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = datePickerFactoryElem;
            col.CellTemplate = cellTemplate;

            gridSelectedItems.Columns.Add(col);
        }

        public void AddDataGridDatePicker(FrameworkElementFactory datePickerFactoryElem, PLMProperty plmProperty, int index)
        {

            Binding dateBind = new Binding("PlmProperties[" + index + "].DisplayValue");
            dateBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            dateBind.Mode = BindingMode.TwoWay;
            datePickerFactoryElem.SetValue(DatePicker.SelectedDateProperty, dateBind);
            datePickerFactoryElem.SetValue(DatePicker.DisplayDateProperty, dateBind);
        }


        public void AddDataGridComboBoxColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {


            DataGridComboBoxColumn cboColumn = new DataGridComboBoxColumn();
            cboColumn.Header = plmProperty.Label;

            cboColumn.ItemsSource = plmProperty.ListItems;
            if (plmProperty.ColumnWidth > 0) cboColumn.Width = plmProperty.ColumnWidth;
            cboColumn.DisplayMemberPath = "Label";

            Binding dateBind = new Binding("PlmProperties[" + index + "].DataValue");
            dateBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            dateBind.Mode = BindingMode.TwoWay;

            //還是需要,不然選到的物件值會被清空
            cboColumn.SelectedValueBinding = dateBind;
            cboColumn.SelectedValuePath = "Value";
            gridSelectedItems.Columns.Add(cboColumn);
        }




        #endregion







        #region "                          DataGrid"

        /*
        public void AddDataGridImageColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {


            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = (plmProperty != null) ? plmProperty.Label : "";

            //FrameworkElementFactory imagePickerFactoryElem = new FrameworkElementFactory(typeof(Image));
            ////Binding imageBind = new Binding("PlmProperties[" + index + "].DataValue");
            //Binding imageBind = (plmProperty != null) ? new Binding("PlmProperties[" + index + "].DataValue") : new Binding("Thumbnail");
            //imageBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //imageBind.Mode = BindingMode.TwoWay;
            //imageBind.Converter = new ThumbnailToPathConverter();

            //imagePickerFactoryElem.SetValue(Image.SourceProperty, imageBind);

            //Style imgStyle = new Style();
            //imgStyle.TargetType = typeof(Image);
            //for (int i = 0; i < 2; i++)
            //{
            //    Setter imgSetter = new Setter();
            //    imgSetter.Property = (i==0)? Image.WidthProperty: Image.HeightProperty;
            //    imgSetter.Value = (double)24;
            //    imgStyle.Setters.Add(imgSetter);
            //}
            //imagePickerFactoryElem.SetValue(Image.StyleProperty, imgStyle);

            FrameworkElementFactory imagePickerFactoryElem = new FrameworkElementFactory(typeof(Image));
            AddDataGridImage(imagePickerFactoryElem, plmProperty, index);


            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = imagePickerFactoryElem;
            col.CellTemplate = cellTemplate;

            gridSelectedItems.Columns.Add(col);
        }


        public void AddDataGridImage(FrameworkElementFactory imagePickerFactoryElem, PLMProperty plmProperty, int index)
        {

            //FrameworkElementFactory imagePickerFactoryElem = new FrameworkElementFactory(typeof(Image));
            //Binding imageBind = new Binding("PlmProperties[" + index + "].DataValue");
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

        public void AddDataGridHeaderColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {

            //Modify by kenny 2020/07/31 --------------
            AddDataGridHeaderTextStyleColumn(gridSelectedItems, plmProperty, index);
            return;
            //AddDataGridTextStyleColumn(gridSelectedItems, plmProperty, index);
            //-----------------------------------------
            //return;

            //switch (plmProperty.DataType)
            //{
            //    case "date":
            //        //AddDataGridDateColumn(gridSelectedItems, plmProperty, index);
            //        AddDataGridTextStyleColumn(gridSelectedItems, plmProperty, index);
            //        break;
            //    case "list":
            //        //AddDataGridComboBoxColumn(gridSelectedItems, plmProperty, index);
            //        AddDataGridTextStyleColumn(gridSelectedItems, plmProperty, index);
            //        break;
            //    case "filter list":
            //        //AddDataGridComboBoxColumn(gridSelectedItems, plmProperty, index);
            //        AddDataGridTextStyleColumn(gridSelectedItems, plmProperty, index);
            //        break;
            //    case "image":
            //        AddDataGridImageColumn(gridSelectedItems, plmProperty, index);
            //        break;
            //    case "item":
            //        AddDataGridTextStyleColumn(gridSelectedItems, plmProperty, index);
            //        //AddDataGridTextStyleItemColumn(gridSelectedItems, plmProperty, index);
            //        break;
            //    default:
            //        AddDataGridTextColumn(gridSelectedItems, plmProperty, index );
            //        break;
            //}

        }


        //private void AddDataGridHeaderTextStyleColumn(DataGrid gridSelectedItems, PLMProperties plmProperty, int index)
        //{

        //    gridSelectedItems.ColumnHeaderHeight = 70;
        //    DataGridTextColumn textColumn = new DataGridTextColumn();

        //    FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));

        //    VerticalAlignment verticalAlignment = new VerticalAlignment();
        //    stackPanel.SetValue(StackPanel.VerticalAlignmentProperty, verticalAlignment);
        //    FrameworkElementFactory txt1 = new FrameworkElementFactory(typeof(TextBlock));
        //    txt1.SetValue(TextBlock.TextProperty, plmProperty.Label);
        //    stackPanel.AppendChild(txt1);


        //    FrameworkElementFactory txt2 = new FrameworkElementFactory(typeof(TextBox));
        //    AddDataGridTextStyleBinding(txt2, index);

        //    stackPanel.AppendChild(txt2);

        //    DataTemplate cellTemplate = new DataTemplate();
        //    cellTemplate.VisualTree = stackPanel;

        //    textColumn.SetValue(DataGridTextColumn.HeaderTemplateProperty, cellTemplate);

        //    Binding textboxBind = new Binding("PlmProperties[" + index + "].DisplayValue");
        //    textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    textboxBind.Mode = BindingMode.TwoWay;
        //    textColumn.Binding=textboxBind;

        //    gridSelectedItems.Columns.Add(textColumn);

        //}

        public void AddDataGridDateColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {
            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = plmProperty.Label;
            FrameworkElementFactory datePickerFactoryElem = new FrameworkElementFactory(typeof(DatePicker));
            Binding dateBind = new Binding("PlmProperties[" + index + "].DisplayValue");
            dateBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            dateBind.Mode = BindingMode.TwoWay;
            datePickerFactoryElem.SetValue(DatePicker.SelectedDateProperty, dateBind);
            datePickerFactoryElem.SetValue(DatePicker.DisplayDateProperty, dateBind);

            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = datePickerFactoryElem;
            col.CellTemplate = cellTemplate;

            //dateBind.BindsDirectlyToSource = "DataValue";
            //e.Column = col;//Set the new generated column
            gridSelectedItems.Columns.Add(col);
        }

        public void AddDataGridHeaderTextStyleColumn(DataGrid gridSelectedItems, PLMProperty plmProperty, int index)
        {

            //Modify by kenny 2020/08/05
            //gridSelectedItems.ColumnHeaderHeight = 70;
            gridSelectedItems.ColumnHeaderHeight = 65;

            DataGridTemplateColumn col = new DataGridTemplateColumn();

            FrameworkElementFactory stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            AddDataGridHeaderSearchControls(stackPanel, plmProperty);

            FrameworkElementFactory txtBox = new FrameworkElementFactory(typeof(TextBox));
            AddDataGridTextStyleBinding(txtBox, plmProperty);
            txtBox.SetValue(TextBox.HeightProperty, 25d);  //TextBox高度
            txtBox.SetValue(TextBox.HeightProperty, 25d);
            //txtBox.SetValue(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            stackPanel.AppendChild(txtBox);

            DataTemplate headerTemplate = new DataTemplate();
            headerTemplate.VisualTree = stackPanel;
            col.HeaderTemplate = headerTemplate;

            DataTemplate cellTemplate = new DataTemplate();
            if (plmProperty.DataType == "image")
            {
                FrameworkElementFactory imagePickerFactoryElem = new FrameworkElementFactory(typeof(Image));

                AddDataGridImage(imagePickerFactoryElem, plmProperty, index);
                cellTemplate.VisualTree = imagePickerFactoryElem;
            }
            else if (plmProperty.DataType == "revision")
            {
                FrameworkElementFactory txtboxFactoryElem = new FrameworkElementFactory(typeof(TextBox));
                //AddDataGridHeaderTextStyleBinding(txtboxFactoryElem, index);
                AddDataGridTextStyleBinding(txtboxFactoryElem, index, plmProperty.DataType);
                cellTemplate.VisualTree = txtboxFactoryElem;
            }
            else
            {
                //Update
                //FrameworkElementFactory txtboxFactoryElem = new FrameworkElementFactory(typeof(TextBox));
                //AddDataGridTextStyleBinding(txtboxFactoryElem, index);
                //cellTemplate.VisualTree = txtboxFactoryElem;

                //Readonly
                FrameworkElementFactory txtBlockFactoryElem = new FrameworkElementFactory(typeof(TextBlock));
                AddDataGridTextBlockBinding(txtBlockFactoryElem, index);
                cellTemplate.VisualTree = txtBlockFactoryElem;
            }


            //Style headerStyle = new Style(typeof(DataGridColumnHeader));
            //headerStyle.Setters.Add(new Setter(FrameworkElement.HeightProperty, 55d));
            //headerStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            //headerStyle.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Bottom));
            //col.HeaderStyle = headerStyle;

            col.CellTemplate = cellTemplate;
            //col.SetValue(DataGridTemplateColumn.HeaderTemplateProperty, cellTemplate);
            gridSelectedItems.Columns.Add(col);
            gridSelectedItems.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

        }

        public void AddDataGridTextStyleBinding(FrameworkElementFactory txtboxFactoryElem, int index, string type)
        {

            Binding textboxBind = new Binding("PlmProperties[" + index + "]");//DataValue  DisplayValue  (原本有問題是:PlmProperties[" + index + "].DisplayValue)
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.DataContextProperty, textboxBind);

            textboxBind = new Binding("DisplayValue");//DataValue  DisplayValue
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TextProperty, textboxBind);

            textboxBind = (type == "revision") ? new Binding("SoruceSearchItem.ItemId") : new Binding("DataSource");
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TagProperty, textboxBind);


            if (type == "revision")
            {
                textboxBind = new Binding("DataValue");//DataValue  DisplayValue
                textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                textboxBind.Mode = BindingMode.OneWay;
                txtboxFactoryElem.SetBinding(TextBox.ToolTipProperty, textboxBind);
            }

            //textboxBind = (type == "revision") ? new Binding("SoruceSearchItem.ItemId") : new Binding("DataSource");

            //if (type == "revision")
            //{
            //    MultiBinding mlBind = new MultiBinding();
            //    mlBind.StringFormat = "{}{0}";
            //    textboxBind = new Binding("SoruceSearchItem.ClassName");
            //    mlBind.Bindings.Add(textboxBind);
            //    textboxBind = new Binding("SoruceSearchItem.ItemId");
            //    mlBind.Bindings.Add(textboxBind);

            //    mlBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //    mlBind.Mode = BindingMode.TwoWay;
            //    txtboxFactoryElem.SetBinding(TextBox.TagProperty, mlBind);
            //}
            //else
            //{
            //    textboxBind = new Binding("DataSource");
            //    textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //    textboxBind.Mode = BindingMode.TwoWay;
            //    txtboxFactoryElem.SetBinding(TextBox.TagProperty, textboxBind);
            //}


            //textboxBind = new Binding("DataSource");
            //textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //textboxBind.Mode = BindingMode.TwoWay;
            //txtboxFactoryElem.SetBinding(TextBox.TagProperty, textboxBind);

            //plmProperty.SoruceSearchItem.ClassName

            textboxBind = new Binding("DataType");
            textboxBind.Converter = new StyleGridConverter();
            txtboxFactoryElem.SetValue(TextBox.StyleProperty, textboxBind);

        }

         public void AddDataGridTextStyleBinding(FrameworkElementFactory txtboxFactoryElem, PLMProperty plmProperty)
        {



            txtboxFactoryElem.SetValue(TextBox.DataContextProperty, plmProperty);

            //textboxBind = new Binding("PlmProperties[" + index + "]");//DataValue  DisplayValue  (原本有問題是:PlmProperties[" + index + "].DisplayValue)
            //textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //textboxBind.Mode = BindingMode.TwoWay;
            //txtboxFactoryElem.SetBinding(TextBox.DataContextProperty, textboxBind);

            Binding textboxBind = new Binding("DisplayValue");//DataValue  DisplayValue
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TextProperty, textboxBind);

            textboxBind = new Binding("DataSource");
            textboxBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textboxBind.Mode = BindingMode.TwoWay;
            txtboxFactoryElem.SetBinding(TextBox.TagProperty, textboxBind);

            if (plmProperty.DataType == "image" || plmProperty.DataType == "revision") return;

            textboxBind = new Binding("DataType");
            textboxBind.Converter = new StyleGridConverter();
            txtboxFactoryElem.SetValue(TextBox.StyleProperty, textboxBind);
        }


        */

        #endregion








        #endregion

    }
}
