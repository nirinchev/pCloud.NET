using pCloud.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace pCloud
{
    public class FileOpenRequestEventArgs : EventArgs
    {
		public string FileLink { get; private set; }
		public Icon FileType { get; private set; }

		public FileOpenRequestEventArgs(string fileLink, Icon fileType) : base()
		{
			this.FileLink = fileLink;
			this.FileType = fileType;
		}
    }
}
