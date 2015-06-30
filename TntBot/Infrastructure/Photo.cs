using System.Collections.Specialized;
using System.Web;
using TBot.Infrastructure;

namespace TntBot.Infrastructure
{
    public class Photo
    {
        public string CollectionKey { get; private set; }

        public Link Link { get; set; }

        public string Source { get; set; }

        public Photo(Link link)
        {
            this.Link = link;

            NameValueCollection query = HttpUtility.ParseQueryString(link.Href);
            CollectionKey = query.Get("amp;collection_key");
        }

        public string GetUrl()
        {
            //https://m.tuenti.com/?m=Photos&func=view_album_photo&collection_key=17-62396120-836547415-60892029-1341244606&stats=%7B%22m%22%3A%22MobilePhotoNavigation%22%2C%22a%22%3A%22o%22%2C%22s%22%3A206%7D

            return string.Format("https://m.tuenti.com/?m=Photos&func=view_album_photo&collection_key={0}", CollectionKey);
        }
    }
}