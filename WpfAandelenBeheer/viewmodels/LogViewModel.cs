using ClassLibraryAandelen.basis;
using ClassLibraryAandelen.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WpfAandelenBeheer.viewmodels
{
    public class LogViewModel : BaseUserControl
    {
        #region fields
        private IEnumerable<Log> _logLijst;
        private CollectionView _LogView;
        private LogOrigin _selectedLogKind = LogOrigin.None;
        private string _inputSearch;
        private bool _SaveMd = true;
        private bool _SaveHtml = true;
        #endregion

        #region constructor
        /// <summary>
        /// taken van de constructor:
        /// 1. Haalt de repo/context en gebruikers id binnen
        /// 2. Haalt de lijst op van alle logs van de user met de specifieke ID
        /// 3. Stelt de CollectionView gelijkt met de log lijst
        /// 4. Maakt de commando's om de loglijst te kunnen sorteren
        /// 5. Legt de enumeratie logsoorten in een array
        /// 6. Commando aanmaken om file aan te kunnen maken voor de logs
        /// </summary>
        /// <param name="ID">id gebruiker</param>
        /// <param name="repo">repo/context is de datasource</param>
        public LogViewModel(int ID, AandelenRepo repo)
        {
            Repo = repo;
            IdGebruiker = ID;
            Titel = $"Log boek van {repo.GetEigenaar(ID).Naam}";
            LogLijst = repo.LogLijst(ID);
            LogView = (CollectionView)CollectionViewSource.GetDefaultView(LogLijst.ToArray());

            BtnHighDate = new CmdHelper(SorteerHoogsteDatum, LogNietLeeg);
            BtnLowDate = new CmdHelper(SorteerLaagsteDatum, LogNietLeeg);
            LogSoorten = Enum.GetValues(typeof(LogOrigin)).Cast<LogOrigin>();

            CmdSaveFileMdHmtl = new CmdHelper(SaveMarkdownHtmlFiles, () => LogNietLeeg() && (SaveMd || SaveMd));
        }
        #endregion

        #region properties
        public AandelenRepo Repo { get; set; }
        public int IdGebruiker { get; }

        public CmdHelper BtnHighDate { get; private set; }
        public CmdHelper BtnLowDate { get; private set; }
        
        public bool SaveHTML
        {
            get { return _SaveHtml; }
            set
            {
                _SaveHtml = value;
                OnPropertyChanged();
            }
        }

        public Boolean SaveMd
        {
            get { return _SaveMd; }
            set
            {
                _SaveMd = value;
                OnPropertyChanged();
            }
        }

        public CmdHelper CmdSaveFileMdHmtl { get; private set; }

        public IEnumerable<LogOrigin> LogSoorten { get; }

        public Predicator PredHelper { get; set; } = new Predicator();

        public LogOrigin SelectedLogKind
        {
            get { return _selectedLogKind; }
            set
            {
                if (_selectedLogKind != value)
                {
                    _selectedLogKind = value;
                    OnPropertyChanged();
                    Filter();
                }
            }
        }

        public CollectionView LogView
        {
            get { return _LogView; }
            private set
            {
                if (_LogView != value)
                {
                    _LogView = value;
                    NotifyProperties();
                    Filter();
                }
            }
        }

        public IEnumerable<Log> LogLijst
        {
            get { return _logLijst; }
            set
            {
                if (_logLijst != value)
                {
                    _logLijst = value;
                    LogView = (CollectionView)CollectionViewSource.GetDefaultView(LogLijst.ToArray());
                    NotifyProperties("LogView");
                }
            }
        }

        public String InputSearch
        {
            get { return _inputSearch; }
            set
            {
                if (_inputSearch != value)
                {
                    _inputSearch = value;
                    NotifyProperties();
                    Filter();
                }
            }
        }

        #endregion

        #region methods
        /// <returns>Laat weten of de logview leeg is of niet.</returns>
        public bool LogNietLeeg() => LogView.Count != 0;

        /// <summary>
        /// Maakt de sorteerbeschrijving leeg en voegt er een nieuwe toe om te aflopend te sorteren op de datum van de log
        /// </summary>
        private void SorteerHoogsteDatum()
        {
            LogView.SortDescriptions.Clear();
            LogView.SortDescriptions.Add(new SortDescription("DateLog", ListSortDirection.Ascending));
        }

        /// <summary>
        /// Maakt de sorteerbeschrijving leeg en voegt er een nieuwe toe om te oplopend te sorteren op de datum van de log
        /// </summary>
        private void SorteerLaagsteDatum()
        {
            LogView.SortDescriptions.Clear();
            LogView.SortDescriptions.Add(new SortDescription("DateLog", ListSortDirection.Descending));
        }

        /// <summary>
        /// Methode om de logs te filteren. De filteringsvoorwaarde is een predicaat dat in de logview Filter wordt gezet. De gezette
        /// predicaat hangt af van de staat van de properties verandwoordelijk waarop gefiltert zal zijn.
        /// </summary>
        private void Filter()
        {
            PredHelper.PredList.Clear();
            PredHelper.Add(IsInputSearchEmpty, T => (T as Log).Beschrijving.ToLower().Contains(InputSearch.ToLower()));
            PredHelper.Add(IsNothingSelected, T => (T as Log).LogOrigin.Equals(SelectedLogKind));
            LogView.Filter = obj => PredHelper.IsEmpty() ? PredHelper.Filter(obj) : true;
        }

        /// <summary>
        /// Methode om de gebruiker de keuze te geven om een file(s) aan te maken om de html/markdown beschrijving van de logs
        /// in te zetten als de juiste property op true is gezet voor die specifieke formaat.
        /// </summary>
        private void SaveMarkdownHtmlFiles()
        {
            String MarkDownText = Log.LogMarkdownReport(CollectionViewSource.GetDefaultView(LogView.SourceCollection).Cast<Log>(),
                Repo.GetEigenaar(IdGebruiker).Naam);
            if (Messenger.ReturnMessage("Bent u zeker dat u file wilt opslaan?") == MessageBoxResult.Yes)
            {
                String pathToSaveFile = FileManager.PathToSaveFile();
                if (SaveMd) FileManager.SaveFileMd(pathToSaveFile, MarkDownText);
                if (SaveHTML) FileManager.SaveFileHtml(pathToSaveFile, MarkDownText);
            }
        }

        public bool IsInputSearchEmpty() => !string.IsNullOrEmpty(InputSearch);

        public bool IsNothingSelected() => !SelectedLogKind.Equals(LogOrigin.None);
        #endregion
    }
}