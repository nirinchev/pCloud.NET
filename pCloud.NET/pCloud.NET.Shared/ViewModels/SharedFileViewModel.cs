using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace pCloud.ViewModels
{
    public class SharedFileViewModel : SharedItemViewModelBase
    {
		public StorageFile File { get; private set; }

		public SharedFileViewModel(StorageFile file) : base(file)
		{
			this.File = file;
		}

		protected override async Task GetAdditionalInformation(IStorageItem storageItem)
		{
			var file = (StorageFile)storageItem;
			this.Thumbnail = await file.GetThumbnailAsync(ThumbnailMode.ListView);
		}
	}
}
