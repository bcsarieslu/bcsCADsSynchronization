using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCS.CADs.Synchronization
{
    public class DependencyObjectProxy : DependencyObject
    {
        public static readonly DependencyProperty ObjectProperty
            = DependencyProperty.Register(nameof(Value), typeof(object), typeof(DependencyObjectProxy));
        public object Value
        {
            get { return GetValue(ObjectProperty); }
            set { SetValue(ObjectProperty, value); }
        }
    }
}
