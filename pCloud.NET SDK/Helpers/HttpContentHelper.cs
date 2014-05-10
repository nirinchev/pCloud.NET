using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace pCloud.NET.Helpers
{
	public static class HttpContentHelper
	{
		public static HttpContent GetJsonContent(object payload)
		{
			return HttpContentHelper.GetJsonContent(JsonConvert.SerializeObject(payload));
		}

		public static HttpContent GetJsonContent(string payload)
		{
			return new StringContent(payload, Encoding.UTF8, "application/json");
		}
	}
}