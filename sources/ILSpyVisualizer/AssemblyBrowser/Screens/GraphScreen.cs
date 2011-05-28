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
		#region // Static members

		private static TypeViewModel _currentType;

		private static TypeViewModel CurrentType
		{
			get { return _currentType; }
			set
			{
				if (_currentType != null)
				{
					_currentType.IsCurrent = false;
				}
				_currentType = value;
				value.IsCurrent = true;
			}
		}

		#endregion

		private TypeGraph _graph;
		private TypeViewModel _type;

		public GraphScreen(AssemblyBrowserWindowViewModel windowViewModel) : base(windowViewModel)
		{
			Commands.Add(WindowViewModel.ShowSearchUserCommand);
		}
		
		public event Action GraphChanged;

		public override bool AllowAssemblyDrop
		{
			get { return false; }
		}

		public TypeViewModel Type
		{
			get { return _type; }
			set
			{
				_type = value;
				OnPropertyChanged("Type");
			}
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
			CurrentType = type;
			Type = type;
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
