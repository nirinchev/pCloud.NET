using System;
using System.Linq;
using System.Threading.Tasks;
using pCloud.NET.Dtos;
using pCloud.NET.Helpers;

namespace pCloud.NET
{
	public class pCloudClient
	{
		private const string GetDigestMethod = "getdigest";

		public async Task<bool> Login(string username, string password)
		{
			var digestEndpoint = GetEndpoint(GetDigestMethod);
			var digestResponse = await HttpRequestHelper.Get<DigestResponse>(digestEndpoint);
			var digest = digestResponse.Digest;
			return true;
		}

		private static string GetEndpoint(string relativeEndpoint)
		{
			return string.Format("http://api.pcloud.com/{0}", relativeEndpoint);
		}
	}
}