using HtmlAgilityPack;
using System.Net;
using TntBot.Loader;

namespace TntBotTest.Mock
{
    internal class MockDocumentLoader : IDocumentLoader
    {
        private readonly string inputPath;

        public MockDocumentLoader(string inputPath)
        {
            this.inputPath = inputPath;
        }

        public CookieContainer Cookies { get { return null; } }

        public string UserAgent { get { return null; } }

        public HtmlDocument LoadDocument(string url)
        {
            var document = new HtmlDocument();
            document.Load(inputPath);
            return document;
        }
    }
}