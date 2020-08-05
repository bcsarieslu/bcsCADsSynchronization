using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCS.CADs.Synchronization.ViewModels
{
    public class LanguageList: NotifyPropertyBase
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }
    }
}
