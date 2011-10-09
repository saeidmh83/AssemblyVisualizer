// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Infrastructure;

namespace AssemblyVisualizer.InteractionBrowser
{
    class HierarchyViewModel : ViewModelBase
    {
        private bool _allSelected;

        public HierarchyViewModel(IEnumerable<TypeViewModel> types)
        {
            Types = types;
            foreach (var type in types)
            {
                type.Hierarchies.Add(this);
            }
            UpdateAllSelected();
        }

        public IEnumerable<TypeViewModel> Types { get; private set; }

        public bool AllSelected
        {
            get
            {
                return _allSelected;
            }
            set
            {                
                _allSelected = value;
                if (value)
                {
                    foreach (var type in Types)
                    {
                        type.DefineIsSelected(_allSelected);
                    }
                }
                else
                {
                    foreach (var type in Types.Where(t => t.Hierarchies.All(h => h.AllSelected == _allSelected)).ToArray())
                    {
                        type.DefineIsSelected(_allSelected);
                    }
                }
                OnPropertyChanged("AllSelected");
            }
        }

        public void NotifySelectionChanged()
        {
            UpdateAllSelected();
        }

        private void UpdateAllSelected()
        {
            _allSelected = Types.All(t => t.IsSelected);
            OnPropertyChanged("AllSelected");
        }
    }
}
