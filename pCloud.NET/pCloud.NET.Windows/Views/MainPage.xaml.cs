using GalaSoft.MvvmLight.Ioc;
using pCloud.ViewModels;
using Windows.UI.Xaml.Controls;

namespace pCloud.Views
{
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
            this.DataContext = SimpleIoc.Default.GetInstance<MainViewModel>();
		}
	}
}