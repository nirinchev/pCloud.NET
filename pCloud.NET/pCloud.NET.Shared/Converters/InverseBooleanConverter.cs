using System;
using System.Collections.Generic;
using System.Text;

namespace pCloud.Converters
{
    public class InverseBooleanConverter : BooleanConverterBase<bool>
    {
		public InverseBooleanConverter() : base(false, true)
		{
		}
    }
}
