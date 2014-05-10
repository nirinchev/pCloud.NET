using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pCloud.NET.Helpers
{
	public static class HttpRequestHelper
	{
		public static Task<T> Get<T>(string endpoint, IDictionary<string, string> headers = null)
		{
			return Send<T>(HttpMethod.Get, endpoint, null, headers);
		}

		public static Task<T> Post<T>(string endpoint, object payload = null, IDictionary<string, string> headers = null, Action<HttpClient> setupClient = null)
		{
			return Send<T>(HttpMethod.Post, endpoint, payload, headers, setupClient);
		}

		public static async Task Delete(string endpoint, IDictionary<string, string> headers = null)
		{
			await Send(HttpMethod.Delete, endpoint, null, headers);
		}

		private static async Task<T> Send<T>(HttpMethod method, string endpoint, object payload, IDictionary<string, string> headers, Action<HttpClient> setupClient = null)
		{
			var response = await Send(method, endpoint, payload, headers, setupClient);
			using (var stream = await response.Content.ReadAsStreamAsync())
			using (var sr = new StreamReader(stream))
			{
				var content = sr.ReadToEnd();
				return JsonConvert.DeserializeObject<T>(content);
			}
		}

		private static async Task<HttpResponseMessage> Send(HttpMethod method, string endpoint, object payload, IDictionary<string, string> headers, Action<HttpClient> setupClient = null)
		{
			using (var client = new HttpClient())
			{
				if (setupClient != null)
				{
					setupClient(client);
				}

				var request = new HttpRequestMessage(method, endpoint);
				if (payload != null)
				{
					request.Content = HttpContentHelper.GetJsonContent(payload);
				}

				if (headers != null)
				{
					foreach (var header in headers)
					{
						request.Headers.TryAddWithoutValidation(header.Key, header.Value);
					}
				}
				var response = await client.SendAsync(request);
				CheckIsSuccess(response, string.Format("Unable to {0} to {1}", method.Method, endpoint));
				return response;
			}
		}

		private static void CheckIsSuccess(HttpResponseMessage response, string message)
		{
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(string.Format("{0}\nStatus code: {1}", message, response.StatusCode));
			}
		}
	}
}