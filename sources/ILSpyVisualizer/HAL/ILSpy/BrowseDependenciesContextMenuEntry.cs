// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using ILSpyVisualizer.Properties;
using ILSpyVisualizer.Model;
using ILSpyVisualizer.AncestryBrowser;
using ILSpyVisualizer.DependencyBrowser;

namespace ILSpyVisualizer.HAL.ILSpy
{
    [ExportContextMenuEntry(Header = "Browse Dependencies")]
    sealed class BrowseDependenciesContextMenuEntry : IContextMenuEntry
    {
        public bool IsVisible(SharpTreeNode[] selectedNodes)
        {
            return selectedNodes.OfType<AssemblyTreeNode>().Count() > 0;                   
        }

        public bool IsEnabled(SharpTreeNode[] selectedNodes)
        {
            return true;
        }

        public void Execute(SharpTreeNode[] selectedNodes)
        {
            var assemblyDefinitions = selectedNodes
                .OfType<AssemblyTreeNode>()
                .Select(n => n.LoadedAssembly.AssemblyDefinition);

            var window = new DependencyBrowserWindow(assemblyDefinitions.Select(HAL.Converter.Assembly))
            {
                Owner = Services.MainWindow
            };
            window.Show();
        }
    }
}
#endif