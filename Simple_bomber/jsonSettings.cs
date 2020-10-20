using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_bomber
{
    public class sources
    {
        public string uri { get; set; }
        public string method { get; set; }
        public string phone_parameter { get; set; }
        public cookies[] cookies { get; set; }
        public parameters[] parameters { get; set; }
        public headers[] headers { get; set; }
        public string content_type { get; set; }
        public string accept { get; set; }
        public int delay { get; set; }
    }

    public class proxies
    {
        public string ip { get; set; }
        public int port { get; set; }
    }

    public class cookies
    {
        public string cookie { get; set; }
        public string value { get; set; }
    }
    public class headers
    {
        public string header { get; set; }
        public string value { get; set; }
    }

    public class parameters
    {
        public string parameter { get; set; }
        public string value { get; set; }
    }

    public class SettingsData
    {
        public sources[] sources { get; set; }
        public proxies[] proxies { get; set; }
        public int proxy_delay { get; set; }
        public bool multi_thread { get; set; }
    }
}
