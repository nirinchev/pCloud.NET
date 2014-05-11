using GalaSoft.MvvmLight.Ioc;
using pCloud.NET;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace pCloud.Converters
{
    public class StorageItemIconUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = (StorageItem)value;
            if (item.HasThumbnail && item is File)
            {
                var client = SimpleIoc.Default.GetInstance<pCloudClient>();
                return new Uri(client.GetThumbnailUri(((File)item).FileId, parameter.ToString()));
            }

            return new Uri(string.Format("ms-appx:///Assets/Icons/{0}.png", item.Icon));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
