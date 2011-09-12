// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ILSpyVisualizer.Infrastructure;

namespace ILSpyVisualizer.Common.CommandsGroup
{
	/// <summary>
	/// Interaction logic for CommandsGroupView.xaml
	/// </summary>
	partial class CommandsGroupView : UserControl
	{
		public CommandsGroupView()
		{
			InitializeComponent();
		}

		public IEnumerable<UserCommand> Commands
		{
			get { return (IEnumerable<UserCommand>)GetValue(CommandsProperty); }
			set { SetValue(CommandsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Commands.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandsProperty =
			DependencyProperty.Register("Commands", typeof(IEnumerable<UserCommand>), typeof(CommandsGroupView), new UIPropertyMetadata(null));
		
		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(string), typeof(CommandsGroupView), new UIPropertyMetadata(""));


	}
}
