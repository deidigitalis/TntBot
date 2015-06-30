using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Net;
using System.Windows.Input;
using TntBot.Helper;
using TntBot.View;

namespace TntBot.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public static CookieContainer Cookies { get; set; }

        public MainWindow View { get; private set; }

        #region Commands

        public ICommand LoadCookiesCommand { get; private set; }

        public ICommand SaveCookiesCommand { get; private set; }

        #endregion

        public MainWindowViewModel(MainWindow view, string baseName)
        {
            View = view;

            var loginView = new WebBrowserView(this, baseName);

            LoadCookiesCommand = new DelegateCommand<object>(x => Cookies = CookieHelper.DeserializeCookies());
            SaveCookiesCommand = new DelegateCommand<object>(x => CookieHelper.SerializeCookies(loginView.ViewModel.GetCookies()));

            view.AddDocumentControl(loginView);
        }
    }
}