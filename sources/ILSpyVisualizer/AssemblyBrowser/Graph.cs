// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Windows;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using ILSpyVisualizer.Controls.Graph;
using ILSpyVisualizer.Controls.Graph.GraphSharp.Layout;
using ILSpyVisualizer.Controls.Graph.QuickGraph;

namespace ILSpyVisualizer.AssemblyBrowser
{
	class TypeGraph : BidirectionalGraph<TypeViewModel, Edge<TypeViewModel>>
	{
		public TypeGraph(bool allowParallelEdges) : base(allowParallelEdges)
		{
		}		
	}

	class TypeGraphLayout : GraphLayout<TypeViewModel, Edge<TypeViewModel>, TypeGraph>
	{
		public event Action LayoutFinished;		

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
}
