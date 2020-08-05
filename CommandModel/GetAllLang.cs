using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCS.CADs.Synchronization.CommandModel
{
    internal class GetAllLang : NotifyPropertyBase
    {
        private string _langname;
        public string LangName
        {
            get { return _langname; }
            set { SetProperty(ref _langname, value); }
        }
        private string _langdisplayname;
        public string LangDisplayName 
        {
            get { return _langdisplayname; }
            set { SetProperty(ref _langdisplayname, value); }
        }
    }
}
