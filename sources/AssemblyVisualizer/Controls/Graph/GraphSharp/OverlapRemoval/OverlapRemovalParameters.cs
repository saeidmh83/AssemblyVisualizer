// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.OverlapRemoval
{
	public class OverlapRemovalParameters : IOverlapRemovalParameters
	{
		private float verticalGap = 10;
		private float horizontalGap = 10;
		
		public float VerticalGap
		{
			get { return verticalGap; }
			set
			{
				if ( verticalGap != value )
				{
					verticalGap = value;
					NotifyChanged( "VerticalGap" );
				}
			}
		}

		public float HorizontalGap
		{
			get { return horizontalGap; }
			set
			{
				if ( horizontalGap != value )
				{
					horizontalGap = value;
					NotifyChanged( "HorizontalGap" );
				}
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		protected void NotifyChanged( string propertyName )
		{
			if ( PropertyChanged != null )
				PropertyChanged( this, new System.ComponentModel.PropertyChangedEventArgs( propertyName ) );
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
}