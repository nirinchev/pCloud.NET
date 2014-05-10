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
		}
    }
}
