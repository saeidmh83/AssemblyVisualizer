// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ICSharpCode.TreeView;
using ICSharpCode.ILSpy.TreeNodes;
using AssemblyVisualizer.TypeBrowser;

namespace AssemblyVisualizer.HAL.ILSpy
{
    [ExportContextMenuEntry(Header = "Browse Type")]
    sealed class MyAnalyzerContextMenuEntry : IContextMenuEntry
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

            var window = new TypeBrowserWindow(typeDefinition)
            {
                Owner = MainWindow.Instance
            };
            window.Show();
        }
    }
}

#endif