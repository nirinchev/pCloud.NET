using GalaSoft.MvvmLight.Ioc;
using pCloud.NET;
using pCloud.Services;
using pCloud.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace pCloud
{
    internal static class IocConfig
    {
		public static void RegisterTypes()
		{
			SimpleIoc.Default.Register<LoginViewModel>();
			SimpleIoc.Default.Register<NavigationService>();
			SimpleIoc.Default.Register<LocalStorageService>();
		}

		public static void RegisterpCloudClient(pCloudClient client)
		{
			if (SimpleIoc.Default.IsRegistered<pCloudClient>())
			{
				throw new InvalidOperationException("pCloudClient has already been registered");
			}

			SimpleIoc.Default.Register<pCloudClient>(() => client);
			var localStorageService = SimpleIoc.Default.GetInstance<LocalStorageService>();
			localStorageService.Set(LocalStorageConstants.AuthTokenKey, client.AuthToken, LocalStorageConstants.LoginContainer);
		}
    }
}
