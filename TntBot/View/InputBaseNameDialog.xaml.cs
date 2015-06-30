using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using TntBot.ViewModel;

namespace TntBot.View
{
    public partial class InputBaseNameDialog : Window
    {
        internal InputBaseNameViewModel Model { get { return DataContext as InputBaseNameViewModel; } }

        internal InputBaseNameDialog()
        {
            InitializeComponent();
            DataContext = new InputBaseNameViewModel(this);
        }

        private void InputBaseNameDialog_OnLoaded(object sender, RoutedEventArgs e)
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