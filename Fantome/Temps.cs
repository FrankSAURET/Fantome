using System;
using System.IO;
using System.Xml.Serialization;

namespace Fantome
{
    public static class Temps
    {
        public static void CalculProchainChangement()
        {
            if (!Globs.Figé)
            {
                switch (Globs.IntervalDeChangementUnité)
                {
                    case "Minutes":
                        Globs.DateHeureProchainChangement = DateTime.Now.AddMinutes(Globs.IntervalDeChangementDélais);
                        break;

                    case "Heures":
                        Globs.DateHeureProchainChangement = DateTime.Now.AddHours(Globs.IntervalDeChangementDélais);
                        break;

                    case "Jours":
                        Globs.DateHeureProchainChangement = DateTime.Now.AddDays(Globs.IntervalDeChangementDélais);
                        break;

                    case "Seulement au démarrage":
                        Globs.DateHeureProchainChangement = new DateTime();//01/01/0001 00:00:00
                        break;
                }
            }
            else Globs.DateHeureProchainChangement = DateTime.Now.AddYears(10); // on s'est donné Rdv dans 10 ans encore
            // Mise à jour de la date de prochain changement de fond dans le configuration.xml
            Serialiser Config = new Serialiser();
            XmlSerializer mySerializer = new XmlSerializer(typeof(Serialiser));
            StreamWriter writer = new StreamWriter("Configuration.xml");
            new XmlSerializer(typeof(Serialiser)).Serialize(writer, Config);
            writer.Close();
        }

        public static void CalculProchaineMàJDeLaListe()
        {
            switch (Globs.IntervalDeMiseàJourUnité)
            {
                case "Jours":
                    Globs.DateHeureProchaineMiseàJourDeLaListe = DateTime.Now.AddDays(Globs.IntervalDeMiseàJourDélais);
                    break;

                case "Mois":
                    Globs.DateHeureProchaineMiseàJourDeLaListe = DateTime.Now.AddMonths(Globs.IntervalDeMiseàJourDélais);
                    break;

                default:
                    Globs.DateHeureProchaineMiseàJourDeLaListe = DateTime.Now.AddYears(10); // 10 ans si la liste est vide
                    break;
            }
            Globs.DateHeureDernièreMiseàJourDeLaListe = DateTime.Now;

            // Mise à jour de la date de prochain changement de fond dans le configuration.xml
            Serialiser Config = new Serialiser();
            XmlSerializer mySerializer = new XmlSerializer(typeof(Serialiser));
            StreamWriter writer = new StreamWriter("Configuration.xml");
            new XmlSerializer(typeof(Serialiser)).Serialize(writer, Config);
            writer.Close();

            //////////////////////////
        }
    }
}