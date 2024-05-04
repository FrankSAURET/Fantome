using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace Fantome
{
    public static class Fond
    {
        #region Déclaration de la dll user32 et de ses paramètres

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 1;
        public const int SPIF_SENDCHANGE = 2;

        [DllImport("user32.dll")]
        private static extern bool SetSysColors(int cElements, int[] lpaElements, int[] lpaRgbValues);

        #endregion Déclaration de la dll user32 et de ses paramètres

        /// <summary>
        /// Change le fond d'écran
        /// </summary>
        /// <param name="CheminCompletImage"></param>
        public static void ChangeImage(string CheminCompletImage)
        {
            int HauteurEcran = Screen.PrimaryScreen.Bounds.Height;
            int LargeurEcran = Screen.PrimaryScreen.Bounds.Width;
            //CheminCompletImage=@"E:\01 Images et dessins\Fond d'écran Frank\Test.jpg"
            if (File.Exists(CheminCompletImage))
            {
                Bitmap FondCourant = new Bitmap(CheminCompletImage);
                FondCourant.Save(Globs.CheminCompletFondCourant);

                if ((HauteurEcran != FondCourant.Height) || (LargeurEcran != FondCourant.Width))
                {
                    int XImage, YImage, LargeurImage = FondCourant.Width, HauteurImage = FondCourant.Height;

                    Bitmap new_image = new Bitmap(LargeurEcran, HauteurEcran);
                    Graphics g = Graphics.FromImage((Image)new_image);
                    if (FondCourant.Width > LargeurEcran)
                    {
                        LargeurImage = LargeurEcran;
                        HauteurImage = (FondCourant.Height * LargeurEcran) / FondCourant.Width;
                        if (HauteurImage > HauteurEcran)
                        {
                            HauteurImage = HauteurEcran;
                            LargeurImage = (FondCourant.Width * HauteurEcran) / FondCourant.Height;
                        }
                    }
                    if (FondCourant.Height > HauteurEcran)
                    {
                        HauteurImage = HauteurEcran;
                        LargeurImage = (FondCourant.Width * HauteurEcran) / FondCourant.Height;
                        if (LargeurImage > LargeurEcran)
                        {
                            LargeurImage = LargeurEcran;
                            HauteurImage = (FondCourant.Height * LargeurEcran) / FondCourant.Width;
                        }
                    }
                    if (FondCourant.Width < LargeurEcran && FondCourant.Height < HauteurEcran)
                    {
                        LargeurImage = LargeurEcran;
                        HauteurImage = (LargeurEcran * FondCourant.Height) / FondCourant.Width;
                        if (HauteurImage > HauteurEcran)
                        {
                            HauteurImage = HauteurEcran;
                            LargeurImage = (HauteurEcran * FondCourant.Width) / FondCourant.Height;
                        }
                    }
                    XImage = (LargeurEcran - LargeurImage) / 2;
                    YImage = (HauteurEcran - HauteurImage) / 2;
                
                    g.InterpolationMode = InterpolationMode.High;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.Clear(LireLaCouleur(CheminCompletImage));//Repeint l'image avec la couleur de fond
                    g.DrawImage(FondCourant, XImage, YImage, LargeurImage, HauteurImage);
                    
                    new_image.Save(Globs.CheminCompletFondCourant);

                    g.Dispose();
                    FondCourant.Dispose();
                    new_image.Dispose();
                }
                // Force un fond d'écran centré
                var Clef = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                Clef.SetValue(@"WallpaperStyle", 1.ToString());
                Clef.SetValue(@"TileWallpaper", 0.ToString());
                //Change le fond
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, Globs.CheminCompletFondCourant, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
        }

        /// <summary>
        /// Change le couleur du fond d'écran.
        /// Attention ça n'est pas immédiat.
        /// Cette fonction n'est plus utilisée
        /// </summary>
        /// <param name="CouleurFond"></param>
        public static void ChangeCouleur(int CouleurFond)
        {
            int[] element = new int[] { 1 };//1=Couleur du Fond
            int[] rgb = new int[] { CouleurFond };
            SetSysColors(1, element, rgb);
        }

        /// <summary>
        /// Passe le fond suivant en changeant aussi la couleur de fond pour si la taille du fond est inférieure à la taille de l'écran.
        /// Supprime le fond de la liste.
        /// </summary>
        public static void Suivant()
        {
            #region Mise à jour de la liste des fonds selon la date

            if (DateTime.Now > Globs.DateHeureProchaineMiseàJourDeLaListe) Lister();

            #endregion Mise à jour de la liste des fonds selon la date

            #region Mise à jour de la liste des fonds si plus de fond dans le fihier

            var NbLigne = File.ReadLines(Globs.CheminCompletListeDeFond).Count();
            if (NbLigne < 1)
            {
                Lister();
                NbLigne = File.ReadLines(Globs.CheminCompletListeDeFond).Count();
            }

            #endregion Mise à jour de la liste des fonds si plus de fond dans le fihier

            #region selection et changement du fond suivant

            // Si problème de temps, placer le générateur aléatoire dans la création de la liste et le stocker dans un fichier à part
            // @ devant "" pour que les \ soient échappés

            try
            {
                Random aleatoire = new Random();
                int NumDuFond = aleatoire.Next(NbLigne);
                List<string> FondSuivant = File.ReadAllLines(Globs.CheminCompletListeDeFond).ToList();
                ChangeImage(FondSuivant[NumDuFond]);
                //ChangeCouleur(LireLaCouleur(FondSuivant[NumDuFond]));
                Globs.FondCourant = FondSuivant[NumDuFond];
                FondSuivant.RemoveAt(NumDuFond);
                if (File.Exists(Globs.CheminCompletListeDeFond)) File.Delete(Globs.CheminCompletListeDeFond);
                File.WriteAllLines(Globs.CheminCompletListeDeFond, FondSuivant);
                Temps.CalculProchainChangement();
            }
            catch { }

            #endregion selection et changement du fond suivant
        }

        /// <summary>
        /// Stocke une liste des fonds dans le fichier "Liste de fond.txt" du répertoire courant
        /// Mets à jour la date de prochaine mise à jour de cette liste
        /// </summary>
        public static void Lister()
        {
            if (Globs.Lancé)
            {
                if (Directory.Exists(Globs.DossierFondDécranChemin))
                {
                    string[] ExtensionsImage = { ".jpg", ".jpeg", ".png" };
                    string[] txtFiles = Directory.EnumerateFiles(Globs.DossierFondDécranChemin, "*.*", Globs.DossierFondDécranListerLesSousDossiers).
                        Where(s => ExtensionsImage.Any(ext => ext == Path.GetExtension(s).ToLower())).ToArray();
                    string FichierListe = Environment.CurrentDirectory + "\\Liste de fond.txt";

                    if (File.Exists(FichierListe)) File.Delete(FichierListe);
                    File.WriteAllLines(FichierListe, txtFiles);

                    Temps.CalculProchaineMàJDeLaListe();

                    System.Windows.MessageBox.Show("La liste des fonds a été mise à jour.", "Info Fantôme !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show("Votre dossier de fond d'écran : « " 
                        + Globs.DossierFondDécranChemin 
                        + " » n'est pas valide. Vous devez choisir un dossier existant en cliquant sur le bouton « Choisir un dossier » !", "Info Fantôme !", MessageBoxButton.OK, MessageBoxImage.Warning);

                    
                        System.Windows.MessageBox.Show("Dossier courant : « " + Environment.CurrentDirectory + "\n"
                            + "Dossier fon dec : « " + Globs.DossierFondDécranChemin + "\n"
                            , "Info debug Fantôme !", MessageBoxButton.OK, MessageBoxImage.Warning);
                    
                        
                    Globs.PremièreFois = true;
                    Window FenConfiguration = new Configuration();
                    FenConfiguration.ShowDialog();
                    Globs.InfoFigé = "Figer ce fond";
                    Globs.IcôneDuSystray = @"\Images\Fantome.ico";
                }
            }
        }

        /// <summary>
        /// Lit la couleur des 4 angles et renvoie la couleur demandé (celle d'un des coins ou la moyenne)
        /// </summary>
        /// <param name="CheminCompletFichierFond"></param>
        /// <returns></returns>
        public static Color LireLaCouleur(string CheminCompletFichierFond)
        {
            //CheminCompletFichierFond = @"E:\01 Images et dessins\Fond d'écran Frank\Test.jpg";
            Bitmap ProchainFond = new Bitmap(CheminCompletFichierFond, true);

            Color CHG, CHD, CBD, CBG;
            Color CouleurDuPixel;

            CHD = ProchainFond.GetPixel(ProchainFond.Width - 1, 0);
            CBG = ProchainFond.GetPixel(0, ProchainFond.Height - 1);
            CBD = ProchainFond.GetPixel(ProchainFond.Width - 1, ProchainFond.Height - 1);
            CHG = ProchainFond.GetPixel(0, 0);

            switch (Globs.PointDeCouleur)
            {
                case "HD":
                    CouleurDuPixel = CHD;
                    break;

                case "HG":
                    CouleurDuPixel = CHG;
                    break;

                case "BD":
                    CouleurDuPixel = CBD;
                    break;

                case "BG":

                    CouleurDuPixel = CBG;
                    break;

                default:// Moyenne des 4 couleurs
                    CouleurDuPixel = Color.FromArgb((CHD.R + CHG.R + CBD.R + CBG.R) / 4, (CHD.G + CHG.G + CBD.G + CBG.G) / 4, (CHD.B + CHG.B + CBD.B + CBG.B) / 4);

                    break;
            }
            return CouleurDuPixel;
        }

        public static string InfoChgt()
        {
            if (!Globs.PremièreFois)
            {
                // Mise à jour des info dans la bulle et le menu
                string réfCoul;
                switch (Globs.PointDeCouleur)
                {
                    case "HD":
                        réfCoul = " Point en haut à droite.";
                        break;

                    case "HG":
                        réfCoul = " Point en haut à gauche.";
                        break;

                    case "BD":
                        réfCoul = " Point en bas à droite.";
                        break;

                    case "BG":

                        réfCoul = " Point en bas à gauche.";
                        break;

                    default:// Moyenne des 4 couleurs
                        réfCoul = " Moyenne des 4 angles.";
                        break;
                }

                string messageIntervalChangement;
                string messageProchainChangement = "";
                if (Globs.Figé) messageIntervalChangement = "Le fond est figé";
                else if (Globs.IntervalDeChangementUnité == "Seulement au démarrage") messageIntervalChangement = "Seulement au démarrage";
                else
                {
                    messageIntervalChangement = Globs.IntervalDeChangementDélais.ToString() + " " + Globs.IntervalDeChangementUnité.ToLower();
                    messageProchainChangement = Globs.DateHeureProchainChangement.ToString();
                }

                string messageProchaineMiseàJourDeLaListe;
                if (Globs.IntervalDeMiseàJourUnité == "Uniquement si la liste est vide")
                {
                    messageProchaineMiseàJourDeLaListe = "quand le fichier sera vide.";
                }
                else
                {
                    messageProchaineMiseàJourDeLaListe = Globs.DateHeureProchaineMiseàJourDeLaListe.ToString();
                }
                string messageMenuEtBulle =
                    "- Prochain changement : " + messageProchainChangement +
                    "\n- Interval de changement : " + messageIntervalChangement +
                    "\n- Référence de couleur :" + réfCoul +
                    "\n- Dernière réinitialisation de la liste des fonds : " + Globs.DateHeureDernièreMiseàJourDeLaListe.ToString() +
                    "\n- Prochaine réinitialisation de la liste des fonds : " + messageProchaineMiseàJourDeLaListe;

                return messageMenuEtBulle;
            }
            else return "";
        }
    }
}