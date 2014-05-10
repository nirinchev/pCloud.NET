using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace pCloud.Converters
{
	public class ThumbnailToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			BitmapImage image = null;

			if (value != null)
			{
				image = new BitmapImage();
				image.SetSource((StorageItemThumbnail)value);
			}

			return image;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
