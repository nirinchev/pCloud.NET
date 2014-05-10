using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace pCloud.NET
{
	public class pCloudClient : IDisposable
	{
        private readonly HttpClient httpClient;
        private readonly string authToken;

        public static async Task<pCloudClient> CreateClientAsync(string username, string password)
        {
            var client = new HttpClient { BaseAddress = new Uri("https://api.pcloud.com") };
            var uri = string.Format("userInfo?getauth=1&logout=1&username={0}&password={0}");
            dynamic userInfo = JToken.Parse(await client.GetStringAsync(uri));
            if (userInfo.result != 0)
            {
                throw (Exception)CreateException(userInfo);
            }

            return new pCloudClient(client, userInfo.auth);
        }

        private pCloudClient(HttpClient httpClient, string authToken)
        {
            this.httpClient = httpClient;
            this.authToken = authToken;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task CreateFolderAsync(long parentFolderId, string name)
        {
            await this.GetJsonAsync(this.BuildRequestUri("createfolder", new { folderid = parentFolderId, name = name }));
        }

        public async Task RenameFolderAsync(long folderId, string toPath)
        {
            await this.GetJsonAsync(this.BuildRequestUri("renamefolder", new { folderid = folderId, toPath = toPath }));
        }

        public async Task DeleteFolderAsync(long folderId, bool recursive)
        {

        }

        public async Task<IEnumerable<StorageItem>> ListFolderAsync(long folderId)
        {
            var requestUri = this.BuildRequestUri("listfolder", new { folderid = folderId });
            var response = await this.GetJsonAsync(requestUri);

            var result = new List<StorageItem>();
            foreach (var item in response.contents)
            {
                if (item.isfolder)
                {
                    result.Add(item.ToObject<Folder>());
                }
                else if (item.isfile)
                {
                    result.Add(item.ToObject<File>());
                }
            }

            return result.ToArray();
        }

        public async Task DownloadFileAsync(long fileId, Stream stream)
        {
            var requestUri = this.BuildRequestUri("getfilelink", new { fileid = fileId });
            var downloadResponse = await this.GetJsonAsync(requestUri);

            var fileUri = downloadResponse.hosts[0] + downloadResponse.path;
            HttpResponseMessage response = await this.httpClient.GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead);

            await response.Content.CopyToAsync(stream);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.httpClient.Dispose();
            }
        }

        private async Task<dynamic> GetJsonAsync(string uri)
        {
            dynamic json = JToken.Parse(await this.httpClient.GetStringAsync(uri));
            if (json.result != 0)
            {
                throw (Exception)CreateException(json);
            }

            return json;
        }

        private string BuildRequestUri(string method, object parameters = null)
        {
            var uri = new StringBuilder(method);
            uri.AppendFormat("?auth=", this.authToken);

            if (parameters != null)
            {
                foreach (var property in parameters.GetType().GetRuntimeProperties())
                {
                    var value = property.GetValue(parameters);
                    uri.AppendFormat("&{0}={1}", property.Name, Uri.EscapeDataString(value != null ? value.ToString() : string.Empty));
                }
            }

            return uri.ToString();
        }

        private static pCloudException CreateException(dynamic errorResponse)
        {
            return new pCloudException(errorResponse.result, errorResponse.error);
        }
	}
}