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
using AssemblyVisualizer.Controls.Graph.QuickGraph;
using AssemblyVisualizer.AssemblyBrowser;
using AssemblyVisualizer.HAL;

namespace AssemblyVisualizer.DependencyBrowser
{
    class DependencyBrowserWindowViewModel : ViewModelBase
    {
        private IEnumerable<AssemblyInfo> _assemblies;
        private IEnumerable<AssemblyViewModel> _assemblyViewModels;
        private AssemblyGraph _assemblyGraph;       

        public DependencyBrowserWindowViewModel(IEnumerable<AssemblyInfo> assemblies)
        {
            _assemblies = assemblies.ToList();
            var inputAssemblyViewModels = assemblies
                .Select(a => AssemblyViewModel.Create(a))
                .ToList();
            foreach(var vm in inputAssemblyViewModels)
            {
                vm.IsMarked = true;
            }
            _assemblyGraph = CreateGraph(inputAssemblyViewModels);
            _assemblyViewModels = _assemblyGraph.Vertices.ToList();
            Commands = new ObservableCollection<UserCommand>
			           	{
			           		new UserCommand(Resources.FillGraph, OnFillGraphRequest),
			           		new UserCommand(Resources.OriginalSize, OnOriginalSizeRequest),	
                            new UserCommand(Resources.Browse, BrowseCommandHandler),
                            new UserCommand(Resources.ClearSelection, ClearSelectionCommandHandler)
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

        private void BrowseCommandHandler()
        {
            var selectedAssemblies = _assemblyViewModels
                .Where(a => a.IsSelected)
                .Select(a => a.AssemblyInfo);
            Services.BrowseAssemblies(selectedAssemblies);            
        }

        private void ClearSelectionCommandHandler()
        {
            foreach (var assemblyViewModel in _assemblyViewModels)
            {
                assemblyViewModel.IsSelected = false;
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

                var edge = new Edge<AssemblyViewModel>(assembly, refAssembly);

                Edge<AssemblyViewModel> reverseEdge;
                var result = graph.TryGetEdge(refAssembly, assembly, out reverseEdge);
                if (result)
                {
                    reverseEdge.IsTwoWay = true;
                    edge.IsTwoWay = true;
                }

                graph.AddEdge(edge);
                if (!refAssembly.IsProcessed)
                {
                   AddReferencesRecursive(graph, refAssembly);
                }
            }
        }        
    }
}
