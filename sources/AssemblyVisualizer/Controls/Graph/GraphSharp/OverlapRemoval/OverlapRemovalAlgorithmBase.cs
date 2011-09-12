// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Collections.Generic;
using System.Windows;

namespace ILSpyVisualizer.Controls.Graph.GraphSharp.OverlapRemoval
{
	public abstract class OverlapRemovalAlgorithmBase<TObject, TParam> : AlgorithmBase, IOverlapRemovalAlgorithm<TObject, TParam>
		where TObject : class
		where TParam : IOverlapRemovalParameters
	{
		protected IDictionary<TObject, Rect> originalRectangles;
		public IDictionary<TObject, Rect> Rectangles
		{
			get { return originalRectangles; }
		}

		public TParam Parameters { get; private set; }

		public IOverlapRemovalParameters GetParameters()
		{
			return Parameters;
		}

		protected List<RectangleWrapper<TObject>> wrappedRectangles;


		public OverlapRemovalAlgorithmBase( IDictionary<TObject, Rect> rectangles, TParam parameters )
		{
			//eredeti téglalapok listája
			originalRectangles = rectangles;

			//wrapping the old rectangles, to remember which one belongs to which object
			wrappedRectangles = new List<RectangleWrapper<TObject>>();
			int i = 0;
			foreach ( var kvpRect in rectangles )
			{
				wrappedRectangles.Insert( i, new RectangleWrapper<TObject>( kvpRect.Value, kvpRect.Key ) );
				i++;
			}

			Parameters = parameters;
		}

		protected sealed override void InternalCompute()
		{
			AddGaps();

			RemoveOverlap();

			RemoveGaps();

			foreach ( var r in wrappedRectangles )
				originalRectangles[r.Id] = r.Rectangle;
		}

		protected virtual void AddGaps()
		{
			foreach ( var r in wrappedRectangles )
			{
				r.Rectangle.Width += Parameters.HorizontalGap;
				r.Rectangle.Height += Parameters.VerticalGap;
				r.Rectangle.Offset( -Parameters.HorizontalGap / 2, -Parameters.VerticalGap / 2 );
			}
		}

		protected virtual void RemoveGaps()
		{
			foreach ( var r in wrappedRectangles )
			{
				r.Rectangle.Width -= Parameters.HorizontalGap;
				r.Rectangle.Height -= Parameters.VerticalGap;
				r.Rectangle.Offset( Parameters.HorizontalGap / 2, Parameters.VerticalGap / 2 );
			}
		}

		protected abstract void RemoveOverlap();
	}
}