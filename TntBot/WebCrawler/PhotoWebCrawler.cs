using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using TntBot.Helper;
using TntBot.Infrastructure;

namespace TntBot.WebCrawler
{
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    internal class PhotoWebCrawler : BaseWebCrawler
    {
        private readonly Album album;

        private readonly Dictionary<string, Tuple<string, string>> downloaded;
        private readonly string outputFolder;
        private readonly Photo photo;

        internal event EventHandler<PhotoEventArg> PhotoSaved;

        internal event EventHandler<PhotoEventArg> PhotoSaving;

        internal PhotoWebCrawler(CookieContainer cookies, string outputFolder, Album album, Photo photo)
            : base(cookies)
        {
            this.outputFolder = outputFolder;
            downloaded = new Dictionary<string, Tuple<string, string>>();
            this.album = album;
            this.photo = photo;
        }

        internal void Download()
        {
            string url = photo.GetUrl();
            HtmlDocument document = Loader.LoadDocument(url);

            DateTime creationTime;

            try
            {
                creationTime = GetCreationTime(document);
            }
            catch (Exception)
            {
                creationTime = DateTime.Now;
            }

            HtmlNodeCollection htmlNodeCollection = document.DocumentNode.SelectNodes("//div");

            foreach (HtmlNode htmlNode in htmlNodeCollection)
            {
                HtmlAttributeCollection attributeCollection = htmlNode.Attributes;

                if (attributeCollection.Contains("class") && (attributeCollection["class"].Value == "full_size_photo"))
                {
                    HtmlNode linkNode = htmlNode.ChildNodes.FirstOrDefault();

                    if (linkNode != null)
                    {
                        HtmlNode image = linkNode.ChildNodes[0];
                        photo.Source = image.Attributes["src"].Value;

                        using (var client = new CookieAwareWebClient(Loader.Cookies))
                        {
                            client.Headers["User-Agent"] = Loader.UserAgent;

                            string name = photo.Link.Name;

                            if (string.IsNullOrWhiteSpace(name))
                            {
                                name = photo.Source.Substring(photo.Source.LastIndexOf('/') + 1);
                            }

                            if (string.IsNullOrWhiteSpace(name))
                                name = string.Format("photo_{0}", DateTime.Now.Ticks);

                            var fileInfo = new FileInfo(name);
                            if (fileInfo.Extension != ".jpg")
                                name = name + ".jpg";

                            string directoryPath = Path.Combine(outputFolder, album.Link.Name);
                            if (!Directory.Exists(directoryPath))
                                Directory.CreateDirectory(directoryPath);

                            string fullPath = Path.Combine(directoryPath, name);

                            OnPhotoSaving(photo, fullPath);

                            if (!downloaded.ContainsKey(photo.Source))
                            {
                                client.DownloadFile(photo.Source, fullPath);
                                downloaded.Add(photo.Source, new Tuple<string, string>(name, fullPath));
                            }
                            else
                            {
                                Tuple<string, string> copyInfo = downloaded[photo.Source];
                                fullPath = Path.Combine(directoryPath, copyInfo.Item1);
                                File.Copy(copyInfo.Item2, fullPath);
                            }

                            File.SetCreationTime(fullPath, creationTime);

                            OnPhotoSaved(photo, fullPath);
                        }

                        break;
                    }
                }
            }
        }

        private static DateTime GetCreationTime(HtmlDocument document)
        {
            DateTime creationTime = DateTime.Now;

            HtmlNodeCollection htmlNcTime = document.DocumentNode.SelectNodes("//span");
            foreach (HtmlNode htmlNode in htmlNcTime)
            {
                HtmlAttributeCollection attributeCollection = htmlNode.Attributes;

                if (attributeCollection.Contains("class") && (attributeCollection["class"].Value == "time"))
                {
                    string time = htmlNode.InnerText;

                    var spaceIndex = time.Select((c, i) => new { Ch = c, Index = i })
                        .Where(x => x.Ch == ' ')
                        .ToList();

                    time.Substring(0, spaceIndex[4].Index);

                    string[] timeSections = time.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int day = int.Parse(timeSections[0]);

                    int month;
                    switch (timeSections[1])
                    {
                        case "Ene":
                            month = 1;
                            break;

                        case "Feb":
                            month = 2;
                            break;

                        case "Mar":
                            month = 3;
                            break;

                        case "Abr":
                            month = 4;
                            break;

                        case "May":
                            month = 5;
                            break;

                        case "Jun":
                            month = 6;
                            break;

                        case "Jul":
                            month = 7;
                            break;

                        case "Ago":
                            month = 8;
                            break;

                        case "Sep":
                            month = 9;
                            break;

                        case "Oct":
                            month = 10;
                            break;

                        case "Nov":
                            month = 11;
                            break;

                        case "Dic":
                            month = 12;
                            break;

                        default:
                            month = 1;
                            break;
                    }

                    int year = int.Parse(timeSections[2].Substring(0, 4));

                    string[] hourSection = timeSections[3].Split(':');

                    int hour = int.Parse(hourSection[0]);
                    int minute = int.Parse(hourSection[1]);

                    creationTime = new DateTime(year, month, day, hour, minute, 0);

                    break;
                }
            }
            return creationTime;
        }

        private void OnPhotoSaved(Photo ph, string fullPath)
        {
            if (ph == null)
                throw new ArgumentNullException("ph");

            if (PhotoSaved != null)
                PhotoSaved(this, new PhotoEventArg(ph, fullPath));
        }

        private void OnPhotoSaving(Photo ph, string fullPath)
        {
            if (ph == null)
                throw new ArgumentNullException("ph");

            if (PhotoSaving != null)
                PhotoSaving(this, new PhotoEventArg(ph, fullPath));
        }

        internal class PhotoEventArg : EventArgs
        {
            internal string FullPath { get; private set; }

            internal Photo Photo { get; private set; }

            internal PhotoEventArg(Photo photo, string fullPath)
            {
                this.Photo = photo;
                this.FullPath = fullPath;
            }
        }
    }
}