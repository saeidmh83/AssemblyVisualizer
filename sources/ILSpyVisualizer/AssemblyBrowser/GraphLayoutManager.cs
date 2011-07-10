using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ILSpyVisualizer.AssemblyBrowser.ViewModels;

namespace ILSpyVisualizer.AssemblyBrowser
{
	static class GraphLayoutManager
	{
		private static readonly IDictionary<TypeViewModel, IDictionary<TypeViewModel, Point>>
			SavedLayouts = new Dictionary<TypeViewModel, IDictionary<TypeViewModel, Point>>();

		public static void SaveLayout(TypeViewModel type, IDictionary<TypeViewModel, Point> layout)
		{
			SavedLayouts.Add(type, layout);
		}

		public static IDictionary<TypeViewModel, Point> LoadLayout(TypeViewModel type)
		{
			if (SavedLayouts.ContainsKey(type))
			{
				return SavedLayouts[type];
			}
			return null;
		}
	}
}
