using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using pCloud.Data;
using pCloud.NET;
using pCloud.Services;
using pCloud.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace pCloud.ViewModels
{
    public partial class FolderViewModel : pCloudViewModelBase
    {
        private readonly pCloudClient client;

        public SortedObservableCollection<StorageItem> Items { get; private set; }

        public ObservableCollection<StorageItem> SelectedItems { get; private set; }

        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }
            private set
            {
                if (this.isLoading != value)
                {
                    this.isLoading = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private ListedFolder folder;
        public ListedFolder Folder
        {
            get
            {
                return this.folder;
            }
            set
            {
                if (this.folder != value)
                {
                    this.folder = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool progressVisible;
        public bool ProgressVisible
        {
            get
            {
                return this.progressVisible;
            }
            private set
            {
                if (this.progressVisible != value)
                {
                    this.progressVisible = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private string progressMessage;
        public string ProgressMessage
        {
            get
            {
                return this.progressMessage;
            }
            private set
            {
                if (this.progressMessage != value)
                {
#if WINDOWS_APP
					this.RaisePropertyChanging();
#endif
                    this.progressMessage = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private ICommand cancelOperationCommand;
        public ICommand CancelOperationCommand
        {
            get
            {
                return this.cancelOperationCommand;
            }
            set
            {
                if (this.cancelOperationCommand != value)
                {
#if WINDOWS_APP
                    this.RaisePropertyChanging();
#endif
					this.cancelOperationCommand = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ICommand NavigateToItemCommand { get; private set; }

        public RelayCommand RefreshCommand { get; private set; }

        public RelayCommand UploadCommand { get; private set; }

        public RelayCommand SelectAllCommand { get; private set; }

        public RelayCommand ClearSelectionCommand { get; private set; }

        public RelayCommand DeleteItemsCommand { get; private set; }

        public RelayCommand OpenWithCommand { get; private set; }

        public RelayCommand CopyLinkCommand { get; private set; }

        public FolderViewModel(long folderId)
        {
            this.client = SimpleIoc.Default.GetInstance<pCloudClient>();

            this.Items = new SortedObservableCollection<StorageItem>();
            this.Items.SortDescriptors.Add(LambdaSortDescriptor.Create<StorageItem, bool>(item => item is Folder, ListSortDirection.Descending));
            this.Items.SortDescriptors.Add(new SortDescriptor("Name", ListSortDirection.Ascending));

            this.SelectedItems = new ObservableCollection<StorageItem>();
            this.SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;

            this.NavigateToItemCommand = new RelayCommand<ItemClickEventArgs>(this.NavigateToItem);
            this.RefreshCommand = new RelayCommand(() => this.Refresh());
            this.UploadCommand = new RelayCommand(() => this.Upload());
            this.SelectAllCommand = new RelayCommand(this.SelectAll);
            this.ClearSelectionCommand = new RelayCommand(this.SelectedItems.Clear, this.SelectedItems.Any);
            this.DeleteItemsCommand = new RelayCommand(() => this.DeleteSelectedItems(), this.SelectedItems.Any);
            this.OpenWithCommand = new RelayCommand(() => this.OpenSelectedFile(), () => this.SelectedItems.Count == 1 && this.SelectedItems.Single() is File);
			this.CopyLinkCommand = new RelayCommand(() => this.CopyLink(), () => this.SelectedItems.Count == 1 && this.SelectedItems.Single() is File);

            this.Populate(folderId);
        }

        private void OnSelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ClearSelectionCommand.RaiseCanExecuteChanged();
            this.DeleteItemsCommand.RaiseCanExecuteChanged();
            this.OpenWithCommand.RaiseCanExecuteChanged();
            this.CopyLinkCommand.RaiseCanExecuteChanged();
        }

        private async Task Populate(long folderId)
        {
            this.IsLoading = true;
            try
            {
                this.Folder = await this.client.ListFolderAsync(folderId);

                this.Items.Clear();
                foreach (var item in this.Folder.Contents)
                {
                    this.Items.Add(item);
                }
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        private async Task Refresh()
        {
            await this.Populate(this.Folder.FolderId);
        }

        private void NavigateToItem(ItemClickEventArgs args)
        {
            var item = (StorageItem)args.ClickedItem;
            if (item is File)
            {
                this.OpenFile((File)item);
            }
            else if (item is Folder)
            {
                this.OpenFolder((Folder)item);
            }
        }

        private void OpenFolder(Folder folder)
        {
            SimpleIoc.Default.GetInstance<NavigationService>().Navigate<FolderPage>(folder.FolderId);
        }

        private async Task OpenFile(File file, bool displayApplicationPicker = false)
        {
            StorageFile tempFile = null;
            using (var cts = new CancellationTokenSource())
            using (this.ShowProgress(string.Format("Opening {0}", file.Name), cts))
            {
                tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(file.Name, CreationCollisionOption.GenerateUniqueName);
                using (var stream = await System.IO.WindowsRuntimeStorageExtensions.OpenStreamForWriteAsync(tempFile))
                {
                    if (cts.IsCancellationRequested)
                    {
                        return;
                    }

                    await client.DownloadFileAsync(file.FileId, stream, cts.Token);
                }
            }

            var options = new LauncherOptions
            {
#if WINDOWS_APP
				DesiredRemainingView = this.GetPreferredViewSizeForFile(file),
#endif
                DisplayApplicationPicker = displayApplicationPicker
            };
            await Launcher.LaunchFileAsync(tempFile, options);
        }

#if WINDOWS_APP
        private Windows.UI.ViewManagement.ViewSizePreference GetPreferredViewSizeForFile(File file)
        {
            if (file.Category == Category.Audio)
            {
                return Windows.UI.ViewManagement.ViewSizePreference.UseMore;
            }

            return Windows.UI.ViewManagement.ViewSizePreference.UseLess;
        }
#endif

        private IDisposable ShowProgress(string message, CancellationTokenSource cts = null)
        {
            this.ProgressVisible = true;
            this.ProgressMessage = message;

            if (cts != null)
            {
                this.CancelOperationCommand = new RelayCommand(cts.Cancel);
            }

            return new Disposable(() =>
            {
                this.ProgressVisible = false;
                this.ProgressMessage = null;
                this.CancelOperationCommand = null;
            });
        }

        private async Task Upload()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add("*");

            var files = await picker.PickMultipleFilesAsync();

            using (var cts = new CancellationTokenSource())
            {
                foreach (var file in files)
                {
                    if (cts.IsCancellationRequested)
                    {
                        break;
                    }

                    using (this.ShowProgress(string.Format("Uploading {0}", file.DisplayName), cts))
                    using (var stream = await System.IO.WindowsRuntimeStorageExtensions.OpenStreamForReadAsync(file))
                    {
                        await this.client.UploadFileAsync(stream, this.Folder.FolderId, file.Name, cts.Token);
                    }
                }
				await this.Refresh();
            }
        }

        private void SelectAll()
        {
            this.SelectedItems.Clear();
            foreach (var item in this.Items)
            {
                this.SelectedItems.Add(item);
            }
        }

        private async Task DeleteSelectedItems()
        {
            var dialog = new MessageDialog(string.Format("Are you sure you wish to delete {0} item(s)?", this.SelectedItems.Count));
            var deleteCommand = new UICommand("Delete");
            dialog.Commands.Add(deleteCommand);
            dialog.Commands.Add(new UICommand("Cancel"));
            var result = await dialog.ShowAsync();

            if (result != deleteCommand)
            {
                return;
            }

            using (this.ShowProgress("Deleting"))
            {
                var tasks = new List<Task>();

                foreach (var item in this.SelectedItems)
                {
                    var folder = item as Folder;
                    if (folder != null)
                    {
                        tasks.Add(this.client.DeleteFolderAsync(folder.FolderId, true));
                        continue;
                    }

                    var file = item as File;
                    if (file != null)
                    {
                        tasks.Add(this.client.DeleteFileAsync(file.FileId));
                        continue;
                    }
                }

                await Task.WhenAll(tasks);
                
                foreach (var item in this.SelectedItems.ToArray())
                {
                    this.Items.Remove(item);
                }
            }
        }

        private async Task OpenSelectedFile()
        {
            var file = this.SelectedItems.SingleOrDefault() as File;
            if (file != null)
            {
                await this.OpenFile(file, true);
            }
        }

        partial void CopyLink();
    }
}
