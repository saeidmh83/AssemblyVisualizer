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
using AssemblyVisualizer.Properties;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.AssemblyBrowser;

namespace AssemblyVisualizer.HAL.ILSpy
{
    [ExportContextMenuEntry(Header = "Visualize Descendants")]
    sealed class VisualizeDescendantsContextMenuEntry : IContextMenuEntry
    {
        public bool IsVisible(SharpTreeNode[] selectedNodes)
        {
            return (selectedNodes.Count() == 1)
                   && (selectedNodes.Single() is TypeTreeNode);
        }

        public bool IsEnabled(SharpTreeNode[] selectedNodes)
        {
            return true;
        }

        public void Execute(SharpTreeNode[] selectedNodes)
        {
            var typeDefinition = selectedNodes
                .OfType<TypeTreeNode>()
                .Single().TypeDefinition;
            var type = HAL.Converter.Type(typeDefinition);
            var assembly = type.Module.Assembly;

            var window = new AssemblyBrowserWindow(new [] { assembly }, type)
            {
                Owner = MainWindow.Instance
            };
            window.Show();
        }
    }
}
#endif