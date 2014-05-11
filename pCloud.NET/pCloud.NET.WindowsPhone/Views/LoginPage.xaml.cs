using System;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using pCloud.ViewModels;

namespace pCloud.Views
{
	public sealed partial class LoginPage : Page
	{
		private LoginViewModel ViewModel
		{
			get
			{
				return this.DataContext as LoginViewModel;
			}
			set
			{
				this.DataContext = value;
			}
		}

		public LoginPage()
		{
			this.InitializeComponent();
			this.ViewModel = SimpleIoc.Default.GetInstance<LoginViewModel>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.ViewModel.SetSharingOperation(e.Parameter as ShareOperation);
		}
	}
}