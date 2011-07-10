// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using System.Windows;
using ILSpyVisualizer.Controls.Graph.QuickGraph;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.Layout
{
	public interface ICompoundLayoutAlgorithm<TVertex, TEdge, TGraph> : ILayoutAlgorithm<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : IBidirectionalGraph<TVertex, TEdge>
	{
	    IDictionary<TVertex, Size> InnerCanvasSizes { get; }
	}
}
