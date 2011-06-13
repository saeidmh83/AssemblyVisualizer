// Copyright 2011 Denis Markelov
// This code is distributed under Apache 2.0 license 
// (for details please see \docs\LICENSE, \docs\NOTICE)

using System.ComponentModel;

namespace ILSpyVisualizer.Infrastructure
{
	abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyname)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyname));
			}
		}
	}
}
