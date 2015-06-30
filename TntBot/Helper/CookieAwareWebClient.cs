using System;
using System.Net;

namespace TntBot.Helper
{
    public class CookieAwareWebClient : WebClient
    {
        public readonly CookieContainer Cookies;

        public CookieAwareWebClient()
        {
            Cookies = new CookieContainer();
        }

        public CookieAwareWebClient(CookieContainer cookies)
        {
            Cookies = cookies;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            HttpWebRequest webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = Cookies;
            }
            return request;
        }
    }
}