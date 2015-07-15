using HtmlAgilityPack;
using System.Net;

namespace TntBot.Loader
{
    public interface IDocumentLoader
    {
        CookieContainer Cookies { get; }

        string UserAgent { get; }

        HtmlDocument LoadDocument(string url);
    }
}