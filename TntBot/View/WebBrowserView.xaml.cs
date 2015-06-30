using Microsoft.Practices.Prism.Mvvm;
using System.Windows.Controls;
using TntBot.ViewModel;

namespace TntBot.View
{
    public partial class WebBrowserView : UserControl, IView
    {
        internal WebBrowserViewModel ViewModel { get { return DataContext as WebBrowserViewModel; } }

        public WebBrowserView()
        {
            InitializeComponent();
            DataContext = new WebBrowserViewModel(this, null, "");
        }

        public WebBrowserView(MainWindowViewModel main, string baseName)
        {
            InitializeComponent();
            DataContext = new WebBrowserViewModel(this, main, baseName);
        }
    }
}