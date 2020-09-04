
#region "                   名稱空間"
using BCS.CADs.Synchronization.ConfigProperties;
using BCS.CADs.Synchronization.Models;
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
    interface IClassTemplateFile: IDefaultElements
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"


        /// <summary>
        /// 檔案名稱
        /// </summary>
        string FileName { get; set; } 

        /// <summary>
        /// 檔案
        /// </summary>
        string FileId { get; set; } 

        /// <summary>
        /// 是否被選取
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        int Quantity { get; set; }


        /// <summary>
        /// 類別名稱
        /// </summary>
        string ClassName { get; set; }

        /// <summary>
        /// CAD檔案類別名稱
        /// </summary>
        string FileClassName { get; set; }

        /// <summary>
        /// 特殊功能
        /// </summary>
        SpecialFeatures SpecialFeatures { get; set; }


        /// <summary>
        /// 縮圖圖檔完整名稱
        /// </summary>
        string ClassThumbnail { get; set; }

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion
    }
}
