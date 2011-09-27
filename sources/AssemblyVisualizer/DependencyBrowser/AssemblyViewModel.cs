// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Infrastructure;
using AssemblyVisualizer.Model;
using System.Windows.Input;

namespace AssemblyVisualizer.DependencyBrowser
{
    class AssemblyViewModel : ViewModelBase
    {
        private static Dictionary<AssemblyInfo, AssemblyViewModel> _correspondence = new Dictionary<AssemblyInfo, AssemblyViewModel>();

        private AssemblyInfo _assembly;
        private IList<AssemblyViewModel> _referencedAssemblies = new List<AssemblyViewModel>();
        private bool _isSelected;
        private bool _isFound;
        
        private AssemblyViewModel(AssemblyInfo assembly)
        {
            _assembly = assembly;
            _correspondence.Add(assembly, this);
            foreach (var assemblyInfo in _assembly.ReferencedAssemblies)
            {
                _referencedAssemblies.Add(Create(assemblyInfo));
            }

            ToggleSelectionCommand = new DelegateCommand(ToggleSelectionCommandHandler);
        }

        public ICommand ToggleSelectionCommand { get; private set; }

        public bool IsProcessed { get; set; }
        public bool IsMarked { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsRoot { get; set; }
        public string Name { get { return _assembly.Name; } }        
        public string FullName { get { return _assembly.FullName; } }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public bool IsFound
        {
            get
            {
                return _isFound;
            }
            set
            {
                _isFound = value;
                OnPropertyChanged("IsFound");
            }
        }

        public IEnumerable<AssemblyViewModel> ReferencedAssemblies
        {
            get
            {
                return _referencedAssemblies;
            }
        }

        public AssemblyInfo AssemblyInfo
        {
            get { return _assembly; }
        }

        public static AssemblyViewModel Create(AssemblyInfo assemblyInfo)
        {
            if (_correspondence.ContainsKey(assemblyInfo))
            {
                return _correspondence[assemblyInfo];
            }
            return new AssemblyViewModel(assemblyInfo); 
        }

        public static void ClearCache()
        {
            _correspondence.Clear();
        }

        private void ToggleSelectionCommandHandler()
        {
            IsSelected = !IsSelected;
        }
    }
}
