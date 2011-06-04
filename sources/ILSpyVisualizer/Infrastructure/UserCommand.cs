using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ILSpyVisualizer.Infrastructure
{
	class UserCommand
	{
		public UserCommand(string text, ICommand command)
		{
			Text = text;
			Command = command;
		}

		public UserCommand(string text, Action action) : this(text, new DelegateCommand(action))
		{
		}

		public string Text { get; private set; }
		public ICommand Command { get; set; }
	}
}
