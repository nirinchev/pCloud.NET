using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace pCloud.ViewModels
{
    public class SharedFileViewModel : SharedItemViewModelBase, IDisposable
    {
		public StorageFile File { get; private set; }

		public SharedFileViewModel(StorageFile file) : base(file)
		{
			this.File = file;
		}

		protected override async Task GetAdditionalInformation(IStorageItem storageItem)
		{
			var file = (StorageFile)storageItem;
			var thumb = await Task.Run(async () =>
			{
				return await this.GetThumbnailForFile(file);
			});

			this.Thumbnail = thumb;
		}

		private async Task<StorageItemThumbnail> GetThumbnailForFile(StorageFile file)
		{
			try
			{
				return await file.GetThumbnailAsync(ThumbnailMode.ListView, 120);
			}
			catch
			{
			}

			var assetsFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
			StorageFile iconFile = null;
			try
			{
				iconFile = await assetsFolder.GetFileAsync(string.Format("{0}-icon.png", file.FileType.Trim('.')));
			}
			catch
			{
			}

			if (iconFile == null)
			{
				iconFile = await assetsFolder.GetFileAsync("generic-icon.png");
			}

			return await iconFile.GetThumbnailAsync(ThumbnailMode.ListView, 120);
		}
	}
}
