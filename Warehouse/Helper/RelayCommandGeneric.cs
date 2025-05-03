﻿using System;
using System.Windows.Input;

namespace Warehouse.Helper
{
  
    // Команда с параметром типа T.
  
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null) return true;
            return parameter is T t && _canExecute(t);
        }

        public void Execute(object? parameter)
        {
            if (parameter is T t) _execute(t);
        }

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
