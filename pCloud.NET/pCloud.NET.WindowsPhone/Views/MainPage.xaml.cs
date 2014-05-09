using System;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using pCloud.ViewModels;

namespace pCloud.Views
{
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
			this.DataContext = SimpleIoc.Default.GetInstance<MainViewModel>();
			this.NavigationCacheMode = NavigationCacheMode.Required;
		}
	}
}