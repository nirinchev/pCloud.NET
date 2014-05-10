using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using pCloud.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.UI.Core;

namespace pCloud.ViewModels
{
    public class ShareViewModel : pCloudViewModelBase
    {
		private readonly ObservableCollection<SharedItemViewModelBase> sharedItems;
		private readonly pCloudClient pCloudClient = SimpleIoc.Default.GetInstance<pCloudClient>();

		private ShareOperation currentOperation;

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

		public ShareViewModel()
		{
			this.sharedItems = new ObservableCollection<SharedItemViewModelBase>();
			this.UploadCommand = new RelayCommand(this.UploadItems, this.SharedItems.Any);
			this.ShareCommand = new RelayCommand(this.ShareItems, this.SharedItems.Any);
			this.CancelCommand = new RelayCommand(this.Cancel);
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
						var sharedStorageItems = await operation.Data.GetStorageItemsAsync();

						this.SharedItems.Clear();

						foreach (var item in sharedStorageItems.OfType<StorageFolder>())
						{
							//this.SharedItems.Add(new SharedItemViewModel
						}

						foreach (var item in sharedStorageItems.OfType<StorageFile>())
						{
							this.SharedItems.Add(new SharedFileViewModel(item));
						}

						this.UploadCommand.RaiseCanExecuteChanged();
						this.ShareCommand.RaiseCanExecuteChanged();
					}
					catch
					{
						// TODO
					}
				} 
			}
		}

		private void Cancel()
		{
			this.currentOperation.ReportCompleted();
		}
    
		private async void UploadItems()
		{
			this.currentOperation.ReportStarted();
			try
			{
				var parentFolderId = await this.GetUploadFolderId();

				var test = await UploadFiles(parentFolderId);

				this.currentOperation.ReportCompleted();
			}
			catch
			{
				this.currentOperation.ReportError("An error occured while uploading the files. Try again later.");
			}
		}

		private async void ShareItems()
		{
			this.currentOperation.ReportStarted();

			try
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

				this.currentOperation.ReportCompleted();
			}
			catch
			{
				this.currentOperation.ReportError("An error occured while sharing the files. Try again later.");
			}
		}

		private async Task<IEnumerable<File>> UploadFiles(long parentFolderId)
		{
			var results = new List<File>();

			foreach (var vm in this.SharedItems.OfType<SharedFileViewModel>())
			{
				var fileStream = await Task.Run(async () =>
				{
					return await vm.File.OpenReadAsync();
				});

				var file = await this.pCloudClient.UploadFileAsync(fileStream.AsStreamForRead(), parentFolderId, vm.Name);
				results.Add(file);
			}

			return results;
		}

		private async Task<long> GetUploadFolderId()
		{
			var items = await this.pCloudClient.ListFolderAsync(ClientConstants.RootFolderId);
			var wpFolder = items.OfType<Folder>().FirstOrDefault(f => f.Name.Equals(ClientConstants.DefaultFolderName, StringComparison.OrdinalIgnoreCase));
			if (wpFolder == null)
			{
				wpFolder = await this.pCloudClient.CreateFolderAsync(ClientConstants.RootFolderId, ClientConstants.DefaultFolderName);
			}

			return wpFolder.FolderId;
		}
	}
}
