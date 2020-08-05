#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace BCS.CADs.Synchronization.ConfigProperties
{
    public class PLMRevision : IPLMRevision, ICloneable//Revision
    {

        #region "                   屬性"
        /// <summary>
        /// Id
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// Config Id
        /// </summary>
        public string ItemConfigId { get; set; }

        /// <summary>
        /// Revision (MajorRevision.Generation)
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// MajorRevision
        /// </summary>
        public string MajorRevision { get; set; }

        /// <summary>
        /// Generation
        /// </summary>
        public string Generation { get; set; }


        public bool IsSelected { get; set; } = false;

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
