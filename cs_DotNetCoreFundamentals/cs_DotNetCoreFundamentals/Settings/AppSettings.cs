using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_DotNetCoreFundamentals.Settings
{
    // This class represents the "AppSettings" section in appsettings.json.
    // Using a typed class is safer than reading strings everywhere.
    public class AppSettings
    {
        public string ProcessedBy { get; set; }
        public string RunMode { get; set; }
    }
}
