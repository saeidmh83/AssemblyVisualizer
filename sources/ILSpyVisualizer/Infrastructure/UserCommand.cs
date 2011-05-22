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

		public string Text { get; private set; }
		public ICommand Command { get; private set; }
	}
}
