// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    /// <summary>
    /// An edge factory
    /// </summary>
    public delegate TEdge EdgeFactory<TVertex, TEdge>(TVertex source, TVertex target)
        where TEdge : IEdge<TVertex>;
}
