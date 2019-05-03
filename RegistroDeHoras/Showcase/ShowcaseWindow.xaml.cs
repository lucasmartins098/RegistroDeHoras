using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Hardcodet.Wpf.TaskbarNotification;

namespace RegistroDeHorasDevOpsAzure
{
    /// <summary>
    /// Interaction logic for ShowcaseWindow.xaml
    /// </summary>
    public partial class ShowcaseWindow : Window
    {
        public ShowcaseWindow()
        {
            InitializeComponent();
            Loaded += delegate
            {
                //show balloon at startup
                var balloon = new TaskBarRegistroDeHoras();
                tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);
            };
        }
        /// <summary>
        /// Displays a balloon tip.
        /// </summary>
        private void showBalloonTip_Click(object sender, RoutedEventArgs e)
        {
            
                //just display the icon on the tray
                var icon = tb.Icon;
                tb.ShowBalloonTip("Tarefas", "Tarefa em andamento", icon);
        }

        private void hideBalloonTip_Click(object sender, RoutedEventArgs e)
        {
            tb.HideBalloonTip();
        }
        /// <summary>
        /// Resets the tooltip.
        /// </summary>
        private void removeToolTip_Click(object sender, RoutedEventArgs e)
        {
            tb.TrayToolTip = null;
        }
        private void showCustomBalloon_Click(object sender, RoutedEventArgs e)
        {
            TaskBarRegistroDeHoras balloon = new TaskBarRegistroDeHoras();
            //balloon.BalloonText = customBalloonTitle.Text;
            //show and close after 2.5 seconds
            tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);
        }
        private void hideCustomBalloon_Click(object sender, RoutedEventArgs e)
        {
            tb.CloseBalloon();
        }
        private void OnNavigationRequest(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            tb.Dispose();
            base.OnClosing(e);
        }
    }
}