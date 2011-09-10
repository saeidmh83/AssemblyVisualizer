// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Infrastructure;
using ILSpyVisualizer.Model;

namespace ILSpyVisualizer.DependencyBrowser
{
    class DependencyBrowserWindowViewModel : ViewModelBase
    {
        private IList<AssemblyInfo> _assemblies;
        private AssemblyGraph _assemblyGraph;       

        public DependencyBrowserWindowViewModel(IEnumerable<AssemblyInfo> assemblies)
        {
            _assemblies = assemblies.ToList();
            _assemblyGraph = CreateGraph(assemblies.Select(a => AssemblyViewModel.Create(a)));
        }

        public AssemblyGraph Graph
        {
            get 
            {
                return _assemblyGraph;
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
