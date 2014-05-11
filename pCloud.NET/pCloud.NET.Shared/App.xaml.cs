using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using pCloud.NET;
using pCloud.Services;
using pCloud.ViewModels;
using pCloud.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace pCloud
{
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        public App()
        {
            this.InitializeComponent();

			IocConfig.RegisterTypes();

#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += this.OnHardwareButtonsBackPressed;
#endif
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.CacheSize = 1;

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

				this.HandleLaunch<FolderPage>(ClientConstants.RootFolderId);
            }

            Window.Current.Activate();
        }

		protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
		{
			var rootFrame = new Frame();
			Window.Current.Content = rootFrame;

			this.HandleLaunch<SharePage>(args.ShareOperation);

			Window.Current.Activate();
		}

		private void HandleLaunch<T>(object navigationParameter) where T : Page
		{
			var navigationService = SimpleIoc.Default.GetInstance<NavigationService>();
			var localStorageService = SimpleIoc.Default.GetInstance<LocalStorageService>();

			Type initialPageType = null;
			string authToken;
			if (localStorageService.TryGet(LocalStorageConstants.AuthTokenKey, out authToken, LocalStorageConstants.LoginContainer))
			{
				try
				{
					var client = pCloudClient.FromAuthToken(authToken);
					IocConfig.RegisterpCloudClient(client);
					initialPageType = typeof(T);
				}
				catch
				{
				}
			}

			if (initialPageType == null)
			{
				initialPageType = typeof(LoginPage);
			}

			if (!navigationService.Navigate(initialPageType, navigationParameter))
			{
				throw new Exception("Failed to create initial page");
			}
		}

#if WINDOWS_PHONE_APP
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        private void OnHardwareButtonsBackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
#endif
    }
}