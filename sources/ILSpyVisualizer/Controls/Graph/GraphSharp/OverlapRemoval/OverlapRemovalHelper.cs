// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.OverlapRemoval
{
	public static class OverlapRemovalHelper
	{
		public static Point GetCenter( this Rect r )
		{
			return new Point( r.Left + r.Width / 2, r.Top + r.Height / 2 );
		}
	}
}