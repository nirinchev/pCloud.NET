using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using pCloud.NET;
namespace pCloud.ViewModels
{
    public class MainViewModel : pCloudViewModelBase
    {
		public MainViewModel()
		{
			var client = new pCloudClient();
			client.Login("aa", "bb");
		}
    }
}
