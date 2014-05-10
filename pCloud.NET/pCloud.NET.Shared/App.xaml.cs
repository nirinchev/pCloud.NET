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
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
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
				var navigationService = SimpleIoc.Default.GetInstance<NavigationService>();
				var localStorageService = SimpleIoc.Default.GetInstance<LocalStorageService>();

				Type initialPageType = null;
				string authToken;
				if (localStorageService.TryGet(LocalStorageConstants.AuthTokenKey, out authToken, LocalStorageConstants.LoginContainer))
				{
					try
					{
						var client = await pCloudClient.CreateClientAsync(authToken);
						IocConfig.RegisterpCloudClient(client);
						initialPageType = typeof(MainPage);
					}
					catch
					{
					}
				}

				if (initialPageType == null)
				{
					initialPageType = typeof(LoginPage);
				}

                if (!navigationService.Navigate(initialPageType, e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif
    }
}