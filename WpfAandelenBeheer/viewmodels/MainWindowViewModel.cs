using ClassLibraryAandelen.basis;
using ClassLibraryAandelen.classes;
using System;
using System.Linq;
using System.Windows;

namespace WpfAandelenBeheer.viewmodels
{
    class MainWindowViewModel : BaseUserControl
    {

        #region fields
        private BaseUserControl _basisContentControl;
        private BaseUserControl _aandelenContent;
        private BaseUserControl _rapportLogsContent;

        //Voorstelt de repository, waardoor je met de database kan communiceren
        AandelenRepo repo;
        private int idGebruikteEigenaar;
        #endregion

        #region constructor
        /// <summary>
        /// Taken van het initialiseren van een MainWindowViewModel:
        /// 1. Maken van een nieuwe repo/Data source
        /// 2. Aanmaken van een InlogViewModel en deze zetten in de basis contentcontrole
        /// 3. Herfrist de statusbar
        /// 4. Maakt de verschillende commando's aan voor de menu items in het openklapbaar menu
        /// </summary>
        public MainWindowViewModel()
        {
            repo = new AandelenRepo();
            InlogViewModel = new InlogViewModel(repo);
            BasisContentControl(InlogViewModel);
            RefreshStatusBarProperties();
            CmdVeranderGebruiker = new CmdHelper(() => BasisContentControl(InlogViewModel), ValidUser);
            CmdCloseWindow = new CmdHelper(() => Application.Current.Shutdown());
            CmdToonRapporten = new CmdHelper(ToonRapport, ValidUser);
            CmdToonLogs = new CmdHelper(ToonLogs, ValidUser);
            CmdSaveShortCut = new CmdHelper(OpslaanRapportLogs, () => RapportLogsContent != null);
            CmdRefreshRapport = new CmdHelper(() => RapporteringViewModel.CmdRefreshPreview.Execute(""), ValidRapportLogs);
            CmdHighDate = new CmdHelper(() => LogViewModel.BtnHighDate.Execute(""), ValidRapportLogs);
            CmdLowDate = new CmdHelper(() => LogViewModel.BtnLowDate.Execute(""), ValidRapportLogs);
        }
        #endregion

        #region properties
        //Wordt gebruikt om de portefeuilles te beheren
        public PortefeuillesWindowViewModel PortefeuillesWindowViewModel { get; set; }
        public AandelenWindowViewModel AandelenWindowViewModel { get; set; }
        public RapporteringViewModel RapporteringViewModel { get; set; }
        public LogViewModel LogViewModel { get; set; }
        public InlogViewModel InlogViewModel { get; set; }

        //statusbar properties => basis info over de Eigenaar
        public string NaamGebruiker { get; set; }
        public int AantalPortefeuilles { get; set; }
        public int AantalAandelen { get; set; }

        //Menu item commando's
        public CmdHelper CmdVeranderGebruiker { get; private set; }
        public CmdHelper CmdCloseWindow { get; private set; }
        public CmdHelper CmdToonRapporten { get; private set; }
        public CmdHelper CmdToonLogs { get; private set; }
        public CmdHelper CmdOpenMenu { get; private set; }

        public CmdHelper CmdSaveShortCut { get; private set; }
        public CmdHelper CmdRefreshRapport { get; private set; }
        public CmdHelper CmdHighDate { get; private set; }
        public CmdHelper CmdLowDate { get; private set; }
        
        public BaseUserControl RapportLogsContent
        {
            get { return _rapportLogsContent; }
            set
            {
                if (_rapportLogsContent != value) _rapportLogsContent = value;
                OnPropertyChanged();
            }
        }

        //stelt de inhoud van de contentcontrol voor, deze kan gelijk gesteld worden aan een instantie van de BaseUserControl
        public BaseUserControl BasisContent
        {
            get { return _basisContentControl; }
            set
            {
                if (_basisContentControl != value) _basisContentControl = value;
                OnPropertyChanged();
            }
        }

        public BaseUserControl AandelenContent
        {
            get { return _aandelenContent; }
            set
            {
                if (_aandelenContent != value) _aandelenContent = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Op basis van de usercontrol worden bepaalde events van de usercontrol die doorgegeven, opgevuld met methodes. Dit is alleen als de 
        /// parameter connected op waar staat.
        /// </summary>
        /// <param name="userControl">usercontrol die in de contentcontrol zal gezet worden</param>
        /// <param name="connected">Bepaalt of wat er met de events van de usercontrol zal gebeuren</param
        private void BasisContentControl(BaseUserControl userControl, Boolean connected = true)
        {
            if (connected && userControl != null)
                BasisContentControl(userControl, false);
            if (userControl != null)
            {
                switch (userControl.GetType().Name)
                {
                    case "PortefeuillesWindowViewModel":
                        if (connected)
                        {
                            PortefeuillesWindowViewModel.AddEvent += RefreshStatusBarProperties;
                            PortefeuillesWindowViewModel.UpdateEvent += HandleSelectedPortefeuilleUpdate;
                            PortefeuillesWindowViewModel.RemoveEvent += HandleSelectedPortefeuilleDeletion;
                            PortefeuillesWindowViewModel.SelectedPortefeuilleEvent += ModifyAandelenContentControl;
                            BasisContent = userControl;
                        }
                        else
                        {
                            PortefeuillesWindowViewModel.AddEvent -= RefreshStatusBarProperties;
                            PortefeuillesWindowViewModel.UpdateEvent -= HandleSelectedPortefeuilleUpdate;
                            PortefeuillesWindowViewModel.RemoveEvent -= HandleSelectedPortefeuilleDeletion;
                            PortefeuillesWindowViewModel.SelectedPortefeuilleEvent -= ModifyAandelenContentControl;
                        }
                        break;
                    case "InlogViewModel":
                        if (connected)
                        {
                            InlogViewModel.InlogEvent += InlogEvent;
                            BasisContent = userControl;
                            if(idGebruikteEigenaar != 0)
                            {
                                repo.AddLog(new Log(idGebruikteEigenaar, LogOrigin.User, $"Gebruiker {repo.GetEigenaar(idGebruikteEigenaar).Naam}" +
                                    $" heeft de verbinding los gemaakt."));
                            }
                            idGebruikteEigenaar = 0;
                            RapporteringContentControl(RapportLogsContent, false);
                            AandelenContent = null;
                            RapportLogsContent = null;
                            RefreshStatusBarProperties();
                        }
                        else
                        {
                            InlogViewModel.InlogEvent -= InlogEvent;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Op basis van de usercontrol worden bepaalde events van de usercontrol die doorgegeven, opgevuld met methodes. Dit is alleen als de 
        /// parameter connected op waar staat.
        /// </summary>
        /// <param name="userControl">usercontrol die in de contentcontrol zal gezet worden</param>
        /// <param name="connected">Bepaalt of wat er met de events van de usercontrol zal gebeuren</param
        private void RapporteringContentControl(BaseUserControl userControl, Boolean connected = true)
        {
            if (connected && userControl != null)
                RapporteringContentControl(userControl, false);
            if (userControl != null)
            {
                switch (userControl.GetType().Name)
                {
                    case "RapporteringViewModel":
                        if (connected)
                        {
                            RapportLogsContent = userControl;
                        }
                        break;
                    case "LogViewModel":
                        if (connected)
                        {
                            RapportLogsContent = userControl;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Verandert de portefeuille waarop de aandelen viewmodel, door een andere portefeuille. Dit zorgt ervoor dat de aandelen van 
        /// de aandelen viewmodel zich baseren op de nieuwe portefeuille.
        /// </summary>
        /// <param name="portefeuille">portefeuille dat in de aandelen viewmodel gaat gaan</param>
        private void HandleSelectedPortefeuilleUpdate(Portefeuille portefeuille)
        {
            AandelenWindowViewModel.CurrentPortefeuille = portefeuille;
            RefreshStatusBarProperties();
        }

        /// <summary>
        /// Als de aandelen viewmodel of de aandelen viewmodel leeg zijn dan wordt er een nieuwe aandelen view model aangemaakt zijn. Hierna 
        /// worden de id, portefeuille en contentcontrole voor aandelen geüpdatet.
        /// </summary>
        /// <param name="portefeuille">portefeuille dat gaat dienen om de aandelen view model te vullen met aandelen erin</param>
        private void ModifyAandelenContentControl(Portefeuille portefeuille)
        {
            if (AandelenWindowViewModel == null && PortefeuillesWindowViewModel.SelectedPortefeuille != null)
                AandelenWindowViewModel = new AandelenWindowViewModel(repo, idGebruikteEigenaar, PortefeuillesWindowViewModel.SelectedPortefeuille);
            AandelenRapporteringContentControle(AandelenWindowViewModel);
            AandelenWindowViewModel.IdGebruiker = idGebruikteEigenaar;
            AandelenWindowViewModel.CurrentPortefeuille = portefeuille;
        }

        /// <summary>
        /// Op basis van de usercontrol worden bepaalde events van de usercontrol die doorgegeven, opgevuld met methodes. Dit is alleen als de 
        /// parameter connected op waar staat.
        /// </summary>
        /// <param name="userControl">usercontrol die in de contentcontrol zal gezet worden</param>
        /// <param name="connected">Bepaalt of wat er met de events van de usercontrol zal gebeuren</param
        private void AandelenRapporteringContentControle(BaseUserControl userControl, Boolean connected = true)
        {
            if (connected && userControl != null)
                AandelenRapporteringContentControle(userControl, false);
            if (userControl != null)
            {
                switch (userControl.GetType().Name)
                {
                    case "AandelenWindowViewModel":
                        if (connected)
                        {
                            AandelenWindowViewModel.AddAandeelEvent += RefreshStatusBarProperties;
                            AandelenWindowViewModel.RemoveAandeelEvent += RefreshStatusBarProperties;
                            AandelenWindowViewModel.UpdateAandeelEvent += RefreshStatusBarProperties;
                            AandelenContent = userControl;
                        }
                        else
                        {
                            AandelenWindowViewModel.AddAandeelEvent -= RefreshStatusBarProperties;
                            AandelenWindowViewModel.RemoveAandeelEvent -= RefreshStatusBarProperties;
                            AandelenWindowViewModel.UpdateAandeelEvent -= RefreshStatusBarProperties;
                            AandelenContent = null;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// taken van inlogevent:
        /// 1. pakt de eigenaars id van de inlog viewmodle
        /// 2. zet de id nummer van de mainviewmodel gelijk aan de eigenaars id
        /// 3. Leegt de inlognaam in de inlog viewmodel
        /// 4. Maakt een nieuwe portefeuille viewmodel en zet de portefeuille contentcontrol hieraan gelijk
        /// 5. Roept de methode om de rapport te tonen
        /// 6. Herfrist de statusbar
        /// </summary>
        private void InlogEvent()
        {
            int gebruikersNummerLogin = repo.Eigenaars.FirstOrDefault(e => e.Naam == InlogViewModel.InlogNaamEigenaar).Id;
            idGebruikteEigenaar = gebruikersNummerLogin;
            InlogViewModel.InlogNaamEigenaar = "";
            PortefeuillesWindowViewModel = new PortefeuillesWindowViewModel(repo, idGebruikteEigenaar);
            BasisContentControl(PortefeuillesWindowViewModel);
            ToonRapport();
            RefreshStatusBarProperties();
        }

        /// <summary>
        /// Herfrist de statusbar. Zet gelijk aan nul de aandelen content als er geen geselecteerde portefeuille is in 
        /// portefeuille viewmodel.
        /// </summary>
        private void HandleSelectedPortefeuilleDeletion()
        {
            RefreshStatusBarProperties();
            if (PortefeuillesWindowViewModel.SelectedPortefeuille == null) AandelenContent = null;
        }

        /// <summary>
        /// Verwijdert alle events van de rapport log content zodat deze niet afgevuurd kunnen worden. Maakt een nieuwe rapport viewmodel
        /// aan en zet deze gelijk aan de rapoort log contentcontrole. Zet de breedte van de rapport log contentcontrole aan 350 px.
        /// </summary>
        private void ToonRapport()
        {
            RapporteringContentControl(RapportLogsContent, false);
            RapporteringViewModel = new RapporteringViewModel(idGebruikteEigenaar, repo);
            RapporteringContentControl(RapporteringViewModel);
        }

        /// <summary>
        /// Roept de opslaan commando van rapportering of log viewmodel, een van deze 2 zal geroepen geworden als deze beschikbaar zijn.
        /// </summary>
        private void OpslaanRapportLogs()
        {
            switch (RapportLogsContent.GetType().Name)
            {
                case "RapporteringViewModel":
                    RapporteringViewModel.CmdSaveFileMdHmtl.Execute("");
                    break;
                case "LogViewModel":
                    LogViewModel.CmdSaveFileMdHmtl.Execute("");
                    break;
            }
        }
        
        /// <summary>
        /// Verwijdert alle events van de rapport log content zodat deze niet afgevuurd kunnen worden. Maakt een nieuwe log viewmodel
        /// aan en zet deze gelijk aan de rapoort log contentcontrole. Zet de breedte van de rapport log contentcontrole aan 350 px.
        /// </summary>
        private void ToonLogs()
        {
            RapporteringContentControl(RapportLogsContent, false);
            LogViewModel = new LogViewModel(idGebruikteEigenaar, repo);
            RapporteringContentControl(LogViewModel);
        }

        /// <summary>
        /// Controleert of de id wel goed is. als deze slecht is dan wordt de content van de statusbar gelijk gestelt aan niks of nul. Als de
        /// id wel goed is dan wordt er basis informatie over de user gepakt en gezet in de statusbar. De recentste log lijst wordt gelijk 
        /// gezet aan de log viewmodel om de logs up te daten.
        /// </summary>
        private void RefreshStatusBarProperties()
        {
            if (idGebruikteEigenaar != 0)
            {
                if (PortefeuillesWindowViewModel.SelectedPortefeuille != null)
                    PortefeuillesWindowViewModel.SelectedPortefeuille.updatePortefeuille();
                NaamGebruiker = repo.GetEigenaar(idGebruikteEigenaar).Naam;
                AantalPortefeuilles = repo.AantalPortefeuilles(idGebruikteEigenaar);
                AantalAandelen = repo.AantalAandelen(idGebruikteEigenaar);
                if (LogViewModel != null)
                {
                    LogViewModel.LogLijst = repo.LogLijst(idGebruikteEigenaar);
                }
            }
            else
            {
                NaamGebruiker = "";
                AantalPortefeuilles = 0;
                AantalAandelen = 0;
            }
            NotifyProperties("NaamGebruiker", "AantalPortefeuilles", "AantalAandelen");
        }

        /// <summary>
        /// Methode om te laten weten of de gebruiker wel geldig is en dat communicatie met de datasource/repo.
        /// </summary>
        /// <returns>Boolean of de gebruiker geldig is</returns>
        private bool ValidUser() => idGebruikteEigenaar != 0 && repo != null;

        /// <summary>
        /// Methode om aan te geven of rapportlogscontent leeg is of niet.
        /// </summary>
        /// <returns>Boolean dat wanneer waar is, betekent dat rapportlogscontent niet leeg is.</returns>
        private bool ValidRapportLogs() => RapportLogsContent != null;
        #endregion
    }
}