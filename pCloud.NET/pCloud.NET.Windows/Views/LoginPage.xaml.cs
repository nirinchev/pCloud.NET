using System;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml.Controls;
using pCloud.ViewModels;

namespace pCloud.Views
{
	public sealed partial class LoginPage : Page
	{
		public LoginPage()
		{
			this.InitializeComponent();
			this.DataContext = SimpleIoc.Default.GetInstance<LoginViewModel>();
		}
	}
}