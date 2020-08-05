using BCS.CADs.Synchronization.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BCS.CADs.Synchronization.ViewModels
{
    public class PositioningCommand : ICommand
    {
        public PositioningCommand()
        {
        }

        public void Execute(object parameter)
        {
            Point mousePos = Mouse.GetPosition((IInputElement)parameter);
            Console.WriteLine("Position: " + mousePos.ToString());
        }

        public bool CanExecute(object parameter) { return true; }

        public event EventHandler CanExecuteChanged;

        public PositioningCommand TargetClick
        {
            get;
            internal set;
        }
    }


}


