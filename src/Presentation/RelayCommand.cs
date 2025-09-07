using System;
using System.Windows.Input;

namespace Presentation
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _run;
        private readonly Predicate<object?>? _can;
        public RelayCommand(Action<object?> run, Predicate<object?>? canExecute = null)
        {
            _run = run; _can = canExecute;
        }
        public bool CanExecute(object? parameter) => _can?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _run(parameter);
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
