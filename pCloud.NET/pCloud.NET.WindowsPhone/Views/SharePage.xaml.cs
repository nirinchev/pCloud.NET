using System;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using pCloud.ViewModels;

namespace pCloud.Views
{
	public sealed partial class SharePage : Page
	{
		private ShareViewModel ViewModel
		{
			get
			{
				return this.DataContext as ShareViewModel;
			}
			set
			{
				this.DataContext = value;
			}
		}

		public SharePage()
		{
			this.InitializeComponent();
			this.ViewModel = SimpleIoc.Default.GetInstance<ShareViewModel>();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			var shareOperation = e.Parameter as ShareOperation;
			this.ViewModel.Initialize(shareOperation);
		}
	}
}