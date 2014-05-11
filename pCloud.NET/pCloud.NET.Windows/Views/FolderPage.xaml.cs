using GalaSoft.MvvmLight.Ioc;
using pCloud.ViewModels;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace pCloud.Views
{
    public sealed partial class FolderPage : Page
    {
        public FolderPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.DataContext = new FolderViewModel(e.Parameter != null ? (long)e.Parameter : 0L);
        }

        private void OnItemGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.itemGridView.SelectedItems.Count > 0)
            {
                this.AppBar.IsOpen = true;
            }
        }

        
    }
}