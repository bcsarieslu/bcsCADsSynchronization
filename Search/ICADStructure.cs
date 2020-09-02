
#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Search
{
    interface ICADStructure
    {
        #region "                   宣告區"

        #endregion


        #region "                   屬性"
        /// <summary>
        /// 序號
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 副本Id
        /// </summary>
        string InstanceId { get; set; }


        /// <summary>
        /// 物件類型
        /// </summary>
        string ItemType { get; set; }

        /// <summary>
        /// config_id
        /// </summary>
        string ItemConfigId { get; set; }


        /// <summary>
        /// 是否為虛擬
        /// </summary>
        bool IsVirtual { get; set; }

        /// <summary>
        /// 是否為抑制
        /// </summary>
        bool IsSuppressed { get; set; }



        /// <summary>
        /// 是否為輕量抑制
        /// </summary>
        bool IsLightweighSuppressed { get; set; }
        

        /// <summary>
        /// 子階資訊 <子階config_id,List<sort_order>>
        /// </summary>
        SearchItem Child { get; set; }
        #endregion
    }
}
