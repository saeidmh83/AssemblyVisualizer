// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Infrastructure;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.Properties;
using System.Collections.ObjectModel;

namespace AssemblyVisualizer.DependencyBrowser
{
    class DependencyBrowserWindowViewModel : ViewModelBase
    {
        private IList<AssemblyInfo> _assemblies;
        private AssemblyGraph _assemblyGraph;       

        public DependencyBrowserWindowViewModel(IEnumerable<AssemblyInfo> assemblies)
        {
            _assemblies = assemblies.ToList();
            _assemblyGraph = CreateGraph(assemblies.Select(a => AssemblyViewModel.Create(a)));

            Commands = new ObservableCollection<UserCommand>
			           	{
			           		new UserCommand(Resources.FillGraph, OnFillGraphRequest),
			           		new UserCommand(Resources.OriginalSize, OnOriginalSizeRequest),	
			           	};
        }

        public event Action FillGraphRequest;
        public event Action OriginalSizeRequest;

        public IEnumerable<UserCommand> Commands { get; private set; }

        public AssemblyGraph Graph
        {
            get
            {
                return _assemblyGraph;
            }
        }

        private void OnFillGraphRequest()
        {
            var handler = FillGraphRequest;

            if (handler != null)
            {
                handler();
            }
        }

        private void OnOriginalSizeRequest()
        {
            var handler = OriginalSizeRequest;

            if (handler != null)
            {
                handler();
            }
        }

        private static AssemblyGraph CreateGraph(IEnumerable<AssemblyViewModel> assemblies)
        {
            var graph = new AssemblyGraph(true);
            
            foreach (var assembly in assemblies)
            {
                if (!graph.ContainsVertex(assembly))
                {
                    graph.AddVertex(assembly);
                }                              
                AddReferencesRecursive(graph, assembly);
            }

            AssemblyViewModel.ClearCache();
            return graph;
        }

        private static void AddReferencesRecursive(AssemblyGraph graph, AssemblyViewModel assembly)
        {
            assembly.IsProcessed = true;
            foreach (var refAssembly in assembly.ReferencedAssemblies)
            {
                if (!graph.ContainsVertex(refAssembly))
                {
                    graph.AddVertex(refAssembly);
                }

                var edge = new Controls.Graph.QuickGraph.Edge<AssemblyViewModel>(assembly, refAssembly);
                
                graph.AddEdge(edge);
                if (!refAssembly.IsProcessed)
                {
                   AddReferencesRecursive(graph, refAssembly);
                }
            }
        }
    }
}
