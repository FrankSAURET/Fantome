using System;
using System.IO;

namespace Fantome
{
    internal static class Globs
    {
        public static string DossierFondDécranChemin;
        public static SearchOption DossierFondDécranListerLesSousDossiers;

        public static string FondNeutre;
        public static string FondCourant;

        public static int IntervalDeChangementDélais;
        public static string IntervalDeChangementUnité;
        public static bool Figé;

        public static string IcôneDuSystray;
        public static string InfoFigé;

        public static int IntervalDeMiseàJourDélais;
        public static string IntervalDeMiseàJourUnité;

        public static string PointDeCouleur;

        public static string EditeurChemin;
        public static string EditeurNom;

        public static DateTime DateHeureProchainChangement;
        public static DateTime DateHeureProchaineMiseàJourDeLaListe;
        public static DateTime DateHeureDernièreMiseàJourDeLaListe;

        public static bool PremièreFois;
        public static bool Lancé = false;
        public static bool FondNeutreAuDemarrage;

        public static string CheminCompletListeDeFond = Path.GetFullPath("Liste de fond.txt");
        public static string CheminCompletFondCourant = Path.GetFullPath("FondCourant.jpg");
        
        //public static
    }
}