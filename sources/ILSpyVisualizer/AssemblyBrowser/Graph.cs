// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphSharp.Controls;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;
using QuickGraph;

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
