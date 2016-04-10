/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace small_n_stats_WPF.Utilities
{
    class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> predicate)
        {
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = predicate;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;

            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
