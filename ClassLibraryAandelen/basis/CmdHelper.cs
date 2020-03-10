using System;
using System.Windows.Input;

namespace ClassLibraryAandelen.basis
{
    /// <summary>
    /// CmdHelper overerft van ICommand, zodat deze kan geïmplementeert worden op XAML elementen.
    /// 
    /// Parameters: 
    ///     execute: Actie dat geëxecuteert zal worden
    ///     canExecute: Func dat bepaalt of we execute kunnen laten uitvoeren, als deze leeggelaten zou 
    ///     worden dan wordt de execute uitgevoerdt zonder controle ervoor uit te voeren.
    /// </summary>
    public class CmdHelper : ICommand
    {
        private Action execute;
        private readonly Func<Boolean> canExecute;

        public CmdHelper(Action execute, Func<Boolean> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => execute.Invoke();
    }

    public class CmdHelper<T> : ICommand
    {
        private Action<T> execute;
        private readonly Func<Boolean> canExecute;

        public CmdHelper(Action<T> execute, Func<Boolean> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }
            return CanExecute((bool)parameter);
        }

        public void Execute(object parameter)
        {
            execute.Invoke((T)parameter);
        }
    }
}
