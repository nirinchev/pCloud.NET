using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace pCloud.ViewModels
{
    public abstract class SharedItemViewModelBase : pCloudViewModelBase, IDisposable
    {
		private GCHandle rcwHandle;
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
			this.rcwHandle = GCHandle.Alloc(storageItem, GCHandleType.Normal);
			this.Name = storageItem.Name;
			this.GetAdditionalInformation(storageItem);
		}

		~SharedItemViewModelBase()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(false);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.rcwHandle.IsAllocated)
			{
				this.rcwHandle.Free();
			}
		}

		protected abstract Task GetAdditionalInformation(IStorageItem storageItem);
    }
}
