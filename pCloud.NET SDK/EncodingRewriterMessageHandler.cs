using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace pCloud.NET
{
	internal class EncodingRewriterMessageHandler : DelegatingHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
		{
			var response = await base.SendAsync(request, cancellationToken);
			if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.CharSet == "\"UTF8\"")
			{
				response.Content.Headers.ContentType.CharSet = "utf-8";
			}

			return response;
		}
	}
}