using System;
using System.Windows.Input;

namespace SnakeWpf
{
    public class Command : ICommand
    {
        private readonly Action<object> execute = null;
        private readonly Func<object, bool> canExecute = null;

        public Command(Action execute) : this((e) => execute(), null)
        {
        }

        public Command(Action execute, Func<bool> canExecute) : this((e) => execute(), (e) => canExecute())
        {
        }

        public Command(Action<object> execute) : this(execute, null)
        {
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
