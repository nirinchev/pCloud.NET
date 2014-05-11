using GalaSoft.MvvmLight.Ioc;
using pCloud.NET;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace pCloud.Converters
{
    public class StorageItemIconToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((Icon)value)
            {
                case Icon.Folder:
                    return "";
                case Icon.Font:
                    return "";
                case Icon.Presentation:
                    return "";
                case Icon.Audio:
                    return "";
                case Icon.Video:
                    return "";
                case Icon.Image:
                    return "";
                case Icon.Web:
                    return "";
                case Icon.GIS:
                    return "";
                case Icon.File:
                case Icon.Document:
                case Icon.Database:
                case Icon.Archive:
                case Icon.Spreadsheet:
                case Icon.DiskImage:
                case Icon.Package:
                case Icon.Executable:
                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
