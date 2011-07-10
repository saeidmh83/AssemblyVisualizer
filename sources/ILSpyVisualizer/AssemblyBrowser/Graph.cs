// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Windows;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Simple.FDP;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Controls.Graph;
using QuickGraph;

namespace ILSpyVisualizer.AssemblyBrowser
{
	class TypeGraph : BidirectionalGraph<TypeViewModel, Edge<TypeViewModel>>
	{
		public TypeGraph(bool allowParallelEdges) : base(allowParallelEdges)
		{
		}

		public TypeViewModel Root { get; set; }
	}

	class TypeGraphLayout : GraphLayout<TypeViewModel, Edge<TypeViewModel>, TypeGraph>
	{
		public event Action LayoutFinished;

		protected override ILayoutAlgorithm<TypeViewModel, Edge<TypeViewModel>, TypeGraph> CreateLayoutAlgorithm
			(bool continueLayout, ILayoutContext<TypeViewModel, Edge<TypeViewModel>, TypeGraph> layoutContext)
		{
			return new TypeGraphLayoutAlgorithm(
								layoutContext.Graph,
								layoutContext.Positions,
								LayoutParameters as LinLogLayoutParameters);
		}

		protected override void OnLayoutFinished()
		{
			base.OnLayoutFinished();

			var handler = LayoutFinished;
			if (handler != null)
			{
				handler();
			}
		}
	}

	class TypeGraphLayoutAlgorithm : LinLogLayoutAlgorithm<TypeViewModel, Edge<TypeViewModel>, TypeGraph>
	{
		public TypeGraphLayoutAlgorithm(TypeGraph visitedGraph, IDictionary<TypeViewModel, Point> positions,
									  LinLogLayoutParameters parameters)
			: base(visitedGraph, positions, parameters)
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
