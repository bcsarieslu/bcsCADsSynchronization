#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.Classes
{
    /// <summary>
    /// 外掛模組
    /// </summary>
    interface IClassPlugin: IDefaultElements
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 提示
        /// </summary>
        string Tips { get; set; }

        /// <summary>
        /// 圖片
        /// </summary>
        string Image { get; set; }

        /// <summary>
        /// 外部組件檔案:組鍵名稱
        /// </summary>
        string ComponentAssemblyName { get; set; } 

        /// <summary>
        /// 外部組件檔案:類別名稱
        /// </summary>
        string ComponentClassName { get; set; }

        /// <summary>
        /// 外部組件檔案"檔案名稱
        /// </summary>
        string ComponentFileName { get; set; } 

        /// <summary>
        /// 外部組件檔案:檔案
        /// </summary>
        string ComponentFileId { get; set; }

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
