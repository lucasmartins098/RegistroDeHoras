using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows;


namespace RegistroDeHorasDevOpsAzure
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            ShowcaseWindow showcaseWindow = new ShowcaseWindow();
            showcaseWindow.InitializeComponent();
        }

        /// <summary>
        /// Sets <see cref="Window.WindowStartupLocation"/> and
        /// <see cref="Window.Owner"/> properties of a dialog that
        /// is about to be displayed.
        /// </summary>
        /// <param name="window">The processed window.</param>
        private void ShowDialog(Window window)
        {
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
        private void btnMainSample_Click(object sender, RoutedEventArgs e)
        {
            var sampleWindow = new ShowcaseWindow();
            sampleWindow.Owner = this;
            sampleWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            sampleWindow.ShowDialog();
        }
        private void OnNavigationRequest(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
    }
}