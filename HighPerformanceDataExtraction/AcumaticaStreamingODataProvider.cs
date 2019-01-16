using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HighPerformanceDataExtraction
{
    internal class AcumaticaStreamingODataProvider : IODataProvider
    {
        private AcumaticaConnection _connection;

        public AcumaticaStreamingODataProvider(AcumaticaConnection connection)
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
            var request = WebRequest.Create(url);
            string authInfo = _connection.Username + ":" + _connection.Password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
            request.ContentType = "application/json; charset=utf-8";

            var serializer = new JsonSerializer();

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
            }

            using (Stream stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        long totalRead = 0;

                        jsonReader.SupportMultipleContent = true;
                        while (jsonReader.Read())
                        {
                            if (jsonReader.TokenType == JsonToken.StartObject && jsonReader.Path.StartsWith("value["))
                            {
                                yield return serializer.Deserialize<T>(jsonReader);
                                totalRead++;
                            }
                        }
                    }
                }
            }
        }
    }
}
