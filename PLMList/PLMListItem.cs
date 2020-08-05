#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace BCS.CADs.Synchronization.PLMList
{
    public class PLMListItem: IPLMListItem, ICloneable
    {
        #region "                   屬性"
        /// <summary>
        /// 序號
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 標籤
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 過濾
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// 是否為過濾
        /// </summary>
        public bool IsFilter { get; set; }

        /// <summary>
        /// 標籤其他語系
        /// </summary>
        public Dictionary<string, string> Labels { get; set; } = new Dictionary<string, string>(); //Language.suffix
        #endregion


        #region "                   事件"

        #endregion

        #region "                   方法"
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

        #region "                   方法(內部)"

        #endregion
    }

}
