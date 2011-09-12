// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if Reflector
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reflector;
using Reflector.CodeModel;
using AssemblyVisualizer.AncestryBrowser;
using AssemblyVisualizer.DependencyBrowser;

namespace AssemblyVisualizer.HAL.Reflector
{
    class Package : IPackage
    {
        private IWindowManager _windowManager;
        private ICommandBarManager _commandBarManager;
        private static IAssemblyBrowser _assemblyBrowser;
        private static IAssemblyManager _assemblyManager;       

        public void Load(IServiceProvider serviceProvider)
        {
            this._windowManager = (IWindowManager)serviceProvider.GetService(typeof(IWindowManager));
            _assemblyBrowser = (IAssemblyBrowser)serviceProvider.GetService(typeof(IAssemblyBrowser));
            _assemblyManager = (IAssemblyManager)serviceProvider.GetService(typeof(IAssemblyManager));

            this._commandBarManager = (ICommandBarManager)serviceProvider.GetService(typeof(ICommandBarManager));
            _commandBarManager.CommandBars["Browser.Assembly"].Items.AddButton("Browse Assembly", BrowseAssemblyHandler);
            _commandBarManager.CommandBars["Browser.Assembly"].Items.AddButton("Browse Dependencies", BrowseDependenciesHandler);
            _commandBarManager.CommandBars["Browser.TypeDeclaration"].Items.AddButton("Browse Ancestry", BrowseAncestryHandler);            
        }

        public static IAssemblyManager AssemblyManager
        {
            get
            {
                return _assemblyManager;
            }
        }

        public static IAssemblyBrowser AssemblyBrowser
        {
            get
            {
                return _assemblyBrowser;
            }
        }

        private void BrowseAssemblyHandler(object sender, EventArgs e)
        { 
            var item = _assemblyBrowser.ActiveItem as IAssembly;
            var assemblies = new[] { HAL.Converter.Assembly(item) };
            if (WindowManager.AssemblyBrowsers.Count > 0)
            {
                var window = WindowManager.AssemblyBrowsers.First();
                window.ViewModel.AddAssemblies(assemblies);
                window.Activate();
            }
            else
            {
                var window = new AssemblyBrowser.AssemblyBrowserWindow(assemblies);
                System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
                window.Show();
            }
        }

        private void BrowseAncestryHandler(object sender, EventArgs e)
        {
            var item = _assemblyBrowser.ActiveItem as ITypeDeclaration;

            var window = new AncestryBrowserWindow(HAL.Converter.Type(item));
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();
        }

        private void BrowseDependenciesHandler(object sender, EventArgs e)
        {
            var item = _assemblyBrowser.ActiveItem as IAssembly;

            var window = new DependencyBrowserWindow(new [] { HAL.Converter.Assembly(item) });
            /*{
                Owner = Services.MainWindow
            };*/
            window.Show();
        }

        public void Unload()
        {
            
        }
    }
}
#endif