using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using TBot.Infrastructure;

namespace TntBot.Infrastructure
{
    public class Album
    {
        public string CollectionKey { get; private set; }

        public Link Link { get; private set; }

        public List<Photo> Photos { get; set; }

        public Album(Link link)
        {
            this.Link = link;

            NameValueCollection query = HttpUtility.ParseQueryString(link.Href);
            CollectionKey = query.Get("amp;collection_key");
        }

        public string GetUrl(int page)
        {
            //https://m.tuenti.com/?m=Albums&func=index&collection_key=17-62396120-836547415-60892029-1341244606&stats=%7B%22m%22%3A%22MobilePhotoNavigation%22%2C%22a%22%3A%22o%22%2C%22s%22%3A206%7D
            //https://m.tuenti.com/?m=Albums&func=view_album_display&collection_key=3-62396120-5118207&photos_page=9

            return string.Format("https://m.tuenti.com/?m=Albums&func=index&collection_key={0}&photos_page={1}", CollectionKey, page);
        }
    }
}