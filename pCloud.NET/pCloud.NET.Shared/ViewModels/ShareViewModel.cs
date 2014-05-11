using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using pCloud.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;

namespace pCloud.ViewModels
{
    public partial class ShareViewModel : pCloudViewModelBase
    {
		private readonly ObservableCollection<SharedItemViewModelBase> sharedItems;
		private readonly pCloudClient pCloudClient = SimpleIoc.Default.GetInstance<pCloudClient>();

		private ShareOperation currentOperation;
		private bool isBusy;

		public RelayCommand UploadCommand { get; set; }
		public RelayCommand ShareCommand { get; set; }
		public RelayCommand CancelCommand { get; set; }

		public ObservableCollection<SharedItemViewModelBase> SharedItems
		{
			get
			{
				return this.sharedItems;
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.isBusy;
			}
			set
			{
				this.Set(ref this.isBusy, value);
			}
		}

		public ShareViewModel()
		{
			this.sharedItems = new ObservableCollection<SharedItemViewModelBase>();
			this.UploadCommand = new RelayCommand(this.UploadItems, this.SharedItems.Any);
			this.ShareCommand = new RelayCommand(this.ShareItems, this.SharedItems.Any);
			this.CancelCommand = new RelayCommand(this.Complete);
		}

		public async void Initialize(ShareOperation operation)
		{
			if (operation != null)
			{
				this.currentOperation = operation;

				if (operation.Data.Contains(StandardDataFormats.StorageItems))
				{
					try
					{
						this.ClearItems();
					}
					catch
					{
					}

					var sharedStorageItems = await operation.Data.GetStorageItemsAsync();

					foreach (var item in sharedStorageItems.OfType<StorageFile>())
					{
						this.SharedItems.Add(new SharedFileViewModel(item));
					}

					this.UploadCommand.RaiseCanExecuteChanged();
					this.ShareCommand.RaiseCanExecuteChanged();
				} 
			}
		}

		private void Complete()
		{
			this.ClearItems();
			this.currentOperation.ReportCompleted();
		}

		private void ClearItems()
		{
			foreach (var item in this.SharedItems)
			{
				item.Dispose();
			}

			this.SharedItems.Clear();
		}

		private void UploadItems()
		{
			this.ExecuteRequest(this.UploadItemsCore(), "An error occured while uploading the files. Try again later.");
		}

		private async Task UploadItemsCore()
		{
			var parentFolderId = await this.GetUploadFolderId();

			await UploadFiles(parentFolderId);

			this.Complete();
		}

		private void ShareItems()
		{
			this.ExecuteRequest(this.ShareItemsCore(), "An error occured while sharing the files. Try again later.");
		}

		private async Task ShareItemsCore()
		{
			var parentFolderId = await this.GetUploadFolderId();

			var files = this.SharedItems.OfType<SharedFileViewModel>();
			var isFolderUpload = files.Count() > 1;
			if (isFolderUpload)
			{
				var newFolder = await this.pCloudClient.CreateFolderAsync(parentFolderId, DateTime.Now.ToString("yyyy.MM.dd - HH.mm.ss"));
				parentFolderId = newFolder.FolderId;
			}

			var uploadedFiles = await this.UploadFiles(parentFolderId);
			
			string shareLink;
			if (isFolderUpload)
			{
				shareLink = await pCloudClient.GetPublicFolderLinkAsync(parentFolderId, null);
			}
			else
			{
				shareLink = await pCloudClient.GetPublicFileLinkAsync(uploadedFiles.First().FileId, null);
			}
			
			this.HandleShareRequested(shareLink);
		}

		private async Task ExecuteRequest(Task request, string errorMessage)
		{
			this.currentOperation.ReportStarted();
			this.IsBusy = true;

			try
			{
				await request;
			}
			catch
			{
				this.currentOperation.ReportError(errorMessage);
			}
			finally
			{
				this.IsBusy = false;
			}
		}

		partial void HandleShareRequested(string shareLink);

		private async Task<IEnumerable<File>> UploadFiles(long parentFolderId)
		{
			var results = new List<File>();

			foreach (var vm in this.SharedItems.OfType<SharedFileViewModel>())
			{
				var fileStream = await Task.Run(async () =>
				{
					return await vm.File.OpenReadAsync();
				});

				var file = await this.pCloudClient.UploadFileAsync(fileStream.AsStreamForRead(), parentFolderId, vm.Name, default(CancellationToken));
				results.Add(file);
			}

			return results;
		}

		private async Task<long> GetUploadFolderId()
		{
			var folder = await this.pCloudClient.ListFolderAsync(ClientConstants.RootFolderId);
			var wpFolder = folder.Contents.OfType<Folder>().FirstOrDefault(f => f.Name.Equals(ClientConstants.DefaultFolderName, StringComparison.OrdinalIgnoreCase));
			if (wpFolder == null)
			{
				wpFolder = await this.pCloudClient.CreateFolderAsync(ClientConstants.RootFolderId, ClientConstants.DefaultFolderName);
			}

			return wpFolder.FolderId;
		}
	}
}
