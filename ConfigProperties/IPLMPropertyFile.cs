#region "                   名稱空間"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace BCS.CADs.Synchronization.ConfigProperties
{
    /// <summary>
    ///  	CAD關聯檔案定義
    /// </summary>
    interface IPLMPropertyFile
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 序號
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// 功能名稱 : (例如 : native_property,viewable_property,thumbnail_property)
        /// </summary>
        string FunctionName { get; set; }

        /// <summary>
        /// 屬性名稱
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 資料類型
        /// </summary>
        string DataType { get; set; } 

        /// <summary>
        /// 檔案名稱 (實際同步到PLM時)
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// 檔案路徑 (實際同步到PLM時)
        /// </summary>
        string FilePath { get; set; }



        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion

    }
}
