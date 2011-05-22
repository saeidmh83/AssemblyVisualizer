using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Infrastructure;
using QuickGraph;

namespace ILSpyVisualizer.AssemblyBrowser.Screens
{
	class GraphScreen : Screen
	{
		private TypeGraph _graph;

		public GraphScreen(AssemblyBrowserWindowViewModel windowViewModel) : base(windowViewModel)
		{
			Commands.Add(new UserCommand("Search", new DelegateCommand(() => WindowViewModel.ShowSearch())));
		}
		
		public event Action GraphChanged;

		public override bool AllowAssemblyDrop
		{
			get { return false; }
		}

		public TypeGraph Graph
		{
			get { return _graph; }
			set
			{
				_graph = value;
				OnPropertyChanged("Graph");
			}
		}

		public void Show(TypeViewModel type)
		{
			Graph = CreateGraph(type);
			OnGraphChanged();
		}

		private static TypeGraph CreateGraph(TypeViewModel typeViewModel)
		{
			var graph = new TypeGraph(true);
			var flattededHierarchy = typeViewModel.FlattenedHierarchy;
			graph.AddVertexRange(flattededHierarchy);
			foreach (var viewModel in flattededHierarchy)
			{
				if (viewModel.BaseType == null || viewModel == typeViewModel)
				{
					continue;
				}
				graph.AddEdge(new Edge<TypeViewModel>(viewModel, viewModel.BaseType));
			}
			return graph;
		}

		private void OnGraphChanged()
		{
			var handler = GraphChanged;

			if (handler != null)
			{
				GraphChanged();
			}
		}
	}
}
