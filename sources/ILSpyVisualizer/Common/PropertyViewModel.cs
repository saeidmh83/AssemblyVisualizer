// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows.Media;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.Common
{
	class PropertyViewModel : MemberViewModel, ICanBeVirtual
	{
		private readonly PropertyInfo _propertyInfo;

		public PropertyViewModel(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		public override ImageSource Icon
		{
			get { return _propertyInfo.Icon; }
		}

		public override string Text
		{
			get
			{
				return _propertyInfo.Text;
			}
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

        public override object MemberReference
        {
            get { return _propertyInfo.MemberReference; }
        }

        public override bool IsPublic
        {
            get
            {
                return _propertyInfo.IsPublic;
            }
        }

        public override bool IsProtected
        {
            get
            {
                return _propertyInfo.IsProtected;
            }
        }

        public override bool IsInternal
        {
            get
            {
                return _propertyInfo.IsInternal;
            }
        }

        public override bool IsPrivate
        {
            get
            {
                return _propertyInfo.IsPrivate;
            }
        }

        public override bool IsProtectedInternal
        {
            get
            {
                return _propertyInfo.IsProtectedOrInternal;
            }
        }
	}
}
