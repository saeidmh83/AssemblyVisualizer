// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using ICSharpCode.ILSpy;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;

namespace ILSpyVisualizer.AncestryBrowser
{	
	class AncestryBrowserWindowViewModel : ViewModelBase
	{
		private TypeDefinition _typeDefinition;
        private TypeViewModel _typeViewModel;
		
		public AncestryBrowserWindowViewModel(TypeDefinition typeDefinition)
		{
			_typeDefinition = typeDefinition;
            _typeViewModel = new TypeViewModel(_typeDefinition);            
		}
		
		public string Name
		{
			get
			{
				return MainWindow.Instance.CurrentLanguage.FormatTypeName(_typeDefinition);
			}
		}

        public TypeViewModel Type
        {
            get 
            {
                return _typeViewModel;
            }
        }
	}
}
