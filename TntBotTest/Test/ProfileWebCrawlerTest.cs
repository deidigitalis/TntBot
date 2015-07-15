using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using TntBot.Infrastructure;
using TntBot.WebCrawler;
using TntBotTest.Mock;

namespace TntBotTest.Test
{
    [TestClass]
    public class ProfileWebCrawlerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        [DeploymentItem(@"Data\example_profile.html", "Data")]
        public void TestMethod1()
        {
            string inputPath = string.Format(@"{0}\Data\example_profile.html", TestContext.DeploymentDirectory);

            var crawler = new ProfileWebCrawler(new CookieContainer(), "MockSocialNetwork");
            crawler.Loader = new MockDocumentLoader(inputPath);

            List<Album> albums = crawler.LoadAlbums();

            Assert.AreEqual(25, albums.Count);

            string[,] expected = new[,] {
                { "1-00000000-000000000-00000000-0000000000", "Fotos etiquetadas" },
                { "2-00000000-000000000-00000000-0000000000", "Subidas por mí"},
                { "17-00000000-000000000-00000000-0000000000", "Fotos de perfil"},
                { "20-00000000-000000000-00000000-0000000000", "Fotos que me gustan"},
                { "3-00000000-0000000-000000000-00000000-0000000000", "ALBUM 001"},
                { "3-00000000-0000000-000000000-00000000-0000000000", "ALBUM 002"},
                { "3-00000000-0000000-000000000-00000000-0000000000", "ALBUM 003"}
            };

            for (int i = 0, count = expected.GetLength(0); i < count; i++)
            {
                Assert.IsNotNull(albums[i]);
                Assert.AreEqual(expected[i, 0], albums[i].CollectionKey);
                Assert.IsTrue(albums[i].Link.Href.Contains("https://m.SocialNetwork.com/?m=Albums"));
                Assert.AreEqual(expected[i, 1], albums[i].Link.Name);
            }
        }
    }
}