using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Notifications;

namespace pCloud.Services
{
    public class NotificationService
    {
		public void ShowToast(string title, string message, string imageUrl)
		{
			var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
			var titleElement = template.GetElementsByTagName("text")[0];
			titleElement.AppendChild(template.CreateTextNode(title));

			var messageElement = template.GetElementsByTagName("text")[1];
			messageElement.AppendChild(template.CreateTextNode(message));

			var imageElement = template.GetElementsByTagName("image")[0];
			imageElement.Attributes[1].NodeValue = imageUrl;

			var toast = new ToastNotification(template);
			var notifier = ToastNotificationManager.CreateToastNotifier();
			notifier.Show(toast);
		}
    }
}
