using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPF_RevitAPI_04.Commands
{
    public class MyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action _excute;
        //private readonly Action<string> _excute1;
        private readonly Predicate<object> _canExecute;

        public MyCommand(Action exeute , Predicate<object> canExecute=null) 
        { 
            _excute=exeute;
            _canExecute=canExecute;
        }
        //public MyCommand(Action<string> excute1, Predicate<object> canExecute = null)
        //{
        //    _excute1 = excute1;
        //    _canExecute = canExecute;
        //}

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _excute();
        }

        //public void Execute1(object parameter)
        //{
        //    _excute1("");
        //}
    }
}
