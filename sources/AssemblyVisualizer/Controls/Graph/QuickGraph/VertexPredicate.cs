// Adopted, originally created as part of QuickGraph library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Diagnostics.Contracts;

namespace ILSpyVisualizer.Controls.Graph.QuickGraph
{
    [Pure]
    public delegate bool VertexPredicate<TVertex>(TVertex v);
}
