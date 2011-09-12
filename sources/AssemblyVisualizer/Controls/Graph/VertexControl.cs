// Adopted, originally created as part of GraphSharp project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows;
using System.Windows.Controls;
using System;
using ILSpyVisualizer.Controls.Graph.Helpers;

namespace ILSpyVisualizer.Controls.Graph
{
	/// <summary>
	/// Logical representation of a vertex.
	/// </summary>
	public class VertexControl : Control, IPoolObject, IDisposable
	{
		public object Vertex
		{
			get { return GetValue( VertexProperty ); }
			set { SetValue( VertexProperty, value ); }
		}

		public static readonly DependencyProperty VertexProperty =
			DependencyProperty.Register( "Vertex", typeof( object ), typeof( VertexControl ), new UIPropertyMetadata( null ) );


        public GraphCanvas RootCanvas
        {
            get { return (GraphCanvas)GetValue(RootCanvasProperty); }
            set { SetValue(RootCanvasProperty, value); }
        }

        public static readonly DependencyProperty RootCanvasProperty =
            DependencyProperty.Register("RootCanvas", typeof(GraphCanvas), typeof(VertexControl), new UIPropertyMetadata(null));

		static VertexControl()
		{
			//override the StyleKey Property
			DefaultStyleKeyProperty.OverrideMetadata( typeof( VertexControl ), new FrameworkPropertyMetadata( typeof( VertexControl ) ) );
		}

		#region IPoolObject Members

		public void Reset()
		{
			Vertex = null;
		}

		public void Terminate()
		{
			//nothing to do, there are no unmanaged resources
		}

		public event DisposingHandler Disposing;

		public void Dispose()
		{
			if ( Disposing != null )
				Disposing( this );
		}

		#endregion
	}
}