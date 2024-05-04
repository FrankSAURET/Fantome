using Hardcodet.Wpf.TaskbarNotification;

using System;
using System.IO;
using System.Windows;
using System.Xml;

namespace Fantome
{
    /// <summary>
    /// Démarrage avec création de l'icône de systray
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Globs.Lancé = true;
            // Lancement de config si pas existant
            if (!File.Exists("configuration.xml"))
            {
                Globs.PremièreFois = true;
                Window FenConfiguration = new Configuration();
                FenConfiguration.ShowDialog();
                Globs.InfoFigé = "Figer ce fond";
                Globs.IcôneDuSystray = @"\Images\Fantome.ico";
            }

            #region Récupération des informations du fichier XML

            try
            {
                using (XmlReader reader = XmlReader.Create("configuration.xml"))
                {
                    reader.ReadToFollowing("DossierFondDécran");
                    Globs.DossierFondDécranChemin = reader.GetAttribute("Chemin");

                    if (reader.GetAttribute("ListerLesSousDossiers") == "AllDirectories") { Globs.DossierFondDécranListerLesSousDossiers = SearchOption.AllDirectories; }
                    else { Globs.DossierFondDécranListerLesSousDossiers = SearchOption.TopDirectoryOnly; }

                    reader.ReadToFollowing("FondNeutre");
                    Globs.FondNeutre = reader.ReadElementContentAsString("FondNeutre", "");

                    reader.ReadToFollowing("FondCourant");
                    Globs.FondCourant = reader.ReadElementContentAsString("FondCourant", "");

                    reader.ReadToFollowing("IntervalDeChangement");
                    Globs.IntervalDeChangementDélais = Convert.ToInt32(reader.GetAttribute("Délais"));
                    Globs.IntervalDeChangementUnité = reader.GetAttribute("Unité");
                    Globs.Figé = Convert.ToBoolean(reader.GetAttribute("Figé"));
                    if (Globs.Figé)
                    {
                        Globs.IcôneDuSystray = @"\Images\FantomeInactif.ico";
                        Globs.InfoFigé = "Activer le changement automatique";
                    }
                    else
                    {
                        Globs.IcôneDuSystray = @"\Images\Fantome.ico";
                        Globs.InfoFigé = "Figer ce fond";
                    }

                    reader.ReadToFollowing("IntervalDeMiseàJour");
                    Globs.IntervalDeMiseàJourDélais = Convert.ToInt32(reader.GetAttribute("Délais"));
                    Globs.IntervalDeMiseàJourUnité = reader.GetAttribute("Unité");

                    reader.ReadToFollowing("PointDeCouleur");
                    Globs.PointDeCouleur = reader.ReadElementContentAsString("PointDeCouleur", "");

                    reader.ReadToFollowing("Editeur");
                    Globs.EditeurNom = reader.GetAttribute("Nom");
                    Globs.EditeurChemin = reader.GetAttribute("Chemin");

                    reader.ReadToFollowing("DateHeureProchainChangement");
                    Globs.DateHeureProchainChangement = Convert.ToDateTime(reader.ReadElementContentAsString("DateHeureProchainChangement", ""));

                    reader.ReadToFollowing("DateHeureProchaineMiseàJourDeLaListe");
                    Globs.DateHeureProchaineMiseàJourDeLaListe = Convert.ToDateTime(reader.ReadElementContentAsString("DateHeureProchaineMiseàJourDeLaListe", ""));
                    
                    reader.ReadToFollowing("FondNeutreAuDemarrage");
                    Globs.FondNeutreAuDemarrage = reader.ReadElementContentAsBoolean("FondNeutreAuDemarrage","");
                }
            }
            catch 
            {
                Console.WriteLine($"Processing failed: ");
                if (File.Exists("configuration.xml"))
                {
                    File.Delete("configuration.xml");
                    Temps.CalculProchainChangement();
                    System.Windows.MessageBox.Show("Le fichier de configuration était invalide. Je l'ai supprimé et j'en ai recréé un avec les informations que j'avais pu récupérer avant cette erreur.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    System.Windows.MessageBox.Show("Une erreur non gérée vient de se produire. Tentez de réinstaller le programme.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            #endregion Récupération des informations du fichier XML

            #region Changement de fond et mise à jour de la liste au démarrage le cas échéant

            if (((!Globs.Figé) && (Globs.IntervalDeChangementUnité != "Jours") && (!Globs.FondNeutreAuDemarrage)) ||
            ((!Globs.Figé) && (Globs.IntervalDeChangementUnité == "Jours") && (Globs.DateHeureProchainChangement > DateTime.Now) && (!Globs.FondNeutreAuDemarrage)))
            {
                Fond.Suivant();
            }
            else
            {
                try
                {
                    Fond.ChangeImage(Globs.FondNeutre);
                    Globs.FondCourant = Globs.FondNeutre;
                    Temps.CalculProchainChangement();
                }
                catch { }
            }
            if (Globs.DateHeureProchaineMiseàJourDeLaListe < DateTime.Now) Fond.Lister();

            #endregion Changement de fond et mise à jour de la liste au démarrage le cas échéant

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }
    }
}