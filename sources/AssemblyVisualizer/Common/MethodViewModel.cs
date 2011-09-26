// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.Common
{
	class MethodViewModel : MemberViewModel, ICanBeVirtual
	{
		private readonly MethodInfo _methodInfo;

		public MethodViewModel(MethodInfo methodInfo) : base(methodInfo)
		{
			_methodInfo = methodInfo;           
		}	

        public bool IsVirtual
        {
            get
            {
                return _methodInfo.IsVirtual;
            }
        }

        public bool IsOverride
        {
            get
            {
                return _methodInfo.IsOverride;           
            }
        }           
	}
}
