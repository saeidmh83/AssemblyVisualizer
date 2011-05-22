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
		
	}
}
