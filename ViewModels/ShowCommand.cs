using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BCS.CADs.Synchronization.ViewModels
{
    class ShowCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                //throw new NotImplementedException();
            }

            remove
            {
                //throw new NotImplementedException();
            }
            //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            //throw new NotImplementedException();
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            //throw new NotImplementedException();
            //MessageBox.Show(parameter.ToString());
        }
    }
}
