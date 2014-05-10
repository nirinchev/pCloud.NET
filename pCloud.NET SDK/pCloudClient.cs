using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace pCloud.NET
{
	public class pCloudClient : IDisposable
	{
        private readonly HttpClient httpClient;
        private readonly string authToken;

		public string AuthToken
		{
			get
			{
				return this.authToken;
			}
		}

        public static Task<pCloudClient> CreateClientAsync(string username, string password)
        {
			return CreateClientAsync(Tuple.Create("username", username), Tuple.Create("password", password));
        }

		public static Task<pCloudClient> CreateClientAsync(string authToken)
		{
			return CreateClientAsync(Tuple.Create("auth", authToken));
		}

		private static async Task<pCloudClient> CreateClientAsync(params Tuple<string, string>[] queryParams)
		{
			var handler = new EncodingRewriterMessageHandler { InnerHandler = new HttpClientHandler() };
			var client = new HttpClient(handler) { BaseAddress = new Uri("http://api.pcloud.com") };
			var uri = string.Format("userinfo?getauth=1&logout=1&{0}", string.Join("&", queryParams.Select(q => string.Format("{0}={1}", q.Item1, q.Item2))));
			var userInfo = JsonConvert.DeserializeObject<dynamic>(await client.GetStringAsync(uri));
			if (userInfo.result != 0)
			{
				throw (Exception)CreateException(userInfo);
			}

			return new pCloudClient(client, (string)userInfo.auth);
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

        public async Task<Folder> CreateFolderAsync(long parentFolderId, string name)
        {
            var response = await this.GetJsonAsync(this.BuildRequestUri("createfolder", new { folderid = parentFolderId, name = name }));
            return response.metadata.ToObject<Folder>();
        }

        public async Task<Folder> RenameFolderAsync(long folderId, string toPath)
        {
            var response = await this.GetJsonAsync(this.BuildRequestUri("renamefolder", new { folderid = folderId, toPath = toPath }));
            return response.metadata.ToObject<Folder>();
        }

        public async Task DeleteFolderAsync(long folderId, bool recursive)
        {
            var method = recursive ? "deletefolderrecursive" : "deletefolder";
            await this.GetJsonAsync(this.BuildRequestUri(method, new { folderid = folderId }));
        }

        public async Task<IEnumerable<StorageItem>> ListFolderAsync(long folderId)
        {
            var requestUri = this.BuildRequestUri("listfolder", new { folderid = folderId });
            var response = await this.GetJsonAsync(requestUri);

            var result = new List<StorageItem>();
            foreach (var item in response.metadata.contents)
            {
                if ((bool)item.isfolder)
                {
                    result.Add(item.ToObject<Folder>());
                }
                else
                {
                    result.Add(item.ToObject<File>());
                }
            }

            return result.ToArray();
        }

        public async Task<File> UploadFileAsync(Stream file, long parentFolderId, string name)
        {
            var requestUri = this.BuildRequestUri("uploadfile", new { folderid = parentFolderId, filename = name, nopartial = 1 });

			var content = new MultipartFormDataContent();
			content.Add(new StreamContent(file), Guid.NewGuid().ToString(), name);
			var response = await this.httpClient.PostAsync(requestUri, content);
            var json = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            if (json.result != 0)
            {
                throw (Exception)CreateException(json);
            }

            return json.metadata[0].ToObject<File>();
        }

        public async Task DownloadFileAsync(long fileId, Stream stream)
        {
            var requestUri = this.BuildRequestUri("getfilelink", new { fileid = fileId });
            var downloadResponse = await this.GetJsonAsync(requestUri);

            var fileUri = string.Format("https://{0}{1}", downloadResponse.hosts[0], downloadResponse.path);
            HttpResponseMessage response = await this.httpClient.GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead);

            await response.Content.CopyToAsync(stream);
        }

        public async Task<File> CopyFileAsync(long fileId, long toFolderId, string toName)
        {
            var response = await this.GetJsonAsync(this.BuildRequestUri("copyfile", new { fileid = fileId, tofolderid = toFolderId, toname = toName}));
            return response.metadata.ToObject<File>();
        }

        public async Task<File> RenameFileAsync(long fileId, long toFolderId, string toName)
        {
            var response = await this.GetJsonAsync(this.BuildRequestUri("renamefile", new { fileid = fileId, tofolderid = toFolderId, toname = toName }));
            return response.metadata.ToObject<File>();
        }

        public async Task DeleteFileAsync(long fileId)
        {
            await this.GetJsonAsync(this.BuildRequestUri("deletefile", new { fileid = fileId }));
        }

        public async Task<string> GetPublicFileLinkAsync(long fileId, DateTime? expires)
        {
            var response = await this.GetJsonAsync(this.BuildRequestUri("getfilepublink", new { fileid = fileId, expire = expires }));
            return response.link;
        }

		public async Task<string> GetPublicFolderLinkAsync(long folderId, DateTime? expires)
		{
			var response = await this.GetJsonAsync(this.BuildRequestUri("getfolderpublink", new { folderid = folderId, expire = expires }));
			return response.link;
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
            var jsonString = await this.httpClient.GetStringAsync(uri);
            dynamic json = JsonConvert.DeserializeObject<dynamic>(jsonString);
            if (json.result != 0)
            {
                throw (Exception)CreateException(json);
            }

            return json;
        }

        private string BuildRequestUri(string method, object parameters = null)
        {
            var uri = new StringBuilder(method);
            uri.AppendFormat("?auth={0}", this.authToken);

            if (parameters != null)
            {
                foreach (var property in parameters.GetType().GetRuntimeProperties())
                {
                    var value = property.GetValue(parameters);
                    if (value != null)
                    {
                        uri.AppendFormat("&{0}={1}", property.Name, Uri.EscapeDataString(value.ToString()));
                    }
                }
            }

            return uri.ToString();
        }

        private static pCloudException CreateException(dynamic errorResponse)
        {
            return new pCloudException((int)errorResponse.result, (string)errorResponse.error);
        }
	}
}