using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using pCloud.NET;
using pCloud.Services;
using pCloud.Views;
namespace pCloud.ViewModels
{
    public class LoginViewModel : pCloudViewModelBase
    {
		private readonly NavigationService navigationService = SimpleIoc.Default.GetInstance<NavigationService>();

		private string username;
		private string password;
		private bool isBusy;

		public RelayCommand LoginCommand { get; private set; }

		public string Username
		{
			get
			{
				return this.username;
			}
			set
			{
				if (this.username != value)
				{
					this.username = value;
					this.RaisePropertyChanged();
					this.LoginCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				if (this.password != value)
				{
					this.password = value;
					this.RaisePropertyChanged();
					this.LoginCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.isBusy;
			}
			set
			{
				if (this.isBusy != value)
				{
					this.isBusy = value;
					this.RaisePropertyChanged();
					this.LoginCommand.RaiseCanExecuteChanged();
				}
			}
		}

		public LoginViewModel()
		{
			this.LoginCommand = new RelayCommand(this.Login, this.CanLogin);
		}

		private async void Login()
		{
			this.IsBusy = true;
			try
			{
				var client = await pCloudClient.CreateClientAsync(this.Username, this.Password);
				SimpleIoc.Default.Register<pCloudClient>(() => client);
				this.navigationService.Navigate<MainPage>();
			}
			catch
			{
			}
			finally
			{
				this.IsBusy = false;
			}
		}

		private bool CanLogin()
		{
			return !string.IsNullOrEmpty(this.Password) &&
				   !string.IsNullOrEmpty(this.Username) &&
				   !this.IsBusy;
		}
    }
}
