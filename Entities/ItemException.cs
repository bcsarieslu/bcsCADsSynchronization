using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.Entities
{
    public class ItemException : Exception
    {
        public int FaultCode = -1;

        public string FaultString = "";

        public string FaultDetail ="";


        public ItemException(int fcode, string fstring, string fdetail, Exception ex): base(fdetail, ex)
		{
            this.FaultCode = fcode;
            this.FaultString = fstring;
            this.FaultDetail = fdetail;
            
        }
    }

}
