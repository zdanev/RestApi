using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RestApi
{
    public class RestApi: IDisposable
    {
        private readonly string _endpoint;

        public RestApi(string endpoint)
        {
            _endpoint = endpoint;
        }

        public async Task<string> Get(string path, params QueryParam[] queryParams)
        {
            var url = BuildUrl(path, queryParams);

            using (var http = new HttpClient())
            {
                var result = await http.GetAsync(url);

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return await result.Content.ReadAsStringAsync();

                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Unauthorized");

                    default:
                        throw new Exception(result.StatusCode.ToString());
                }
            }
        }

        public async Task<T> Get<T>(string path, params QueryParam[] queryParams)
        {
            var response = await Get(path, queryParams);
            var result = JsonConvert.DeserializeObject<T>(response);
            return result;
        }

        public async Task Put(string path, object payload, params QueryParam[] queryParams)
        {
            var data = "[]";

            if (payload != null)
            {
                data = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }

            var url = BuildUrl(path, queryParams);

            using (var http = new HttpClient())
            {
                var content = new ByteArrayContent(Encoding.ASCII.GetBytes(data));
                content.Headers.Add("Content-Type", "application/json");

                var result = await http.PutAsync(url, content);

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return;

                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Unauthorized");

                    default:
                        throw new Exception(result.StatusCode.ToString());
                }
            }
        }

        // todo: extract url builder class
        private string BuildUrl(string path, params QueryParam[] queryParams)
        {
            string url = _endpoint + path;

            var first = true;
            foreach (var param in queryParams)
            {
                if (!string.IsNullOrEmpty(param.Value))
                {
                    url += (first ? "?" : "&") + param.Name + "=" + Uri.EscapeDataString(param.Value);
                }
                first = false;
            }

            return url;
        }

        public void Dispose()
        {
        }
    }
}