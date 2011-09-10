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
using ILSpyVisualizer.AncestryBrowser;

namespace ILSpyVisualizer.HAL.Reflector
{
    class Package : IPackage
    {
        private IWindowManager windowManager;
        private ICommandBarManager commandBarManager;
        private IAssemblyBrowser assemblyBrowser;

        public void Load(IServiceProvider serviceProvider)
        {
            this.windowManager = (IWindowManager)serviceProvider.GetService(typeof(IWindowManager));
            this.assemblyBrowser = (IAssemblyBrowser)serviceProvider.GetService(typeof(IAssemblyBrowser));            

            this.commandBarManager = (ICommandBarManager)serviceProvider.GetService(typeof(ICommandBarManager));
            commandBarManager.CommandBars["Browser.Assembly"].Items.AddButton("Browse Assembly", BrowseAssemblyHandler);
            commandBarManager.CommandBars["Browser.TypeDeclaration"].Items.AddButton("Browse Ancestry", BrowseAncestryHandler);
        }

        private void BrowseAssemblyHandler(object sender, EventArgs e)
        { 
            var item = assemblyBrowser.ActiveItem as IAssembly;
        }

        private void BrowseAncestryHandler(object sender, EventArgs e)
        {
            var item = assemblyBrowser.ActiveItem as ITypeDeclaration;

            var window = new AncestryBrowserWindow(HAL.Converter.Type(item));
            /*{
                Owner = MainWindow.Instance
            };*/
            window.Show();
        }

        public void Unload()
        {
            
        }
    }
}
#endif