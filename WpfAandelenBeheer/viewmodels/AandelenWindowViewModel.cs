using ClassLibraryAandelen.basis;
using ClassLibraryAandelen.classes;
using System;
using System.Collections.ObjectModel;

namespace WpfAandelenBeheer.viewmodels
{
    public class AandelenWindowViewModel : BaseUserControl
    {
        #region fields
        private Portefeuille _CurrentPortefeuille;
        private Aandeel _selecteerdeAandeel;
        private ObservableCollection<Aandeel> _aandelen;

        private Double _ActueleWaarde = 0;
        private Double _BeginWaarde = 0;
        private String _BedrijfsNaam;

        AandelenRepo repo;
        #endregion

        #region events
        public event Action AddAandeelEvent, RemoveAandeelEvent, UpdateAandeelEvent;
        #endregion

        #region constructor
        /// <summary>
        /// taken van de constructor:
        /// 1. haalt de repo(context) en id van de gebruiker binnen en stelt ze gelijk aan de velden in de klasse, om later gebruikt te worden.
        /// 2. Titel wordt verandert
        /// 3. Aandelen worden binnengehaald
        /// 4. Commando's voor het toevoegen, wijzigen en verwijderen worden aangemaakt
        /// </summary>
        /// <param name="repo">context waaruit alle data zal uit komen</param>
        /// <param name="idGebruiker">id van de gebruiker</param>
        /// <param name="portefeuille">Portefeuille waar uit de aandelen zullen uit komen.</param>
        public AandelenWindowViewModel(AandelenRepo repo, int idGebruiker, Portefeuille portefeuille)
        {
            this.repo = repo;
            IdGebruiker = idGebruiker;
            CurrentPortefeuille = portefeuille;
            Titel = $"Aandelen uit portefeuille {CurrentPortefeuille.Naam}";
            AandelenCollectie = repo.GetAandelen(idGebruiker, portefeuille);
            CmdAddAandeel = new CmdHelper(VoegAandeelToe, KanAandeelToevoegen);
            CmdRemoveAandeel = new CmdHelper(VerwijderAandeel, KanAandeelVerwijderenUpdate);
            CmdUpdateAandeel = new CmdHelper(UpdateAandeel, KanAandeelVerwijderenUpdate);
        }
        #endregion

        #region properties
        public int IdGebruiker { get; set; }

        public CmdHelper CmdAddAandeel { get; private set; }
        public CmdHelper CmdRemoveAandeel { get; private set; }
        public CmdHelper CmdUpdateAandeel { get; private set; }

        public String BedrijfsNaam
        {
            get { return _BedrijfsNaam; }
            set
            {
                if (_BedrijfsNaam != value) _BedrijfsNaam = value;
                OnPropertyChanged();
            }
        }

        public Double BeginWaarde
        {
            get { return _BeginWaarde; }
            set
            {
                if (_BeginWaarde != value) _BeginWaarde = value.toEuro();
                OnPropertyChanged();
            }
        }

        public Double ActueleWaarde
        {
            get { return _ActueleWaarde; }
            set
            {
                if (_ActueleWaarde != value) _ActueleWaarde = value.toEuro();
                OnPropertyChanged();
            }
        }

        public Aandeel SelecteerdeAandeel
        {
            get { return _selecteerdeAandeel; }
            set
            {
                if (_selecteerdeAandeel != value)
                {
                    _selecteerdeAandeel = value;
                }
                if (_selecteerdeAandeel != null)
                {
                    BedrijfsNaam = SelecteerdeAandeel.Bedrijfsnaam;
                    BeginWaarde = SelecteerdeAandeel.BeginWaarde;
                    ActueleWaarde = SelecteerdeAandeel.ActueleWaarde;
                }
                NotifyProperties("AandelenCollectie");
            }
        }

        public Portefeuille CurrentPortefeuille
        {
            get { return _CurrentPortefeuille; }
            set
            {
                if (_CurrentPortefeuille != value) _CurrentPortefeuille = value;
                OnPropertyChanged();
                RefreshViewModel();
            }
        }

        public ObservableCollection<Aandeel> AandelenCollectie
        {
            get { return _aandelen; }
            set
            {
                if (_aandelen != value) _aandelen = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Als de portefeuille niet leeg is wordt, de titel en aandelen uit de portefeuille aangepast op de nieuwe portefeuille en 
        /// worden de aandelen form geleegd
        /// </summary>
        public void RefreshViewModel()
        {
            if (CurrentPortefeuille != null)
            {
                Titel = $"Aandelen uit portefeuille {CurrentPortefeuille.Naam}";
                AandelenCollectie = repo.GetAandelen(IdGebruiker, CurrentPortefeuille);
                MaakVeldenLeeg();
            }
        }

        /// <summary>
        /// Voegt een aandeel toe op de portefeuille van dit moment en leegt de aandelen form.
        /// </summary>
        private void VoegAandeelToe()
        {
            repo.AddAandeel(IdGebruiker, CurrentPortefeuille, new Aandeel(BedrijfsNaam, BeginWaarde, ActueleWaarde));
            MaakVeldenLeeg();
            AddAandeelEvent?.Invoke();
        }

        /// <summary>
        /// Verwijdert een aandeel uit de portefeuille van dit moment en leegt de aandelen form.
        /// </summary>
        private void VerwijderAandeel()
        {
            repo.RemoveAandeel(IdGebruiker, CurrentPortefeuille, SelecteerdeAandeel);
            MaakVeldenLeeg();
            RemoveAandeelEvent?.Invoke();
        }

        /// <summary>
        /// Wijzigt de geselecteerde aandeel uit het portefeuille.
        /// </summary>
        private void UpdateAandeel()
        {
            repo.UpdateAandeel(IdGebruiker, CurrentPortefeuille, SelecteerdeAandeel, BedrijfsNaam, BeginWaarde, ActueleWaarde);
            OnPropertyChanged("CurrentPortefeuille");
            UpdateAandeelEvent?.Invoke();
        }

        /// <summary>
        /// Leegt de aandelen form
        /// </summary>
        private void MaakVeldenLeeg()
        {
            BedrijfsNaam = "";
            ActueleWaarde = BeginWaarde = 0;
        }

        //Methode om te bepalen of er een aandeel toegevoegd kan worden.
        private bool KanAandeelToevoegen() => BedrijfsNaam != "" && BeginWaarde != 0 && ActueleWaarde != 0;

        //Methode om te bepalen of een aandeel gewijzigd of verwijdert kan worden.
        private bool KanAandeelVerwijderenUpdate() => CurrentPortefeuille != null && SelecteerdeAandeel != null;
        #endregion
    }
}