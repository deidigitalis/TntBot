using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TBot.Infrastructure;
using TntBot.Infrastructure;

namespace TntBot.WebCrawler
{
    internal class AlbumWebCrawler : BaseWebCrawler
    {
        private const string NextPageText = "Página siguiente";

        private readonly Album album;

        internal event EventHandler<NextPageEventArg> NextPageDetected;

        internal AlbumWebCrawler(CookieContainer cookies, Album album)
            : base(cookies)
        {
            this.album = album;
        }

        internal void LoadPhotos()
        {
            int page = 0;
            Link nextNode = null;

            album.Photos = new List<Photo>();

            do
            {
                string url = album.GetUrl(page);
                HtmlDocument document = LoadDocument(url);

                List<Photo> photos = document.DocumentNode.SelectNodes("//a")
                    .Where(x => x.Attributes["href"].Value.Contains("m=Photos"))
                    .Select(x => new Link(x.InnerText, x.Attributes["href"].Value))
                    .Select(x => new Photo(x))
                    .ToList();

                album.Photos.AddRange(photos);

                nextNode = document.DocumentNode.SelectNodes("//a")
                    .Select(x => new Link(x.InnerText, string.Empty))
                    .FirstOrDefault(x => x.Name == NextPageText);

                if (nextNode != null)
                    page++;

                if (page >= 500)
                    throw new ApplicationException("Warning page 500!!!");

                if (NextPageDetected != null)
                    NextPageDetected(this, new NextPageEventArg(album, page));
            } while (nextNode != null);
        }

        internal class NextPageEventArg : EventArgs
        {
            internal Album Album { get; private set; }

            internal int Page { get; private set; }

            internal NextPageEventArg(Album album, int page)
            {
                this.Album = album;
                this.Page = page;
            }
        }
    }
}