// Adopted, originally created as part of GraphSharp library
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

namespace AssemblyVisualizer.Controls.Graph.GraphSharp.Layout
{
	public class LinLogLayoutParameters : LayoutParametersBase
	{
		internal double attractionExponent = 1.0;

		public double AttractionExponent
		{
			get { return attractionExponent; }
			set
			{
				attractionExponent = value;
				NotifyPropertyChanged("AttractionExponent");
			}
		}

		internal double repulsiveExponent;

		public double RepulsiveExponent
		{
			get { return repulsiveExponent; }
			set
			{
				repulsiveExponent = value;
				NotifyPropertyChanged("RepulsiveExponent");
			}
		}

		internal double gravitationMultiplier = 0.1;

		public double GravitationMultiplier
		{
			get { return gravitationMultiplier; }
			set
			{
				gravitationMultiplier = value;
				NotifyPropertyChanged("GravitationMultiplier");
			}
		}

		internal int iterationCount = 100;

		public int IterationCount
		{
			get { return iterationCount; }
			set
			{
				iterationCount = value;
				NotifyPropertyChanged("IterationCount");
			}
		}
	}
}