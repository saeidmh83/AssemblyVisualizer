// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ILSpyVisualizer.Infrastructure;

namespace ILSpyVisualizer.Common
{
	abstract class MemberViewModel : ViewModelBase
	{
		public abstract ImageSource Icon { get; }
		public abstract string Text { get; }
        public virtual bool IsVirtual
        {
            get
            {
                return false;
            }
        }
        public virtual bool IsOverridden
        {
            get
            {
                return false;
            }
        }
	}
}
