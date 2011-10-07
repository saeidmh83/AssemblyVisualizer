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
        private bool _showInternals;

        public TypeViewModel(TypeInfo typeInfo)
        {
            _typeInfo = typeInfo;
        }

        public string Name 
        {
            get { return _typeInfo.Name; }
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
                _isSelected = value;
                OnPropertyChanged("IsSelected");
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
            }
        }

        public Brush Foreground
        {
            get 
            {
                if (Background == null)
                {
                    return Brushes.Gray;
                }
                var backgroundColor = Background.Color;
                var multiplier = Math.Min(255.0 / backgroundColor.R, Math.Min(255.0 / backgroundColor.G, 255.0 / backgroundColor.B));
                var foregroundColor = new Color 
                { 
                    A = 255, 
                    R = (byte)(backgroundColor.R / 3), 
                    G = (byte)(backgroundColor.G / 3),
                    B = (byte)(backgroundColor.B / 3)
                };
                return new SolidColorBrush(foregroundColor);
            }
        }
        public SolidColorBrush Background { get; set; }
    }
}
