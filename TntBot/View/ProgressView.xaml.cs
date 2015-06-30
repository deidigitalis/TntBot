using System.Net;
using System.Windows.Controls;
using TntBot.ViewModel;

namespace TntBot.View
{
    public partial class ProgressView : UserControl
    {
        public ProgressViewModel ViewModel { get { return DataContext as ProgressViewModel; } }

        public ProgressView(MainWindowViewModel mainWindowViewModel,
            CookieContainer cookies,
            string baseName,
            string outputFolder)
        {
            InitializeComponent();
            DataContext = new ProgressViewModel(mainWindowViewModel, this, cookies, baseName, outputFolder);
        }
    }
}