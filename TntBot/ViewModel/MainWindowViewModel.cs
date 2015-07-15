using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Ookii.Dialogs.Wpf;
using System.Diagnostics;
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

        public ICommand NotifyIssueCommand { get; private set; }

        public ICommand SaveCookiesCommand { get; private set; }

        public ICommand ShowInformationAboutAuthorCommand { get; private set; }

        public ICommand ShowInformationAboutSourceCommand { get; private set; }

        public ICommand ShowLicenseCommand { get; private set; }

        #endregion Commands

        public MainWindowViewModel(MainWindow view, string baseName)
        {
            View = view;

            var loginView = new WebBrowserView(this, baseName);

            LoadCookiesCommand = new DelegateCommand<object>(x => Cookies = CookieHelper.DeserializeCookies());
            SaveCookiesCommand = new DelegateCommand<object>(x => CookieHelper.SerializeCookies(loginView.ViewModel.GetCookies()));
            ShowInformationAboutAuthorCommand = new DelegateCommand(ShowInformationAboutAuthor);
            ShowLicenseCommand = new DelegateCommand(ShowLicense);
            NotifyIssueCommand = new DelegateCommand(NotifyIssue);
            ShowInformationAboutSourceCommand = new DelegateCommand(ShowInformationAboutSource);

            view.AddDocumentControl(loginView);
        }

        private void NotifyIssue()
        {
            try
            {
                Process.Start(@"https://github.com/deidigitalis/TntBot/issues/new");
            }
            catch
            {
                // ignored
            }
        }

        private void ShowInformationAboutAuthor()
        {
            try
            {
                Process.Start(@"https://github.com/deidigitalis");
            }
            catch
            {
                // ignored
            }
        }

        private void ShowInformationAboutSource()
        {
            try
            {
                Process.Start(@"http://deidigitalis.github.io/TntBot/");
            }
            catch
            {
                // ignored
            }
        }

        private void ShowLicense()
        {
            using (var dialog = new TaskDialog()
                {
                    WindowTitle = Properties.Resources.LicenseWindowTitle,
                    MainInstruction = Properties.Resources.LicenseMainInstruction,
                    Content = Properties.Resources.LicenseContent,
                    ExpandedInformation = Properties.Resources.License,
                    FooterIcon = TaskDialogIcon.Information,
                    EnableHyperlinks = true
                })
            {
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                dialog.ShowDialog(View);
            }
        }
    }
}