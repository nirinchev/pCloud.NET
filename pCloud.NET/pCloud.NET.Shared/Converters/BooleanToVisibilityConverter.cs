using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace pCloud.Converters
{
	public sealed class BooleanToVisibilityConverter : BooleanConverterBase<Visibility>
	{
		public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
	}
}
