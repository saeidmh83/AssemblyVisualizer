// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Linq;
using ICSharpCode.ILSpy;
using ILSpyVisualizer.Infrastructure;
using Mono.Cecil;
using System.Collections.Generic;
using ILSpyVisualizer.Common;

namespace ILSpyVisualizer.AncestryBrowser
{	
	class AncestryBrowserWindowViewModel : ViewModelBase
	{
		private TypeDefinition _typeDefinition;
        private TypeViewModel _typeViewModel;
        private IEnumerable<AssemblyViewModel> _assemblies;
        private IEnumerable<TypeViewModel> _ancestry;   
        private MemberOptions _options;        
		
		public AncestryBrowserWindowViewModel(TypeDefinition typeDefinition)
		{
			_typeDefinition = typeDefinition;

            _options = new MemberOptions
            {
                ShowProperties = true,
                ShowEvents = true,
                ShowMethods = true,
                ShowProtected = true,
                ShowProtectedInternal = true,
                ShowPublic = true
            };

            _typeViewModel = new TypeViewModel(_typeDefinition);

            _ancestry = _typeViewModel.Ancestry.ToList();
            _assemblies = _ancestry
                .GroupBy(t => t.TypeDefinition.Module.Assembly)
                .Select(g => new AssemblyViewModel(g.Key, g))
                .ToList();

            int currentIndex = 0;
            foreach (var assembly in _assemblies)
            {
                assembly.BackgroundBrush = BrushProvider.BrushPairs[currentIndex].Background;
                assembly.CaptionBrush = BrushProvider.BrushPairs[currentIndex].Caption;
                currentIndex++;
                if (currentIndex == BrushProvider.BrushPairs.Count)
                {
                    currentIndex = 0;
                }
            }

            UpdateMembers();
		}

        #region // Public properties

        public string Name
		{
			get
			{
				return MainWindow.Instance.CurrentLanguage.FormatTypeName(_typeDefinition);
			}
		}

        public TypeViewModel Type { get { return _typeViewModel; } }
        public IEnumerable<AssemblyViewModel> Assemblies { get { return _assemblies; } }

        public bool ShowFields
        {
            get 
            {
                return _options.ShowFields;
            }
            set 
            {
                _options.ShowFields = value;
                UpdateMembers();
            }
        }

        public bool ShowProperties
        {
            get
            {
                return _options.ShowProperties;
            }
            set
            {
                _options.ShowProperties = value;
                UpdateMembers();
            }
        }

        public bool ShowEvents
        {
            get
            {
                return _options.ShowEvents;
            }
            set
            {
                _options.ShowEvents = value;
                UpdateMembers();
            }
        }

        public bool ShowMethods
        {
            get
            {
                return _options.ShowMethods;
            }
            set
            {
                _options.ShowMethods = value;
                UpdateMembers();
            }
        }

        public bool ShowPublic
        {
            get
            {
                return _options.ShowPublic;
            }
            set
            {
                _options.ShowPublic = value;
                UpdateMembers();
            }
        }

        public bool ShowInternal
        {
            get
            {
                return _options.ShowInternal;
            }
            set
            {
                _options.ShowInternal = value;
                UpdateMembers();
            }
        }

        public bool ShowProtected
        {
            get
            {
                return _options.ShowProtected;
            }
            set
            {
                _options.ShowProtected = value;
                UpdateMembers();
            }
        }

        public bool ShowPrivate
        {
            get
            {
                return _options.ShowPrivate;
            }
            set
            {
                _options.ShowPrivate = value;
                UpdateMembers();
            }
        }

        public bool ShowProtectedInternal
        {
            get
            {
                return _options.ShowProtectedInternal;
            }
            set
            {
                _options.ShowProtectedInternal = value;
                UpdateMembers();
            }
        }

        public string SearchTerm
        {
            get
            {
                return _options.SearchTerm;
            }
            set
            {
                _options.SearchTerm = value;
                OnPropertyChanged("IsSearchTermEmpty");
                UpdateMembers();
            }
        }

        public bool IsSearchTermEmpty
        {
            get
            {
                return string.IsNullOrEmpty(SearchTerm);
            }            
        }

        #endregion

        private void UpdateMembers()
        {
            foreach (var type in _ancestry)
            {
                type.UpdateMembers(_options);
            }
        }
	}
}
