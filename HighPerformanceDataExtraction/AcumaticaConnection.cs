using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighPerformanceDataExtraction
{
    class AcumaticaConnection
    {
        public AcumaticaConnection(string url, string username, string password)
        {
            Url = url;
            Username = username;
            Password = password;
        }
        
        public string Url { get; set; }
        public string Tenant { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
