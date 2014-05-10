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

		public bool Navigate(Type type)
		{
			return this.CurrentFrame.Navigate(type);
		}

		public bool Navigate(Type type, object parameter)
		{
			return this.CurrentFrame.Navigate(type, parameter);
		}
    }

	public static class NavigationServiceExtensions
	{
		public static bool Navigate<T>(this NavigationService navigationService) where T : Page
		{
			return navigationService.Navigate(typeof(T));
		}

		public static bool Navigate<T>(this NavigationService navigationService, object parameter) where T : Page
		{
			return navigationService.Navigate(typeof(T), parameter);
		}
	}
}
