using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ILSpyVisualizer.AssemblyBrowser.Screens;

namespace ILSpyVisualizer.AssemblyBrowser.UserControls
{
	/// <summary>
	/// Interaction logic for CachingScreenPresenter.xaml
	/// </summary>
	partial class CachingScreenPresenter : UserControl
	{
		private static readonly Dictionary<Type, UserControl> ScreensDictionary = new Dictionary<Type, UserControl>();
		private static readonly Type[] NonCacheableScreenTypes = new[] {typeof (GraphScreen)};

		#region // Screen Dependency Property

		public Screen Screen
		{
			get { return (Screen)GetValue(ScreenProperty); }
			set { SetValue(ScreenProperty, value); }
		}
		
		public static readonly DependencyProperty ScreenProperty =
			DependencyProperty.Register("Screen", typeof(Screen), typeof(CachingScreenPresenter), new UIPropertyMetadata(null, ScreenPropertyChangedCallback));

		private static void ScreenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var cachingScreenPresenter = dependencyObject as CachingScreenPresenter;
			var screenType = e.NewValue.GetType();
			var view = GetViewForScreenType(screenType);

			view.DataContext = e.NewValue;
			cachingScreenPresenter.cpPresenter.Content = view;
		}

		#endregion

		public CachingScreenPresenter()
		{
			InitializeComponent();
		}

		private static UserControl GetViewForScreenType(Type screenType)
		{
			if (NonCacheableScreenTypes.Contains(screenType))
			{
				return CreateViewForScreenType(screenType);
			}

			if (!ScreensDictionary.ContainsKey(screenType))
			{
				ScreensDictionary[screenType] = CreateViewForScreenType(screenType);
			}
			return ScreensDictionary[screenType];
		}

		private static UserControl CreateViewForScreenType(Type screenType)
		{
			if (screenType == typeof(SearchScreen))
			{
				return new SearchScreenView();
			}
			if (screenType == typeof(GraphScreen))
			{
				return new GraphScreenView();
			}
			return new UserControl();
		}
	}
}
