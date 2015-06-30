using System.Windows;
using TntBot.View;

namespace TntBot
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dlg = new InputBaseNameDialog();
            dlg.ShowDialog();

            if (dlg.DialogResult == true)
            {
                var mainWindow = new MainWindow(dlg.Model.Input);
                mainWindow.ShowDialog();
            }

            Shutdown(0);
        }
    }
}