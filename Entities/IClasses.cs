
#region "                   名稱空間"
using BCS.CADs.Synchronization.Classes;
using BCS.CADs.Synchronization.ConfigProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion


namespace BCS.CADs.Synchronization.Entities
{
    /// <summary>
    /// CAD類別定義
    /// </summary>
    interface IClasses 
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        List<PLMKey> CsKeys { get; set; }
        List<PLMProperty> CsProperties { get; set; }

        List<PLMPropertyFile> CsPropertyFile { get; set; }

        List<ClassRelationship> CsRelationship { get; set; }

        List<ClassTemplateFile> CsTemplateFile { get; set; }

        List<ClassPlugin> CsPlugin { get; set; }


        /// <summary>
        /// 鍵名稱
        /// </summary>
        string KeyedName { get; set; }

        /// <summary>
        /// 類別名稱
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// CAD檔案類別名稱
        /// </summary>
        string FileClassName { get; set; }

        ///// <summary>
        ///// CAD檔案類型
        ///// </summary>
        //string FileType { get; set; }

        /// <summary>
        /// 副檔名
        /// </summary>
        string Extension { get; set; }

        /// <summary>
        /// 相關的副檔名
        /// </summary>
        string Extensions { get; set; }

        /// <summary>
        /// 縮圖
        /// </summary>
        string Thumbnail { get; set; }

        /// <summary>
        /// 縮圖圖檔完整名稱
        /// </summary>
        string ThumbnailFullName { get; set; }

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"
        void BuildClass();
        #endregion

        #region "                   方法(內部)"

        #endregion

        //
    }
}
