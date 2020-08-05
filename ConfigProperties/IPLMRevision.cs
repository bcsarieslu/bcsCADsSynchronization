#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace BCS.CADs.Synchronization.ConfigProperties
{
    interface IPLMRevision
    {

        #region "                   屬性"
        /// <summary>
        /// Id
        /// </summary>
        string ItemId { get; set; }
        /// <summary>
        /// Config Id
        /// </summary>
        string ItemConfigId { get; set; }

        /// <summary>
        /// Revision (MajorRevision.Generation)
        /// </summary>
        string Revision { get; set; }

        /// <summary>
        /// MajorRevision
        /// </summary>
        string MajorRevision { get; set; }

        /// <summary>
        /// Generation
        /// </summary>
        string Generation { get; set; }

        bool IsSelected { get; set; }
        #endregion

    }
}
