using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.Entities
{
    public class ItemMessage
    {

        public String Time { get; set; }

        public String Name { get; set; }

        public String Value { get; set; }

        public String Detail { get; set; }

        private String _status ="";
        public String Status {
            get { return _status; }
            set { _status= value;
                 if (_status== "End" || _status == "Error" || _status == "Finish") this.StatusImage = ClsSynchronizer.SyncImages.Where(x => x.Key == _status).Select(x => x.Value).FirstOrDefault().ToString();
            }
        }

        public bool IsError { get; set; }

        public String FunctionImage { get; set; }

        public String OperationImage { get; set; }

        public String StatusImage { get; set; }

        public ItemException SyncItemException { get; set; }

        public ItemMessage(SyncMessages syncMessages, DateTime current, string name, string value, string detail,string status) 
		{
            //System.Diagnostics.Debugger.Break();
            this.Time = current.ToString("yyyy-MM-dd HH:mm:ss");
            this.Name = name;
            this.Value = value;
            this.Detail = detail;
            this.Status = status;

            if (status == "Error") this.IsError = true;
            this.FunctionImage = ClsSynchronizer.SyncImages.Where(x => x.Key == syncMessages.Function.ToString()).Select(x => x.Value).FirstOrDefault().ToString();
            this.OperationImage = ClsSynchronizer.SyncImages.Where(x => x.Key == syncMessages.Operation.ToString()).Select(x => x.Value).FirstOrDefault().ToString();
            string image = (status == "End" || status == "Error" || status == "Finish") ? status : "Executing";
            this.StatusImage = ClsSynchronizer.SyncImages.Where(x => x.Key == image).Select(x => x.Value).FirstOrDefault().ToString();
        }

    }
}
