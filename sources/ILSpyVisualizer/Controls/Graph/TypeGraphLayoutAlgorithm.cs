using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GraphSharp.Algorithms.Layout.Simple.FDP;
using ILSpyVisualizer.AssemblyBrowser;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using QuickGraph;

namespace ILSpyVisualizer.Controls.Graph
{
	class TypeGraphLayoutAlgorithm : LinLogLayoutAlgorithm<TypeViewModel, Edge<TypeViewModel>, TypeGraph>
	{
		public TypeGraphLayoutAlgorithm(TypeGraph visitedGraph, IDictionary<TypeViewModel, Point> positions,
		                              LinLogLayoutParameters parameters )
			: base( visitedGraph, positions, parameters )
		{
			
		}

		protected override void InternalCompute()
		{
			var type = VisitedGraph.Root;
			var savedLayout = GraphLayoutManager.LoadLayout(type);
			if (savedLayout == null)
			{
				base.InternalCompute();
				GraphLayoutManager.SaveLayout(type, VertexPositions);
			}
			else
			{
				LoadSavedLayout(savedLayout);
			}
		}

		private void LoadSavedLayout(IDictionary<TypeViewModel, Point> results)
		{
			VertexPositions.Clear();

			foreach (var result in results)
			{
				VertexPositions.Add(result);
			}
		}
	}
}
