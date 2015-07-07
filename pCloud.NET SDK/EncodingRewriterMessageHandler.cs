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
            var contentType = response.Content.Headers.ContentType;
            if (contentType != null && contentType.CharSet == "\"UTF-8\"")
				contentType.CharSet = "utf-8";

			return response;
		}
	}
}