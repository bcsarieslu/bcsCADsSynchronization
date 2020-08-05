#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.PLMList
{
    interface IPLMListItem
    {
        #region "                   屬性"
        /// <summary>
        /// 序號
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 標籤
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// 過濾
        /// </summary>
        string Filter { get; set; }

        /// <summary>
        /// 是否為過濾
        /// </summary>
        bool IsFilter { get; set; }

        /// <summary>
        /// 標籤其他語系
        /// </summary>
        Dictionary<string, string> Labels { get; set; }
        #endregion
    }
}
