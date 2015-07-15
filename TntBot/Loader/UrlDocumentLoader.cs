using HtmlAgilityPack;
using System;
using System.Net;
using TntBot.Helper;

namespace TntBot.Loader
{
    internal class UrlDocumentLoader : IDocumentLoader
    {
        public const string AppleUserAgent = "Apple-iPhone7C2/1202.466";

        protected internal UrlDocumentLoader(CookieContainer cookies)
        {
            if (cookies == null)
                throw new ArgumentNullException(@"cookies");

            Cookies = cookies;
        }

        public CookieContainer Cookies { get; private set; }

        public string UserAgent { get { return AppleUserAgent; } }

        public HtmlDocument LoadDocument(string url)
        {
            var document = new HtmlDocument();
            using (var client = new CookieAwareWebClient(Cookies))
            {
                client.Headers["User-Agent"] = UserAgent;
                string responseString = client.DownloadString(url);
                document.LoadHtml(responseString);
            }
            return document;
        }
    }
}