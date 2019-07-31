using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramCoreMvcBot.Models
{
    public class AppSettings
    {
        public static string Url { get; set; } = "https://<YOUR URL>:443/{0}";
        public static string Name { get; set; } = "<BOT NAME>";
        public static string Key { get; set; } = "<BOT TOKEN>";
        public static string ConnString { get; set; } = "Username=;" +
                                                        "Password=;" +
                                                        "Host=;" +
                                                        "Port=5432;Database=;" +
                                                        "Pooling=true;Use SSL Stream=True;SSL Mode=Require;TrustServerCertificate=True;";
    }
}
