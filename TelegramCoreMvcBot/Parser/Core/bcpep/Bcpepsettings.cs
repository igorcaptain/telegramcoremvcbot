using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Core.bcpep
{
    class BcpepSettings : IParserSettings
    {
        public string BaseUrl { get; set; } = "http://bcpep.org.ua/";
        public string Prefix { get; set; } = "rozklad-ta-zamina/zamina-urokiv";
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
    }
}
