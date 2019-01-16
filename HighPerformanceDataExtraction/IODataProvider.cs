using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighPerformanceDataExtraction
{
    interface IODataProvider
    {
        IEnumerable<T> Load<T>(string endpoint, string filter = "");
    }
}
