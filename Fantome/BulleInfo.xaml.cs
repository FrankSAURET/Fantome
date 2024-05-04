using Hardcodet.Wpf.TaskbarNotification;

using System.Windows;
using System.Windows.Controls;

namespace Fantome
{
    /// <summary>
    /// Logique d'interaction pour BulleInfo.xaml
    /// </summary>
    public partial class BulleInfo : UserControl
    {
        public BulleInfo()
        {
            InitializeComponent();
            TaskbarIcon.AddToolTipOpenedHandler(this, OnToolTipOpening);
        }

        private void OnToolTipOpening(object sender, RoutedEventArgs e)
        {
            TextBlockInfo.Text = Fond.InfoChgt();
        }
    }
}