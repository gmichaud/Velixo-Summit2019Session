using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HighPerformanceDataExtraction
{
    public class ODataResult<T>
    {
        [JsonProperty("odata.context")]
        public string Metadata { get; set; }
        [JsonProperty("value")]
        public List<T> Values { get; set; }
    }

    internal class AcumaticaSimpleODataProvider : IODataProvider
    {
        private AcumaticaConnection _connection;

        public AcumaticaSimpleODataProvider(AcumaticaConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<T> Load<T>(string endpoint, string filter = "")
        {
            var url = $"{_connection.Url}{endpoint}?$format=json";
            if (!String.IsNullOrEmpty(filter))
            {
                url += $"&$filter={filter}";
            }

            foreach (var row in LoadFromODataEndpoint<T>(url))
            {
                yield return row;
            }
        }

        private IEnumerable<T> LoadFromODataEndpoint<T>(string url)
        {
            using (var client = new HttpClient())
            {
                string authInfo = _connection.Username + ":" + _connection.Password;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo); 

                var json = client.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<ODataResult<T>>(json).Values;
            }
        }
    }
}
