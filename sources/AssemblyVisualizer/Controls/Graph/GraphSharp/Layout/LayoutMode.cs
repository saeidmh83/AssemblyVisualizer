// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.Layout
{
	public enum LayoutMode
	{
		/// <summary>
		/// Decide about the layout mode automatically.
		/// </summary>
		Automatic,

		/// <summary>
		/// Simple layout mode without compound vertices.
		/// </summary>
		Simple,

		/// <summary>
		/// Compound vertices, compound graph.
		/// </summary>
		Compound
	}
}