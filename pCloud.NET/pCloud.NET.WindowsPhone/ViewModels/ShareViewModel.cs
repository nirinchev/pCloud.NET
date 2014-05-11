using System;
using System.Linq;

namespace pCloud.ViewModels
{
	public partial class ShareViewModel
	{
		private bool isUrlGenerated;

		private string url;

		public bool IsUrlGenerated
		{
			get
			{
				return !string.IsNullOrEmpty(this.Url);
			}
		}

		public int UrlLength
		{
			get
			{
				return this.Url == null ? 0 : this.url.Length;
			}
		}

		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.Set(ref this.url, value);
				this.RaisePropertyChanged("IsUrlGenerated");
				this.RaisePropertyChanged("UrlLength");
			}
		}

		partial void HandleShareRequested(string shareLink)
		{
			this.IsBusy = false;
			this.Url = shareLink;
		}
	}
}