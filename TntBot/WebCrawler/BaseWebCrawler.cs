using HtmlAgilityPack;
using System.Net;
using TntBot.Helper;

namespace TntBot.WebCrawler
{
    internal class BaseWebCrawler
    {
        internal const string Ip6 = "Apple-iPhone7C2/1202.466";

        protected internal CookieContainer Cookies { get; private set; }

        protected internal BaseWebCrawler(CookieContainer cookies)
        {
            Cookies = cookies;
        }

        protected internal HtmlDocument LoadDocument(string url)
        {
            var document = new HtmlDocument();
            using (var client = new CookieAwareWebClient(Cookies))
            {
                client.Headers["User-Agent"] = Ip6;
                string responseString = client.DownloadString(url);
                document.LoadHtml(responseString);
            }
            return document;
        }
    }
}