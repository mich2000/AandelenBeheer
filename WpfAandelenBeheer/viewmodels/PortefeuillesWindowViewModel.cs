using ClassLibraryAandelen.basis;
using ClassLibraryAandelen.classes;
using System;
using System.Collections.ObjectModel;

namespace WpfAandelenBeheer.viewmodels
{
    public class PortefeuillesWindowViewModel : BaseUserControl
    {
        #region fields
        AandelenRepo repo;
        //Stelt de text van de textbox waarin de naam van de portefeuille in kan gedaan worden.
        private String _naamPortfeuille = "";
        //Collectie van alle portefeuilles van een bepaalde eigenaar
        private ObservableCollection<Portefeuille> _portefeuilles;
        //id nummer van de Eigenaar, deze is op dit moment 1
        private readonly int idGebruiker;
        private Portefeuille _SelectedPortefeuille;
        #endregion

        #region events
        public event Action AddEvent, RemoveEvent;
        public event Action<Portefeuille> SelectedPortefeuilleEvent, UpdateEvent;
        #endregion

        #region constructor
        /// <summary>
        /// viewmodel heeft de repo nodig om met de database te communiceren en de idGebruiker om te kunnen identificeren aan wie
        /// deze portefeuilles zijn. Maakt de verschillende commando's voor de crud operaties voor de portefeuilles.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="nieuweIDgebruiker"></param>
        public PortefeuillesWindowViewModel(AandelenRepo repo, int nieuweIDgebruiker)
        {
            this.repo = repo;
            this.idGebruiker = nieuweIDgebruiker;
            //wordt gebruikt om de naam van de Gebruiker te hebben en deze hierna te tonen.
            NaamEigenaar = repo.GetEigenaar(nieuweIDgebruiker).Naam;
            //Stelt lijst van alle portefeuilles van de gebruiker
            PortefeuilleCollectie = repo.GetPortefeuilles(nieuweIDgebruiker);
            AddCmd = new CmdHelper(AddPortefeuille, CanAddPortefeuille);
            RemoveCmd = new CmdHelper(RemovePortefeuille, CanDeletePortefeuille);
            UpdateCmd = new CmdHelper(UpdatePortefeuille, CanUpdatePortefeulle);
        }
        #endregion

        #region properties
        //Commando om een portefeuille toe te voegen
        public CmdHelper AddCmd { get; private set; }
        //Commando om een portefeuille te verwijderen
        public CmdHelper RemoveCmd { get; private set; }
        //Commando om de naam van een portefeuille te veranderen
        public CmdHelper UpdateCmd { get; private set; }
        //Property die de naam voorstelt van de eigenaar, wordt gebruikt om te tonen wie de portefeuilles bezit
        public String NaamEigenaar { get; set; }

        /**
         * Deze property stelt de listboxitem die geselecteert is in de Listbox waarin alle portefeuilles in zijn.
         * **/
        public Portefeuille SelectedPortefeuille
        {
            get { return _SelectedPortefeuille; }
            set
            {
                if (_SelectedPortefeuille != value) _SelectedPortefeuille = value;
                OnPropertyChanged();
                SelectedPortefeuilleEvent?.Invoke(SelectedPortefeuille);
            }
        }

        public String NaamPortfeuille
        {
            get { return _naamPortfeuille; }
            set
            {
                if (_naamPortfeuille != value) _naamPortfeuille = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Portefeuille> PortefeuilleCollectie
        {
            get { return _portefeuilles; }
            set
            {
                if (_portefeuilles != value)
                {
                    _portefeuilles = value;
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region methods
        //Method gebruikt om een portefeuille toe te voegen en de textbox leeg te maken
        private void AddPortefeuille()
        {
            repo.AddPortefeuille(idGebruiker, NaamPortfeuille);
            NaamPortfeuille = "";
            AddEvent?.Invoke();
        }

        //methode gebruikt om een portefeuille te verwijderen
        private void RemovePortefeuille()
        {
            repo.RemovePortefeuille(idGebruiker, SelectedPortefeuille);
            RemoveEvent?.Invoke();
        }

        //methode gebruikt om een portefeuilles naam up te daten en de textbox leeg te maken 
        private void UpdatePortefeuille()
        {
            repo.UpdatePortefeuille(idGebruiker, SelectedPortefeuille, NaamPortfeuille);
            NaamPortfeuille = "";
            UpdateEvent?.Invoke(SelectedPortefeuille);
        }

        //Geeft terug of er een Portefeuille verwijdert kan worden
        private bool CanDeletePortefeuille() => SelectedPortefeuille != null;
        /**
         * Geeft terug of de naam van een portefeuille verandert kan zijn, hierbij moet er een portefeuille geselecteert zijn en moet
         * NaamPortefeuille property(textbox) niet leeg zijn
         * **/
        private bool CanUpdatePortefeulle() => SelectedPortefeuille != null && NaamPortfeuille != "";

        //Geeft terug of een portefeuille toegevoegt kan worden en moet NaamPortefeuille property(textbox) niet leeg zijn
        private bool CanAddPortefeuille() => NaamPortfeuille != "";
        #endregion
    }
}