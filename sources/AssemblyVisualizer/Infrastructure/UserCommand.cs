// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AssemblyVisualizer.Infrastructure
{
	class UserCommand : ViewModelBase
	{
		private string _text;

		public UserCommand(string text, ICommand command)
		{
			Text = text;
			Command = command;
		}

		public UserCommand(string text, Action action) : this(text, new DelegateCommand(action))
		{
		}

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}
		public ICommand Command { get; set; }
	}
}
