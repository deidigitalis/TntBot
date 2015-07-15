using System.Net;
using TntBot.Loader;

namespace TntBot.WebCrawler
{
    internal class BaseWebCrawler
    {
        protected internal BaseWebCrawler(CookieContainer cookies)
        {
            Loader = new UrlDocumentLoader(cookies);
        }

        protected internal IDocumentLoader Loader { get; set; }
    }
}