// Adopted, originally created as part of GraphSharp project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace ILSpyVisualizer.Controls.Graph
{
	public enum LayoutMode
	{
		/// <summary>
		/// Decide about the layout mode automatically.
		/// </summary>
		Automatic,

		/// <summary>
		/// There should not be any compound vertices.
		/// </summary>
		Simple,

		/// <summary>
		/// Compound vertices, compound graph.
		/// </summary>
		Compound
	}
}