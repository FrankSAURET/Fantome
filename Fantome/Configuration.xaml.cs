using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Fantome
{
    /// <summary>
    /// Logique d'interaction pour Configuration.xaml
    /// </summary>
    ///

    public partial class Configuration : Window
    {
        public Configuration()
        {
            InitializeComponent();

            #region Actualisation de la fenêtre de configuration

            if (!Globs.PremièreFois)
            {
                textBoxChemin.Text = Globs.DossierFondDécranChemin;
                if (Globs.DossierFondDécranListerLesSousDossiers == SearchOption.AllDirectories) 
                    { checkBoxListerSousDossiers.IsChecked = true; }
                else 
                    { checkBoxListerSousDossiers.IsChecked = false; }
                textBoxFondNeutre.Text = Globs.FondNeutre;
                IntegerUpDownIntervalleChangement.Value = Globs.IntervalDeChangementDélais;
                comboBoxUniteIntervalChangement.Text = Globs.IntervalDeChangementUnité;
                IntegerUpDownIntervalleMaJ.Value = Globs.IntervalDeMiseàJourDélais;
                IntegerUpDownIntervalleMaJ.Text = Globs.IntervalDeMiseàJourUnité;
                labelEditeurNom.Content = "L'éditeur actuel est : " + Globs.EditeurNom;
                checkBoxFondNeutreAuDemarrage.IsChecked = Globs.FondNeutreAuDemarrage;
                // Change l'icône du bouton
                try
                {
                    iconButtonEditeur.Icon.Source = System.Drawing.Icon.ExtractAssociatedIcon(Globs.EditeurChemin).ToImageSource();
                }
                catch
                {
                }
                // Change le point de couleur
                switch (Globs.PointDeCouleur)
                {
                    case "Moy":
                        Moy.IsChecked = true;
                        break;

                    case "HD":
                        HD.IsChecked = true;
                        break;

                    case "HG":
                        HG.IsChecked = true;
                        break;

                    case "BD":
                        BD.IsChecked = true;
                        break;

                    case "BG":
                        BG.IsChecked = true;
                        break;
                }
            }

            #endregion Actualisation de la fenêtre de configuration
        }

        private void iconButtonChanger_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description = "Choisissez le dossier contenant vos fond d'écrans";
            folderBrowserDialog1.ShowNewFolderButton = false;
            System.Windows.Forms.DialogResult result = folderBrowserDialog1.ShowDialog();
            textBoxChemin.Text = folderBrowserDialog1.SelectedPath;
            Globs.DossierFondDécranChemin = textBoxChemin.Text;
        }

        private void buttonMiseAJourListe_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(Globs.DossierFondDécranChemin))
            {
                System.Windows.MessageBox.Show("Vous devez choisir un dossier existant en cliquant sur le bouton « Choisir un dossier » !", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Fond.Lister();
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!File.Exists("configuration.xml"))
            {
                MessageBoxResult RéponseBoite = System.Windows.MessageBox.Show("Si vous quittez maintenant, Fantôme ne fonctionnera pas. Voulez-vous vraiment quitter ?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (RéponseBoite == MessageBoxResult.Yes) Close();
            }
            else
            {
                Close();
            }
        }

        private void iconButtonEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            Globs.DossierFondDécranChemin = textBoxChemin.Text;
            Globs.FondNeutre = textBoxFondNeutre.Text;
            Globs.IntervalDeChangementDélais = (int)IntegerUpDownIntervalleChangement.Value;
            Globs.IntervalDeChangementUnité = comboBoxUniteIntervalChangement.Text;

            Globs.IntervalDeMiseàJourDélais = (int)IntegerUpDownIntervalleMaJ.Value;
            Globs.IntervalDeMiseàJourUnité = comboBoxUniteIntervalMaJ.Text;

            Globs.EditeurNom = labelEditeurNom.Content.ToString();
            int pos = Globs.EditeurNom.IndexOf(": ");
            Globs.EditeurNom = Globs.EditeurNom.Substring(pos + 2);

            //En même temps ça sérialise configuration.xml
            Temps.CalculProchainChangement();
            if (!File.Exists(Globs.CheminCompletListeDeFond)) Temps.CalculProchaineMàJDeLaListe();

            if (Globs.PremièreFois) Fond.Lister();
            Globs.PremièreFois = false;
            Close();
        }

        #region Actualisation de checkbox point de couleur

        private void HD_Checked(object sender, RoutedEventArgs e)
        {
            Globs.PointDeCouleur = "HD";
        }

        private void HG_Checked(object sender, RoutedEventArgs e)
        {
            Globs.PointDeCouleur = "HG";
        }

        private void BD_Checked(object sender, RoutedEventArgs e)
        {
            Globs.PointDeCouleur = "BD";
        }

        private void BG_Checked(object sender, RoutedEventArgs e)
        {
            Globs.PointDeCouleur = "BG";
        }

        private void Moy_Checked(object sender, RoutedEventArgs e)
        {
            Globs.PointDeCouleur = "Moy";
        }

        #endregion Actualisation de checkbox point de couleur

        private void iconButtonEditeur_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Exécutables (*.com;*.exe;*.bat)|*.com;*.exe;*.bat";
            dlg.Title = "Choisissez un éditeur pour vos images à partir de l'application";
            dlg.InitialDirectory = @"C:\Program Files\";

            if (dlg.ShowDialog() == true)
            {
                Globs.EditeurChemin = dlg.FileName;
            }
            // Change l'icône du bouton
            iconButtonEditeur.Icon.Source = System.Drawing.Icon.ExtractAssociatedIcon(Globs.EditeurChemin).ToImageSource();

            try
            {
                var infos = Fichier.GetVersionInfo(Globs.EditeurChemin, "ProductName");
                labelEditeurNom.Content = "L'éditeur actuel est : " + infos.Single(x => x.Item1 == "ProductName").Item2;
            }
            catch
            {
            }
        }

        private void iconButtonChangerFondNeutre_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.InitialDirectory = Globs.DossierFondDécranChemin;
            dlg.Filter = "Images (*.gif;*.jpeg;*.jpg;*.png;*.bmp)|*.gif;*.jpeg;*.jpg;*.png;*.bmp|Tous (*.*)|*.*";
            dlg.Title = "Choisissez un fond d'écran neutre";
            dlg.FilterIndex = 1;

            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                Globs.FondNeutre = dlg.FileName;
                textBoxFondNeutre.Text = dlg.FileName;
            }
        }

        private void checkBoxListerSousDossiers_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxListerSousDossiers.IsChecked == true)
            {
                Globs.DossierFondDécranListerLesSousDossiers = System.IO.SearchOption.AllDirectories;
            }
            else
            {
                Globs.DossierFondDécranListerLesSousDossiers = System.IO.SearchOption.TopDirectoryOnly;
            }
        }

        private void checkBoxFondNeutreAuDemarrage_Click(object sender, RoutedEventArgs e)
        {
            Globs.FondNeutreAuDemarrage = checkBoxFondNeutreAuDemarrage.IsChecked ?? false;
        }
    }
}