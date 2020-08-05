using BCS.CADs.Synchronization.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BCS.CADs.Synchronization.ViewModels
{
    class RelayCommand: ICommand
    {
        private readonly Action<object> _executeHandler;
        private readonly Func<object, bool> _canExecuteHandler;

        public event EventHandler CanExecuteChanged;
        public RelayCommand(Action<object> executeHandler, Func<object, bool> canExecuteHandler)
        {
            _executeHandler = executeHandler; //?? throw new ArgumentNullException("execute handler can not be null");
            _canExecuteHandler = canExecuteHandler; //?? throw new ArgumentNullException("canExecute handler can not be null");
        }

        public RelayCommand(Action<object> execute) : this(execute, (x) => true)
        { }

        public bool CanExecute(object parameter)
        {
            return _canExecuteHandler(parameter);
        }

        public void Execute(object parameter)
        {
            _executeHandler(parameter);
        }

        public void RaiseCommand()
        {
            CommandManager.InvalidateRequerySuggested();
        }


    }
}
