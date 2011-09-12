// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Infrastructure;

namespace AssemblyVisualizer.Common.CommandsGroup
{
	class CommandsGroupViewModel : ViewModelBase
	{
		public CommandsGroupViewModel(string header, IEnumerable<GroupedUserCommand> commands)
		{
			Header = header;
			Commands = commands;

			foreach (var command in commands)
			{
				command.GroupViewModel = this;
			}
		}

		public string Header { get; private set; }
		public IEnumerable<GroupedUserCommand> Commands { get; private set; }

		public void ClearCurrentCommand()
		{
			foreach (var command in Commands)
			{
				command.IsCurrent = false;
			}
		}
	}
}
