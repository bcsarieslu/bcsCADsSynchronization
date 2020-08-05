

using BCS.CADs.Synchronization.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BCS.CADs.Synchronization.Entities
{
    /// <summary>
    /// 產品配置
    /// </summary>
    interface IProduct
    {
        #region "                   宣告區"

        #endregion

        #region "                   屬性"
        List<ClassItem> PdClassItem { get; set; }
        List<ClassComposition> PdComposition { get; set; }
        List<IntegrationEvents> PdEvents { get; set; }
        List<ConditionalRule> PdRules { get; set; }
        List<OperationOption> PdOptions { get; set; }

        bool IsResolveAllLightweightSuppres { get; set; }
        bool IsResolveAllSuppres { get; set; }

        string Name { get; set; }

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"
        void BuildClasses();

        #endregion

        #region "                   方法(內部)"


        #endregion

    }
}
