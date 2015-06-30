using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TntBot.Infrastructure;
using TntBot.Properties;
using TntBot.View;
using TntBot.WebCrawler;

namespace TntBot.ViewModel
{
    public class ProgressViewModel : BindableBase
    {
        private readonly string baseName;
        private readonly CookieContainer cookies;
        private readonly string outputFolder;
        private ImageSource currentImage;
        private double progressPercentage;

        private string progressText;

        private BackgroundWorker worker;

        public ImageSource CurrentImage
        {
            get { return currentImage; }
            set { SetProperty(ref currentImage, value, "CurrentImage"); }
        }

        public double ProgressPercentage
        {
            get { return progressPercentage; }
            set { SetProperty(ref progressPercentage, value, "ProgressPercentage"); }
        }

        public string ProgressText
        {
            get { return progressText; }
            set
            {
                Debug.WriteLine(string.Format("Progress: {0}", value));
                SetProperty(ref progressText, value, "ProgressText");
            }
        }

        private MainWindowViewModel MainViewModel { get; set; }

        public ProgressViewModel(
            MainWindowViewModel mainViewModel,
            ProgressView view,
            CookieContainer cookies,
            string baseName,
            string outputFolder)
        {
            MainViewModel = mainViewModel;

            this.cookies = cookies;
            this.outputFolder = outputFolder;
            this.baseName = baseName;

            view.Loaded += (sender, args) =>
            {
                worker = new BackgroundWorker()
                {
                    WorkerReportsProgress = true
                };
                worker.DoWork += (s, e) => Download((BackgroundWorker)s, e);
                worker.ProgressChanged += (s, e) => ProgressPercentage = e.ProgressPercentage;
                worker.RunWorkerAsync();
            };
        }

        /// <summary>
        /// Downloads the photos
        /// </summary>
        private void Download(BackgroundWorker @backgroundWorker, DoWorkEventArgs @doWorkEventArgs)
        {
            try
            {
                List<Album> albums = LoadAlbumsFromProfile();

                LoadPhotosInAlbums(albums);

                DownloadPhotos(albums);

                MessageBox.Show("¡END!", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Downloads all the photos from the data stored in the albums
        /// </summary>
        private void DownloadPhotos(List<Album> albums)
        {
            var items = new List<Tuple<Album, Photo, int>>();
            int index = 0;

            // Make a flat collection of photos
            foreach (Album album in albums)
                foreach (Photo photo in album.Photos)
                    items.Add(new Tuple<Album, Photo, int>(album, photo, ++index));

            double count = items.Count;
            foreach (Tuple<Album, Photo, int> item in items)
            {
                ProgressText = string.Format(Resources.DownloadingPhotoMessage, item.Item1.Link.Name, item.Item2.Source);

                try
                {
                    var photoWebCrawler = new PhotoWebCrawler(cookies, outputFolder, item.Item1, item.Item2);
                    photoWebCrawler.PhotoSaving += DownloadPhotos_PhotoSaving;
                    photoWebCrawler.PhotoSaved += DownloadPhotos_PhotoSaved;
                    photoWebCrawler.Download();
                    Thread.Sleep(5000); // Avoid DoS
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(string.Format(Resources.ErrorDownloadingPhotoLog, item.Item1.Link.Name, item.Item2.Source, ex.Message));
                }

                int progress = (int)Math.Floor((100d * item.Item3) / count);
                worker.ReportProgress(progress);
            }

            MainViewModel.View.Dispatcher.Invoke(() => CurrentImage = null);

            ProgressText = Resources.DownloadingFinishedMessage;
        }

        /// <summary>
        /// Changes the current image
        /// </summary>
        private void DownloadPhotos_PhotoSaved(object sender, PhotoWebCrawler.PhotoEventArg e)
        {
            MainViewModel.View.Dispatcher.Invoke(() =>
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(e.FullPath, UriKind.Absolute);
                bitmapImage.EndInit();
                CurrentImage = bitmapImage;
            });
        }

        /// <summary>
        /// Changes the saving message
        /// </summary>
        private void DownloadPhotos_PhotoSaving(object sender, PhotoWebCrawler.PhotoEventArg e)
        {
            ProgressText = string.Format(Resources.SavingPhotoMessage, e.Photo.Link.Name, e.FullPath);
        }

        /// <summary>
        /// Gets all the albums from the profile
        /// </summary>
        private List<Album> LoadAlbumsFromProfile()
        {
            try
            {
                var profileWebCrawler = new ProfileWebCrawler(cookies, baseName);

                return profileWebCrawler.LoadAlbums();
            }
            catch (Exception ex)
            {
                ProgressText = string.Format(Resources.ErrorDownloadingAlbumsLog, ex.Message);
                SaveErrorMessage(ProgressText);

                return new List<Album>();
            }
        }

        /// <summary>
        /// Load all the data of the photos for each album
        /// </summary>
        private void LoadPhotosInAlbums(List<Album> albums)
        {
            foreach (Album album in albums)
            {
                var albumWebCrawler = new AlbumWebCrawler(cookies, album);
                albumWebCrawler.NextPageDetected += (sender, e) => ProgressText = string.Format(Resources.NextPageDetectedMessage, e.Album.Link.Name, e.Page);
                albumWebCrawler.LoadPhotos();
            }
        }

        /// <summary>
        /// Writes a line a error log. It is in the output folder.
        /// </summary>
        private void SaveErrorMessage(string message)
        {
            using (var error = new StreamWriter(Path.Combine(outputFolder, "error.log"), true))
            {
                error.WriteLine(message);
            }
        }
    }
}