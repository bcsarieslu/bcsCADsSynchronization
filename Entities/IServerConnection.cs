
#region "                   名稱空間"
using BCS.CADs.Synchronization.PLMList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion


// (1)URL(2)Database(3)User Name(4)Password(5)Windows Credentials(6)Login(8)Connection
namespace BCS.CADs.Synchronization.Entities
{
    interface IServerConnection
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        /// <summary>
        /// 登錄的網址
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        string Database { get; set; }

        /// <summary>
        /// 資料庫名稱下拉選項
        /// </summary>
        ObservableCollection<PLMListItem> ListItems { get; set; }

        /// <summary>
        /// 資料庫名稱下拉選項類型
        /// </summary>
        string DataType { get; set; }

        /// <summary>
        /// 登錄使用者名稱
        /// </summary>
        string LoginName { get; set; }

        /// <summary>
        /// 登錄使用者密碼
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// 是否為AD認證
        /// </summary>
        bool IsWinAuth { get; set; }

        /// <summary>
        /// 使用者是否登錄
        /// </summary>
        bool IsActiveLogin { get; set; }


        //void ConnItem<T>() where T : class;


        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        /// <summary>
        /// 登錄
        /// </summary>
        void Login();

        /// <summary>
        /// 登出
        /// </summary>
        void Logoff();

        #endregion

        #region "                   方法(內部)"

        #endregion





    }

}
