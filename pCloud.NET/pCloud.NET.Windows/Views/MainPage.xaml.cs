using System;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using pCloud.NET;
using pCloud.ViewModels;

namespace pCloud.Views
{
	public sealed partial class MainPage : Page
	{
		private MainViewModel ViewModel
		{
			get
			{
				return this.DataContext as MainViewModel;
			}
			set
			{
				this.DataContext = value;
			}
		}

		public MainPage()
		{
			this.InitializeComponent();
			this.ViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
			this.ViewModel.OpenFileRequested += this.OnOpenFileRequested;
			this.MediaPlayer.MediaFailed += (s,e) => 
			{
				this.UnloadMediaPlayer();
			};
		}

		private void OnOpenFileRequested(object sender, FileOpenRequestEventArgs e)
		{
			switch (e.FileType)
			{
				case Icon.Audio:
					this.LoadMediaPlayer(e.FileLink);
					break;
			}
		}

		private void LoadMediaPlayer(string uri)
		{
			this.MediaPlayer.Source = new Uri(uri);
			this.MediaPlayer.Play();
			this.UpdateMediaVisibility(Visibility.Visible);
		}

		private void UnloadMediaPlayer()
		{
			this.MediaPlayer.Stop();
			this.MediaPlayer.Source = null;
			this.UpdateMediaVisibility(Visibility.Collapsed);
		}

		private void UpdateMediaVisibility(Visibility visibility)
		{
			this.MediaPlayer.Visibility = visibility;
			this.PlayButton.Visibility = visibility;
			this.PauseButton.Visibility = visibility;
			this.StopButton.Visibility = visibility;
			
		}

		private void OnGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var gridView = (GridView)sender;
			this.AppBar.IsOpen = gridView.SelectedItems.Count > 0;
		}

		private void MediaPlayerButtonClick(object sender, RoutedEventArgs e)
		{
			var button = (AppBarButton)sender;
			switch (button.Label)
			{
				case "Play":
					this.MediaPlayer.Play();
					break;
				case "Pause":
					this.MediaPlayer.Pause();
					break;
				case "Stop":
					this.UnloadMediaPlayer();
					break;
			}
		}
	}
}