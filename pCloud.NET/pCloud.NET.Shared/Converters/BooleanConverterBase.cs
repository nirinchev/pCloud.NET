using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml.Data;

namespace pCloud.Converters
{
	public abstract class BooleanConverterBase<T> : IValueConverter
	{
		public BooleanConverterBase(T trueValue, T falseValue)
		{
			True = trueValue;
			False = falseValue;
		}

		public T True { get; set; }
		public T False { get; set; }


		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value is bool && ((bool)value) ? True : False;

		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
		}
	}
}
