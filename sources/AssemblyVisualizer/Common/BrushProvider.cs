using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace AssemblyVisualizer.Common
{   
    class BrushProvider
    {
        public class BrushPair
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


        static BrushProvider()
        {
            var brushConverter = new BrushConverter();
            _brushes.Insert(0, brushConverter.ConvertFromString("#88FFB7A5") as SolidColorBrush);
            _brushes.Insert(0, brushConverter.ConvertFromString("#88BFE0FF") as SolidColorBrush);
            _brushes.Insert(0, brushConverter.ConvertFromString("#88D2FFB5") as SolidColorBrush);
        }

        private static IList<BrushPair> _brushPairs = new List<BrushPair>
			           	{
							new BrushPair("#2D6531", "#88D2FFB5"), // Green
							new BrushPair("#113DC2", "#88BFE0FF"), // Blue
							new BrushPair("#9B2119", "#88FFB7A5"), // Red
							new BrushPair("#746BFF", "#88B8B5FF"), // Purple
							new BrushPair("#AF00A7", "#88FFA0F2"), // Violet
							new BrushPair("#C18E00", "#88FFEAA8")  // Yellow
			           	};        

        private static IList<SolidColorBrush> _brushes = new List<SolidColorBrush>
            {   
                MakeTransparent(Brushes.LightGoldenrodYellow),                
                MakeTransparent(Brushes.LightPink), 
                MakeTransparent(Brushes.LightYellow), 
                MakeTransparent(Brushes.LightBlue),                
                MakeTransparent(Brushes.LightSalmon),
                MakeTransparent(Brushes.LightSkyBlue),                  
                MakeTransparent(Brushes.LightGreen),
                MakeTransparent(Brushes.LightCyan),                               
                MakeTransparent(Brushes.LightSeaGreen),                
            };

        public static IList<BrushPair> BrushPairs { get { return _brushPairs; } }
        public static IList<SolidColorBrush> SingleBrushes { get { return _brushes; } }

        private static SolidColorBrush MakeTransparent(SolidColorBrush brush)
        {
            var transparentColor = new Color
            {
                A = 136,
                R = brush.Color.R,
                G = brush.Color.G,
                B = brush.Color.B
            };
            return new SolidColorBrush(transparentColor);
        }
    }
}
