using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ILSpyVisualizer.Common
{
    class BrushPair
    {
        private static readonly BrushConverter BrushConverter = new BrushConverter();

        public BrushPair(string caption, string background)
            : this(BrushConverter.ConvertFromString(caption) as Brush,
                  BrushConverter.ConvertFromString(background) as Brush)
        {
        }

        private BrushPair(Brush caption, Brush background)
        {
            Caption = caption;
            Background = background;
        }

        public Brush Caption { get; private set; }
        public Brush Background { get; private set; }
    }

    class BrushProvider
    {
        private static IList<BrushPair> _brushes = new List<BrushPair>
			           	{
							new BrushPair("#2D6531", "#D2FFB5"), // Green
							new BrushPair("#113DC2", "#BFE0FF"), // Blue
							new BrushPair("#9B2119", "#FFB7A5"), // Red
							new BrushPair("#746BFF", "#B8B5FF"), // Purple
							new BrushPair("#AF00A7", "#FFA0F2"), // Violet
							new BrushPair("#C18E00", "#FFEAA8")  // Yellow
			           	};
        public static IList<BrushPair> BrushPairs { get { return _brushes; } }
    }
}
