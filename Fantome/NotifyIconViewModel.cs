using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

//using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Fantome
{
    public class NotifyIconViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // private Process EditionImage;
        private TaskCompletionSource<bool> eventHandled;

        private DispatcherTimer timerChangement;

        public NotifyIconViewModel()
        {
            if ((Globs.IntervalDeChangementUnité != "Jours") && (Globs.IntervalDeChangementUnité != "Seulement au démarrage"))

            {
                int IntervalMinutes = Globs.IntervalDeChangementDélais;
                if (Globs.IntervalDeChangementUnité == "Heures") IntervalMinutes = 60 * Globs.IntervalDeChangementDélais;
                timerChangement = new DispatcherTimer();
                timerChangement.Interval = TimeSpan.FromMinutes(IntervalMinutes);
                timerChangement.Tick += timerChangement_Tick;
            }
            if (!Globs.Figé) timerChangement.Start();

            OnPropertyChanged("IcôneSysTray");
            OnPropertyChanged("InfoChgt");
            OnPropertyChanged("NomDuFondCourant");
            OnPropertyChanged("InfoFiger");
        }

        private String FondEdité = "";

        private void timerChangement_Tick(object sender, EventArgs e)
        {
            Fond.Suivant();
            OnPropertyChanged("InfoChgt");
            OnPropertyChanged("NomDuFondCourant");
        }

        protected virtual void OnPropertyChanged(string propertyname)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyname));
        }

        #region Commande bindées

        public ICommand ChangerFond => new DelegateCommand
        {
            CommandAction = () =>
            {
                Fond.Suivant();
                OnPropertyChanged("InfoChgt");
                OnPropertyChanged("NomDuFondCourant");
            }
        };

        public ICommand ChoisirFond => new DelegateCommand
        {
            CommandAction = () =>
            {
                using (OpenFileDialog dlg2 = new OpenFileDialog())
                {
                    dlg2.InitialDirectory = Globs.DossierFondDécranChemin;
                    dlg2.Filter = "Images (*.gif;*.jpeg;*.jpg;*.png;*.bmp)|*.gif;*.jpeg;*.jpg;*.png;*.bmp|Tous (*.*)|*.*";
                    dlg2.Title = "Choisissez un fond d'écran (Il changera automatiquement sauf si vous l'avez figé)";
                    dlg2.FilterIndex = 1;
                    dlg2.ShowDialog();
                    if (dlg2.ShowDialog() == DialogResult.OK)
                    {
                        Fond.ChangeImage(dlg2.FileName);
                        //Fond.ChangeCouleur(Fond.LireLaCouleur(dlg2.FileName));
                        Globs.FondCourant = dlg2.FileName;
                        Temps.CalculProchainChangement();
                    }
                }
            }
        };

        public ICommand Configurer => new DelegateCommand
        {
            CommandAction = () =>
            {
                Window FenConfiguration = new Configuration();
                FenConfiguration.ShowDialog();

                timerChangement.Stop();

                if ((!Globs.Figé) && (Globs.IntervalDeChangementUnité != "Jours") && (Globs.IntervalDeChangementUnité != "Seulement au démarrage"))
                {
                    int IntervalMinutes = Globs.IntervalDeChangementDélais;
                    if (Globs.IntervalDeChangementUnité == "Heures") IntervalMinutes = 60 * Globs.IntervalDeChangementDélais;
                    timerChangement.Interval = TimeSpan.FromMinutes(IntervalMinutes);
                    timerChangement.Start();
                }
                OnPropertyChanged("InfoChgt");
                OnPropertyChanged("NomDuFondCourant");
            }
        };

        public ICommand DeplacerPref => new DelegateCommand
        {
            CommandAction = () =>
            {
                string DossierPréféré = Path.Combine(Globs.DossierFondDécranChemin, "Préféré");
                if (!Directory.Exists(DossierPréféré))
                {
                    Directory.CreateDirectory(DossierPréféré);
                }

                try
                {
                    String destination = Path.Combine(Globs.DossierFondDécranChemin, "Préféré", Path.GetFileName(Globs.FondCourant));
                    File.Move(Globs.FondCourant, destination);
                }
                catch
                {
                }
            }
        };

        public ICommand Editer => new DelegateCommand
        {
            CommandAction = () =>
            {
                eventHandled = new TaskCompletionSource<bool>();

                Process EditionImage = new Process();

                EditionImage.StartInfo.FileName = Globs.EditeurChemin;
                EditionImage.StartInfo.Arguments = "\"" + Globs.FondCourant + "\"";

                FondEdité = Globs.FondCourant;
                EditionImage.Start();
                EditionImage.EnableRaisingEvents = true;
                EditionImage.Exited += new EventHandler(EditionImage_Exited);
            }
        };

        internal void EditionImage_Exited(object sender, System.EventArgs e)
        {
            Fond.ChangeImage(FondEdité);
            //Fond.ChangeCouleur(Fond.LireLaCouleur(FondEdité));
            Temps.CalculProchainChangement();
            OnPropertyChanged("InfoChgt");
            OnPropertyChanged("NomDuFondCourant");
            eventHandled.TrySetResult(true);
        }

        public ICommand Supprimer => new DelegateCommand
        {
            CommandAction = () =>
            {
                String ASupprimer = Globs.FondCourant;
                Fond.Suivant();
                OnPropertyChanged("InfoChgt");
                OnPropertyChanged("NomDuFondCourant");
                File.Delete(ASupprimer);
            }
        };

        public ICommand Recharger => new DelegateCommand
        {
            CommandAction = () =>
            {
                Fond.ChangeImage(Globs.FondCourant);
                //Fond.ChangeCouleur(Fond.LireLaCouleur(Globs.FondCourant));
                Temps.CalculProchainChangement();
                OnPropertyChanged("InfoChgt");
                OnPropertyChanged("NomDuFondCourant");
            }
        };

        public ICommand Figer => new DelegateCommand
        {
            CommandAction = () =>
            {
                Globs.Figé = !Globs.Figé;
                if (Globs.Figé)
                {
                    timerChangement.Stop();
                    Globs.InfoFigé = "Activer le changement automatique";
                    Globs.IcôneDuSystray = @"\Images\FantomeInactif.ico";
                }
                else
                {
                    timerChangement.Start();
                    Globs.InfoFigé = "Figer ce fond";
                    Globs.IcôneDuSystray = @"\Images\Fantome.ico";
                }
                Temps.CalculProchainChangement();
                OnPropertyChanged("IcôneSysTray");
                OnPropertyChanged("InfoChgt");
                OnPropertyChanged("NomDuFondCourant");
                OnPropertyChanged("InfoFiger");
            }
        };

        public ICommand FondNeutre => new DelegateCommand
        {
            CommandAction = () =>
            {
                Fond.ChangeImage(Globs.FondNeutre);
                //Fond.ChangeCouleur(Fond.LireLaCouleur(Globs.FondNeutre));
                Globs.FondCourant = Globs.FondNeutre;
                Temps.CalculProchainChangement();
                OnPropertyChanged("InfoChgt");
                OnPropertyChanged("NomDuFondCourant");
            }
        };

        public ICommand APropos => new DelegateCommand
        {
            CommandAction = () =>
            {
                Window FenAPropos = new APropos();
                FenAPropos.ShowDialog();
            }
        };

        public ICommand Quitter => new DelegateCommand
        {
            CommandAction = () =>
            {
                System.Windows.Application.Current.Shutdown();
            }
        };



        #endregion Commande bindées

            #region Info bindées

        public string NomDuFondCourant
        {
            get { return Globs.FondCourant; }
        }

        public string InfoChgt
        {
            get { return Fond.InfoChgt(); }
        }

        public string IcôneSysTray
        {
            get { return Globs.IcôneDuSystray; }
        }

        public string InfoFiger
        {
            get { return Globs.InfoFigé; }
        }

        #endregion Info bindées
    }
}