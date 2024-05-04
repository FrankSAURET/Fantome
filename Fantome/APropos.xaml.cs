using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Fantome
{
    /// <summary>
    /// Logique d'interaction pour APropos.xaml
    /// </summary>
    public partial class APropos : Window
    {
        public APropos()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            textBoxDescription.Text = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            textBoxVersion2.Text = "Version " + assembly.GetName().Version.ToString() + " Build " + assembly.GetName().Version.Build.ToString();
            textBoxTitre.Text = assembly.GetName().Name;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void buttonLien_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.electropol.fr");
        }
    }
}