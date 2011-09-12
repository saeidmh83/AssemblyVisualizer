// Adopted, originally created as part of GraphSharp project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System.Windows;

namespace AssemblyVisualizer.Controls.Graph
{
    public class PositionChangedEventArgs : RoutedEventArgs
    {
        public double XChange { get; private set; }
        public double YChange { get; private set; }

        public PositionChangedEventArgs(RoutedEvent evt, object source, double xChange, double yChange)
            : base(evt, source)
        {
            XChange = xChange;
            YChange = yChange;
        }
    }
}
