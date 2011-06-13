// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System;
using System.Windows.Input;

namespace ILSpyVisualizer.Infrastructure
{
	class DelegateCommand : ICommand
	{
		private readonly Predicate<object> _canExecute;
		private readonly Action<object> _execute;
		private readonly Action _executeParameterless;

		public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action<object> execute)
			: this(execute, null)
		{
		}

		public DelegateCommand(Action executeParameterless)
			: this(executeParameterless, null)
		{
		}

		public DelegateCommand(Action<object> execute,
					   Predicate<object> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public DelegateCommand(Action executeParameterless,
					   Predicate<object> canExecute)
		{
			_executeParameterless = executeParameterless;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute == null)
			{
				return true;
			}

			return _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			if (_execute != null)
			{
				_execute(parameter);
				return;
			}
			_executeParameterless();
		}

		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
			{
				CanExecuteChanged(this, EventArgs.Empty);
			}
		}
	}
}
