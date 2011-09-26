// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.Common
{
	class PropertyViewModel : MemberViewModel, ICanBeVirtual
	{
		private readonly PropertyInfo _propertyInfo;

		public PropertyViewModel(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

        public bool IsVirtual
        {
            get
            {
                return _propertyInfo.IsVirtual;
            }
        }

        public bool IsOverride
        {
            get
            {
                return _propertyInfo.IsOverride;
            }
        }

               
	}
}
