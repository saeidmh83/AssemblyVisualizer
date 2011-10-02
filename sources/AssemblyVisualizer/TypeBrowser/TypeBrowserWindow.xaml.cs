// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mono.Cecil;
using System.Windows.Media.Animation;
using AssemblyVisualizer.Controls.ZoomControl;

namespace AssemblyVisualizer.TypeBrowser
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    partial class TypeBrowserWindow : Window
    {
        public TypeBrowserWindow(TypeDefinition typeDefinition)
        {
            InitializeComponent();
            ViewModel = new TypeBrowserWindowViewModel(typeDefinition);

            ViewModel.FillGraphRequest += FillGraphRequestHandler;
            ViewModel.OriginalSizeRequest += OriginalSizeRequestHandler;    

            WindowManager.AddTypeBrowser(this); 
        }

        public TypeBrowserWindowViewModel ViewModel
        {
            get
            {
                return DataContext as TypeBrowserWindowViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            WindowManager.RemoveTypeBrowser(this);
        }        
                
        private void FillGraphRequestHandler()
        {
            zoomControl.ZoomToFill();
        }

        private void OriginalSizeRequestHandler()
        {
            var animation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));
            zoomControl.BeginAnimation(ZoomControl.ZoomProperty, animation);
        }  
    }
}
#endif