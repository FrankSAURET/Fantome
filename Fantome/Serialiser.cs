using System;
using System.IO;
using System.Xml.Serialization;

namespace Fantome
{
    [Serializable()]
    [XmlRoot("ConfigurationFantôme")] // Nom du noeud racine
    public class Serialiser
    {
        public DossierFondDec DossierFondDécran = new DossierFondDec();
        public string FondNeutre;
        public string FondCourant;
        public IntervalChangement IntervalDeChangement = new IntervalChangement();
        public IntervalMàJ IntervalDeMiseàJour = new IntervalMàJ();
        public string PointDeCouleur;
        public Edition Editeur = new Edition();
        public DateTime DateHeureProchainChangement;
        public DateTime DateHeureProchaineMiseàJourDeLaListe;
        public DateTime DateHeureDernièreMiseàJourDeLaListe;
        public bool FondNeutreAuDemarrage;

        public class DossierFondDec
        {
            [XmlAttribute]
            public string Chemin;

            [XmlAttribute]
            public SearchOption ListerLesSousDossiers;
        }

        public class IntervalChangement
        {
            [XmlAttribute]
            public int Délais;

            [XmlAttribute]
            public string Unité;

            [XmlAttribute]
            public bool Figé;
        }

        public class IntervalMàJ
        {
            [XmlAttribute]
            public int Délais;

            [XmlAttribute]
            public string Unité;
        }

        public class Edition
        {
            [XmlAttribute]
            public string Chemin;

            [XmlAttribute]
            public string Nom;
        }

        // Un constructeur sans paramètres est indispensable pour la sérialisation XML
        public Serialiser()
        {
            DossierFondDécran.Chemin = Globs.DossierFondDécranChemin;
            FondNeutre = Globs.FondNeutre;
            FondCourant = Globs.FondCourant;
            FondNeutreAuDemarrage = Globs.FondNeutreAuDemarrage;
            DossierFondDécran.ListerLesSousDossiers = Globs.DossierFondDécranListerLesSousDossiers;
            IntervalDeChangement.Délais = Globs.IntervalDeChangementDélais;
            IntervalDeChangement.Unité = Globs.IntervalDeChangementUnité;
            IntervalDeChangement.Figé = Globs.Figé;
            IntervalDeMiseàJour.Délais = Globs.IntervalDeMiseàJourDélais;
            IntervalDeMiseàJour.Unité = Globs.IntervalDeMiseàJourUnité;
            PointDeCouleur = Globs.PointDeCouleur;
            Editeur.Chemin = Globs.EditeurChemin;
            Editeur.Nom = Globs.EditeurNom;
            DateHeureProchainChangement = Globs.DateHeureProchainChangement;
            DateHeureProchaineMiseàJourDeLaListe = Globs.DateHeureProchaineMiseàJourDeLaListe;
            DateHeureDernièreMiseàJourDeLaListe = Globs.DateHeureDernièreMiseàJourDeLaListe;
        }
    }
}