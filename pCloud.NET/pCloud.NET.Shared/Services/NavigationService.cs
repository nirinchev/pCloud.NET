using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace pCloud.Services
{
    public class NavigationService
    {
		private Frame CurrentFrame
		{
			get
			{
				return (Frame)Window.Current.Content;
			}
		}

		public bool Navigate<T>() where T : Page
		{
			return this.CurrentFrame.Navigate(typeof(T));
		}

		public bool Navigate<T>(object parameter) where T : Page
		{
			return this.CurrentFrame.Navigate(typeof(T), parameter);
		}
    }
}
