using pCloud.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace pCloud
{
    public class FileOpenRequestEventArgs : EventArgs
    {
		public string FileLink { get; private set; }
		public KnownFileType FileType { get; private set; }

		public FileOpenRequestEventArgs(string fileLink, KnownFileType fileType) : base()
		{
			this.FileLink = fileLink;
			this.FileType = fileType;
		}
    }
}
