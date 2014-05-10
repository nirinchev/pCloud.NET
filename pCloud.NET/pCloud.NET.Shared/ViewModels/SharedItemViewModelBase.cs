using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace pCloud.ViewModels
{
    public abstract class SharedItemViewModelBase : pCloudViewModelBase
    {
		private string name;
		private StorageItemThumbnail thumbnail;
		private DateTimeOffset dateCreated;
		
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.Set(ref this.name, value);
			}
		}

		public StorageItemThumbnail Thumbnail 
		{
			get
			{
				return this.thumbnail;
			}
			set
			{
				this.Set(ref this.thumbnail, value);
			}
		}

		public SharedItemViewModelBase(IStorageItem storageItem)
		{
			this.Name = storageItem.Name;
			this.GetAdditionalInformation(storageItem);
		}

		protected abstract Task GetAdditionalInformation(IStorageItem storageItem);
    }
}
