using System.Net;
using System.Threading.Tasks;
using System.Net.Http;

namespace Parser.Core
{
    class HtmlLoader
    {
        readonly HttpClient client;
        readonly string url;
        public HtmlLoader(IParserSettings settings)
        {
            client = new HttpClient();
            url = $"{settings.BaseUrl}{settings.Prefix}/";
        }
        public async Task<string> GetSourceOfPage()
        {
            string source = null;
            string currentUrl = url;
            try
            {
                var response = await client.GetAsync(currentUrl);
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    source = await response.Content.ReadAsStringAsync();
                }
            }
            catch {}
            return source;
        }
    }
}
