using GalaSoft.MvvmLight.Ioc;
using pCloud.NET;
using pCloud.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace pCloud.ViewModels
{
    public partial class MainViewModel
    {
        partial void CopyLink()
        {
            this.CopyLink((File)this.SelectedItems.First());
        }

        private async Task CopyLink(File file)
        {
            this.IsLoading = true;
            try
            {
                var link = await this.client.GetPublicFileLinkAsync(file.FileId, null);
                var dataPackage = new DataPackage();
                dataPackage.SetText(link);
                dataPackage.SetWebLink(new Uri(link));
                Clipboard.SetContent(dataPackage);

                var notificationService = SimpleIoc.Default.GetInstance<NotificationService>();
                notificationService.ShowToast("File(s) shared", "A link has been copied to the clipboard", "Assets/Logo.scale-100.png");
            }
            finally
            {
                this.IsLoading = false;
            }
        }
    }
}
