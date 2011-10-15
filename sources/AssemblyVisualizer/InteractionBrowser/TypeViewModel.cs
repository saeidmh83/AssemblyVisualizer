// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.Infrastructure;
using System.Windows.Media;

namespace AssemblyVisualizer.InteractionBrowser
{
    class TypeViewModel : ViewModelBase
    {
        private TypeInfo _typeInfo;
        private bool _isSelected;
        private bool _showInternals = true;
        private InteractionBrowserWindowViewModel _windowViewModel;
        private SolidColorBrush _background;
        private SolidColorBrush _foreground = Brushes.Gray;

        public TypeViewModel(TypeInfo typeInfo, InteractionBrowserWindowViewModel windowViewModel)
        {
            _typeInfo = typeInfo;
            _windowViewModel = windowViewModel;

            Hierarchies = new List<HierarchyViewModel>();
        }

        public string Name 
        {
            get { return _typeInfo.Name; }
        }

        public string FullName
        {
            get { return _typeInfo.FullName; }
        }

        public TypeInfo TypeInfo
        {
            get
            {
                return _typeInfo;
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                DefineIsSelected(value);
                NotifyHierarchiesSelectionChanged();
            }
        }

        public bool ShowInternals
        {
            get
            {
                return _showInternals;
            }
            set
            {
                _showInternals = value;
                OnPropertyChanged("ShowInternals");
                _windowViewModel.ReportSelectionChanged();
            }
        }

        public IList<HierarchyViewModel> Hierarchies { get; private set; }

        public SolidColorBrush Foreground
        {
            get 
            {
                return _foreground;
            }
            set
            {
                _foreground = value;
                OnPropertyChanged("Foreground");
            }
        }

        public SolidColorBrush Background 
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                OnPropertyChanged("Background");
                Foreground = GetForeground(value);
            }
        }

        public void DefineIsSelected(bool isSelected)
        {
            _isSelected = isSelected;
            OnPropertyChanged("IsSelected");
            _windowViewModel.ReportSelectionChanged();
        }

        private void NotifyHierarchiesSelectionChanged()
        {
            if (Hierarchies.Count > 0)
            {
                foreach (var hierarchy in Hierarchies)
                {
                    hierarchy.NotifySelectionChanged(); 
                }
            }
        }

        private static SolidColorBrush GetForeground(SolidColorBrush background)
        {
            if (background == null)
            {
                return Brushes.Gray;
            }
            var backgroundColor = background.Color;
            var foregroundColor = new Color
            {
                A = 255,
                R = (byte)(backgroundColor.R / 2.5),
                G = (byte)(backgroundColor.G / 2.5),
                B = (byte)(backgroundColor.B / 2.5)
            };
            return new SolidColorBrush(foregroundColor);
        }
    }
}
