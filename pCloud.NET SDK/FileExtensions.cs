using System;
using System.Linq;

namespace pCloud.NET
{
	public static class FileExtensions
	{
		public static bool TryGetKnownFileType(this File file, out KnownFileType fileType)
		{
			return Enum.TryParse<KnownFileType>(file.Icon, true, out fileType);
		}
	}
}