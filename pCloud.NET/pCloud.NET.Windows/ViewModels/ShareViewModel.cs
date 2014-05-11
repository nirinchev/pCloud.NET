using System;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Windows.ApplicationModel.DataTransfer;
using pCloud.Services;

namespace pCloud.ViewModels
{
	public partial class ShareViewModel
	{
		partial void HandleShareRequested(string shareLink)
		{
			var dataPackage = new DataPackage();
			dataPackage.SetText(shareLink);
			Clipboard.SetContent(dataPackage);

			var notificationService = SimpleIoc.Default.GetInstance<NotificationService>();
			notificationService.ShowToast("File(s) shared", "A link has been copied to the clipboard", "Assets/Logo.scale-100.png");

			this.Complete();
		}
	}
}