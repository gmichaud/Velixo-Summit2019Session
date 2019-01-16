using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighPerformanceDataExtraction
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new AcumaticaConnection("http://localhost/AcumaticaDemo2018R2/OData/", "admin", "admin");

            //Simple, non-streaming OData load
            Console.WriteLine("Testing simple non-streaming provider...");
            IODataProvider provider = new AcumaticaSimpleODataProvider(connection);
            TestLoad<Data.InvoicedItem>(provider, "InvoicedItems");
            Console.WriteLine();

            //Streaming load
            Console.WriteLine("Testing streaming provider...");
            provider = new AcumaticaStreamingODataProvider(connection);
            TestLoad<Data.InvoicedItem>(provider, "InvoicedItems");
            Console.WriteLine();

            //Load accounts modified since... (LastModifiedDateTime)
            Console.WriteLine("Load accounts modified since (with LastModifiedDateTime)...");
            provider = new AcumaticaStreamingODataProvider(connection);
            DateTime lastModifiedDateTime = new DateTime(2019, 01, 15, 8, 30, 0);
            var filter = $"LastModifiedDateTime gt datetime'{lastModifiedDateTime:yyyy-MM-ddTHH:mm:ss.fff}'";
            TestLoad<Data.Account>(provider, "VelixoReportsPro-Accounts", filter);
            Console.WriteLine();

            //Load GL entries modified since... (tstamp)
            Console.WriteLine("Load GL entries modified since (with tstamp)...");
            provider = new AcumaticaStreamingODataProvider(connection);
            string lastTimestamp = Base64TimestampToODataBinaryLiteral("AAAAAAALWJY=");
            filter = $"Timestamp gt binary'{lastTimestamp}'";
            TestLoad<Data.GLHistory>(provider, "VelixoReportsPro-GLHistory", filter);
            Console.WriteLine();
            
            Console.ReadLine();
        }

        private static void TestLoad<T>(IODataProvider provider, string endpoint, string filter = "")
        {
            int i = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var item in provider.Load<T>(endpoint, filter))
            {
                if(i == 0)
                {
                    Console.WriteLine($"Got first item after {sw.ElapsedMilliseconds}ms.");
                }
                i++;
            }
            sw.Stop();
            Console.WriteLine($"Loaded {i} items in {sw.ElapsedMilliseconds}ms.");
        }

        private static string Base64TimestampToODataBinaryLiteral(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return new string('0', 16);
            }
            else
            {
                byte[] data = Convert.FromBase64String(value);
                return BitConverter.ToString(data).Replace("-", String.Empty);
            }
        }
    }
}
