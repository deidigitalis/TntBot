using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TBot.Infrastructure;
using TntBot.Infrastructure;

namespace TntBot.WebCrawler
{
    internal class ProfileWebCrawler : BaseWebCrawler
    {
        private readonly string baseName;

        internal ProfileWebCrawler(CookieContainer cookies, string baseName)
            : base(cookies)
        {
            this.baseName = baseName;
        }

        internal List<Album> LoadAlbums()
        {
            string url = string.Format("https://m.{0}.com/?m=Profile&func=my_profile", baseName);

            HtmlDocument document = LoadDocument(url);

            List<Link> links = new List<Link>();
            HtmlNodeCollection htmlNodeCollection = document.DocumentNode.SelectNodes("//a");
            foreach (HtmlNode htmlNode in htmlNodeCollection)
            {
                string innerText = htmlNode.InnerText;
                HtmlAttributeCollection attributeCollection = htmlNode.Attributes;

                if (attributeCollection.Contains("href"))
                {
                    string href = attributeCollection["href"].Value;
                    links.Add(new Link(innerText, href));
                }
            }

            return links.Where(x => x.Href.Contains("m=Albums")).Select(x => new Album(x)).ToList();
        }
    }
}