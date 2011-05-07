using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ILSpyVisualizer.AssemblyBrowser
{
	abstract class MemberViewModel
	{
		public abstract ImageSource Icon { get; }
		public abstract string Text { get; }
	}
}
