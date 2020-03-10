using ClassLibraryAandelen.basis;
using ClassLibraryAandelen.classes;
using System;

namespace WpfAandelenBeheer.viewmodels
{
    public class InlogViewModel : BaseUserControl
    {
        #region fields
        private Boolean _MaakBasisPortefeuilles = false;
        private String _InlogNaamEigenaar;
        private String _NieuweNaamEigenaar;
        #endregion

        #region events
        public event Action InlogEvent;
        #endregion

        #region constructor
        /// <summary>
        /// Haalt de repo/context binnen, en maakt de commando's op om de eigenaar toe te voegen en in te kunnen loggen
        /// </summary>
        /// <param name="repo">repo of context waaruit alle data gaat komen of we er data gaan wijzigen</param>
        public InlogViewModel(AandelenRepo repo)
        {
            Repo = repo;
            AddEigenaar = new CmdHelper(VoegEigenaarToe, KanEigenaarToevoegen);
            InlogNaam = new CmdHelper(LogInEigenaar, KanEigenaarIdentificeren);
        }
        #endregion

        #region properties
        public AandelenRepo Repo { get; private set; }

        public String NieuweNaamEigenaar
        {
            get { return _NieuweNaamEigenaar; }
            set
            {
                if (_NieuweNaamEigenaar != value)
                {
                    _NieuweNaamEigenaar = value;
                    OnPropertyChanged();
                }
            }
        }

        public String InlogNaamEigenaar
        {
            get { return _InlogNaamEigenaar; }
            set
            {
                if (_InlogNaamEigenaar != value)
                {
                    _InlogNaamEigenaar = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Boolean MaakBasisPortefeuilles
        {
            get { return _MaakBasisPortefeuilles; }
            set
            {
                if (_MaakBasisPortefeuilles != value)
                {
                    _MaakBasisPortefeuilles = value;
                    OnPropertyChanged();
                }
            }
        }

        public CmdHelper AddEigenaar { get; private set; }
        public CmdHelper InlogNaam { get; private set; }
        #endregion

        #region methods
        /// <summary>
        /// Methode om een gebruiker toe te voegen, hierna op basis van of de gebruiker basis portefeuilles wilt aan maken of niet. Worden
        /// basis portefeuilles bij de gebruiker toegevoegd.
        /// try:
        ///     Success bericht wordt getoond
        /// catch:
        ///     Bericht toont de fout
        /// </summary>
        private void VoegEigenaarToe()
        {
            try
            {
                Repo.AddEigenaar(NieuweNaamEigenaar);
                int idGebruiker = Repo.GetEigenaarID(NieuweNaamEigenaar);
                if (MaakBasisPortefeuilles) MaakPortefeuilles(idGebruiker);
                Messenger.SuccessMessage("Welkom " + NieuweNaamEigenaar);
            }
            catch (Exception e)
            {
                Messenger.ErrorMessage(e.Message);
            }
            NieuweNaamEigenaar = "";
            MaakBasisPortefeuilles = false;
        }

        /// <summary>
        /// 2 portefeuilles worden bij de gebruiker toegevoegd.
        /// </summary>
        /// <param name="idGebruiker">id nummer van de gebruiker bij wie de portefeuilles zullen toegevoegd worden</param>
        private void MaakPortefeuilles(int idGebruiker)
        {
            Repo.AddPortefeuille(idGebruiker, "Aandelen");
            Repo.AddPortefeuille(idGebruiker, "Staatsbon");
        }

        /// <summary>
        /// Controleert of de inlognaam voorkomt in een van de eigenaarsnamen, en als het overeen komt dan wordt de inlogevent uitgevoerdt, 
        /// anders wordt er een waarschuwing gegeven.
        /// </summary>
        private void LogInEigenaar()
        {
            bool logInGelukt = false;
            foreach (Eigenaar eigenaar in Repo.Eigenaars)
            {
                if (logInGelukt = eigenaar.Naam.Equals(InlogNaamEigenaar))
                {
                    Messenger.SuccessMessage("inloggen als " + eigenaar.Naam);
                    Repo.AddLog(new Log(Repo.GetEigenaarID(InlogNaamEigenaar), LogOrigin.User, $"{InlogNaamEigenaar} is ingelogd."));
                    InlogEvent?.Invoke();
                    break;
                }
            }
            if (!logInGelukt) Messenger.WarnMessage("Foute gebruikersnaam of passwoord.");
        }


        /// <summary>
        /// Laat weten of er nieuwe eigenaar toegevoegd kan worden, dit is wanneer de registratie textbox niet leeg is
        /// </summary>
        /// <returns>Boolean</returns>
        private bool KanEigenaarToevoegen() => NieuweNaamEigenaar != "";

        /// <summary>
        /// Laat weten of er nieuwe eigenaar toegevoegd kan worden, dit is wanneer de inlog textbox niet leeg is
        /// </summary>
        /// <returns>Boolean</returns>
        private bool KanEigenaarIdentificeren() => InlogNaamEigenaar != "";
        #endregion
    }
}