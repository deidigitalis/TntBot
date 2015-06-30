using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using TntBot.ViewModel;

namespace TntBot.View
{
    public partial class MainWindow : Window
    {
        public MainWindow(string baseName)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            DataContext = new MainWindowViewModel(this, baseName);
        }

        public void AddDocumentControl(UIElement element)
        {
            MainGrid.Children.Add(element);
        }

        public void ClearDocumentControl()
        {
            MainGrid.Children.Clear();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch
            {
                // ignored
            }

            e.Handled = true;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            MouseDown += (s0, e0) =>
            {
                try
                {
                    if (e0.ChangedButton == MouseButton.Left)
                    {
                        Debug.WriteLine("Dragging...");
                        DragMove();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            };
        }
    }
}