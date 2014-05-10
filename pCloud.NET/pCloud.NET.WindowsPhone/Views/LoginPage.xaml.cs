using System;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using pCloud.ViewModels;

namespace pCloud.Views
{
	public sealed partial class LoginPage : Page
	{
		public LoginPage()
		{
			this.InitializeComponent();
			this.DataContext = SimpleIoc.Default.GetInstance<LoginViewModel>();
			this.NavigationCacheMode = NavigationCacheMode.Required;
		}

		private void OnLoginButtonLostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			//this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
		}
	}
}