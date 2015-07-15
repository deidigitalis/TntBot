using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Net;
using System.Windows.Input;
using System.Windows.Navigation;
using TntBot.Helper;
using TntBot.Loader;
using TntBot.Properties;
using TntBot.View;

namespace TntBot.ViewModel
{
    internal class WebBrowserViewModel : BindableBase
    {
        private readonly WebBrowserView view;
        private string loginAddress;
        private string navigationUrl;

        public ICommand DownloadCommand { get; private set; }

        public string NavigationUrl
        {
            get { return navigationUrl; }
            private set { SetProperty(ref navigationUrl, value, "NavigationUrl"); }
        }

        internal WebBrowserViewModel(WebBrowserView view, MainWindowViewModel main, string baseName)
        {
            this.view = view;

            loginAddress = string.Format("http://m.{0}.com", baseName);

            view.Browser.Navigating += Browser_Navigating;
            view.Loaded += view_Loaded;

            DownloadCommand = new DelegateCommand<object>(x => Download(main, baseName));
        }

        internal CookieContainer GetCookies()
        {
            Uri url = view.Browser.Source;
            return CookieHelper.GetUriCookieContainer(url);
        }

        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            NavigationUrl = e.Uri.OriginalString;
        }

        private void Download(MainWindowViewModel main, string baseName)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog()
            {
                UseDescriptionForTitle = false,
                Description = Resources.Download_OutputFolder,
                ShowNewFolderButton = true,
            };

            if (dialog.ShowDialog(main.View) == true)
            {
                if (MainWindowViewModel.Cookies == null)
                    MainWindowViewModel.Cookies = GetCookies();

                main.View.ClearDocumentControl();
                main.View.AddDocumentControl(new ProgressView(main, MainWindowViewModel.Cookies, baseName, dialog.SelectedPath));
            }
        }

        private void view_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            string additionalHeader = string.Format(@"User-Agent: {0}", UrlDocumentLoader.AppleUserAgent);
            view.Browser.Navigate(loginAddress, null, null, null);
        }
    }
}