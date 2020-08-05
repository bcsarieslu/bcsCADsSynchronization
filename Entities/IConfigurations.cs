using System;

#region "                   名稱空間"
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion


namespace BCS.CADs.Synchronization.Entities
{
    interface IConfigurations : IClasses
    {

        #region "                   宣告區"

        #endregion

        #region "                   屬性"

        bool ConditionalRules { get; set; }
        bool OperationOptions { get; set; }
        bool IntegrationEvents { get; set; }

        bool Composition { get; set; }

        #endregion

        #region "                   事件"

        #endregion

        #region "                   方法"

        #endregion

        #region "                   方法(內部)"

        #endregion



    }
}
