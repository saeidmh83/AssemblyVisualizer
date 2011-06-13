// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System.Windows.Media;
using ILSpyVisualizer.Infrastructure;

namespace ILSpyVisualizer.AssemblyBrowser.ViewModels
{
	abstract class MemberViewModel : ViewModelBase
	{
		public abstract ImageSource Icon { get; }
		public abstract string Text { get; }
	}
}
