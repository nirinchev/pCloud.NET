using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace pCloud.Converters
{
	public sealed class InvertedBooleanToVisibilityConverter : BooleanConverterBase<Visibility>
	{
		public InvertedBooleanToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Visible) { }
	}
}
