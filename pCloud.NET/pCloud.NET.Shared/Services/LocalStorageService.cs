using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace pCloud.Services
{
    public class LocalStorageService
    {
		private static readonly ApplicationDataContainer settingsContainer = Windows.Storage.ApplicationData.Current.LocalSettings;

		public void Set<T>(string key, T value, string container = null)
		{
			var currentSettings = GetCurrentSettings(container);

			currentSettings[key] = value;
		}

		public bool TryGet<T>(string key, out T result, string container = null)
		{
			var currentSettings = GetCurrentSettings(container);
			
			if (currentSettings.ContainsKey(key) && currentSettings[key] is T)
			{
				result = (T)currentSettings[key];
				return true;
			}

			result = default(T);
			return false;
		}

		private static IPropertySet GetCurrentSettings(string container = null)
		{
			if (container == null)
			{
				return settingsContainer.Values;
			}
			else
			{
				if (!settingsContainer.Containers.ContainsKey(container))
				{
					settingsContainer.CreateContainer(container, ApplicationDataCreateDisposition.Always);
				}

				return settingsContainer.Containers[container].Values;
			}
		}
    }
}
