
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Models;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Classes
{
    /// <summary>
    /// 類別範本檔案
    /// </summary>
    public class ClassTemplateFile : NotifyPropertyBase, IClassTemplateFile
    {

        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 序號
        /// </summary>
        [XmlSettingTagNameAttribute("sort_order")]
        public int Order { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [XmlSettingTagNameAttribute("name")]
        public string Name { get; set; }


        /// <summary>
        /// CAD檔案類別名稱
        /// </summary>
        public string FileClassName { get; set; }

        /// <summary>
        /// 檔案名稱
        /// </summary>
        [XmlSettingAttributeName("File.filename")]
        public string FileName { get; set; } = "";

        /// <summary>
        /// 檔案
        /// </summary>
        [XmlSettingAttributeName("File.fileid")]
        public string FileId { get; set; } = "";


        /// <summary>
        /// 是否被選取
        /// </summary>
        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                //if (value != _isSelected)
                //{
                //    SetProperty(ref _isSelected, value);
                //}
                SetProperty(ref _isSelected, value, nameof(IsSelected));
            }
        }

        /// <summary>
        /// 數量
        /// </summary>
        private int _quantity = 1;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                //if (value != _quantity)
                //{
                //    SetProperty(ref _quantity, value);
                //}
                SetProperty(ref _quantity, value, nameof(Quantity));
            }
        }

        /// <summary>
        /// 類別名稱
        /// </summary>

        //public string ClassName { get; set; } = "";
        private string _className;
        public string ClassName
        {
            get { return _className; }
            set
            {
                _className = value;
                OnPropertyChanged("ClassName");
            }
        }

        /// <summary>
        /// 特殊功能
        /// </summary>
        private SpecialFeatures _specialFeatures;
        public SpecialFeatures SpecialFeatures
        {
            get { return _specialFeatures; }
            set
            {
                _specialFeatures = value;
                OnPropertyChanged("SpecialFeatures");
            }
        }

        /// <summary>
        /// 縮圖圖檔完整名稱
        /// </summary>
        public string ClassThumbnail { get; set; } = "";


        /// <summary>
        /// 保留,尚未使用
        /// </summary>
        public string  Value { get; set; }
        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion
    }
}
