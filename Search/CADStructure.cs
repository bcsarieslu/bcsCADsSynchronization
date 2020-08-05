
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Search
{
    public class CADStructure : NotifyPropertyBase, ICADStructure
    {

        #region "                   宣告區"

        #endregion


        #region "                   屬性"
        #endregion
        /// <summary>
        /// 序號
        /// </summary>
        [XmlSettingTagNameAttribute("sort_order")]
        public int Order { get; set; } = 0;

        /// <summary>
        /// 副本Id
        /// </summary>
        public string InstanceId { get; set; } = "";


        /// <summary>
        /// 物件類型
        /// </summary>
        public string ItemType { get; set; } = "CAD Structure";

        /// <summary>
        /// config_id
        /// </summary>
        [XmlSettingTagNameAttribute("config_id")]
        public string ItemConfigId { get; set; } = "";

        /// <summary>
        /// 是否為虛擬
        /// </summary>
        public bool IsVirtual { get; set; } = false;

        /// <summary>
        /// 是否為抑制
        /// </summary>
        public bool IsSuppressed { get; set; } = false;

        /// <summary>
        /// 是否為輕量抑制
        /// </summary>
        public bool IsLightweighSuppressed { get; set; } = false;

        /// <summary>
        /// 111:(4,2,1) :CopyToAdd,Replace,Insert
        /// </summary>
        public int OperationType { get; set; } = 0;

        /// <summary>
        /// 子階資訊 <子階config_id,List<sort_order>>
        /// </summary>
        private SearchItem _child;
        public SearchItem Child
        {
            get { return _child; }
            set
            {
                SetProperty(ref _child, value);

            }
        }
    }
}
