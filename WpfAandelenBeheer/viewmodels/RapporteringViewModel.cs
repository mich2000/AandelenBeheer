using ClassLibraryAandelen.basis;
using ClassLibraryAandelen.classes;
using System;
using System.Windows;

namespace WpfAandelenBeheer.viewmodels
{
    public class RapporteringViewModel : BaseUserControl
    {
        #region fields
        private readonly int idGebruiker = 0;
        AandelenRepo repo = null;
        private String _MarkDownText;
        private bool _SaveMd = true;
        private bool _SaveHtml = true;
        #endregion

        #region constructor
        /// <summary>
        /// Haalt de id en repo/datasource binnen. Maakt de commando's op om het rapport te herfrissen en het rapporte te saven.
        /// </summary>
        /// <param name="idGebruiker"></param>
        /// <param name="repo"></param>
        public RapporteringViewModel(int idGebruiker, AandelenRepo repo)
        {
            this.idGebruiker = idGebruiker;
            this.repo = repo;
            CmdRefreshPreview = new CmdHelper(RefreshPreviewMd);
            CmdSaveFileMdHmtl = new CmdHelper(SaveFile, CanFileBeSaved);
        }
        #endregion

        #region propertiess
        public Boolean IncludeAandelen { get; set; } = true;

        public bool SaveMd
        {
            get { return _SaveMd; }
            set
            {
                _SaveMd = value;
                OnPropertyChanged();
            }
        }

        public bool SaveHtml
        {
            get { return _SaveHtml; }
            set
            {
                _SaveHtml = value;
                OnPropertyChanged();
            }
        }

        public String MarkDownText
        {
            get { return _MarkDownText; }
            set
            {
                if (_MarkDownText != value) _MarkDownText = value;
                OnPropertyChanged();
            }
        }


        public CmdHelper CmdRefreshPreview { get; private set; }
        public CmdHelper CmdSaveFileMdHmtl { get; private set; }
        #endregion

        #region methods
        /// <summary>
        /// Herfrist het markdown rapport property van de eigenaar.
        /// </summary>
        private void RefreshPreviewMd()
        {
            MarkDownText = repo.GetEigenaar(idGebruiker).GetReturnMarkdownDescription(IncludeAandelen);
        }

        /// <summary>
        /// Herfrist het markdown rapport. Vraagt aan de gebruiker of hij een rapport wilt opslaan in html/md formaat.
        /// Deze files in een specifieke formaat worden alleen opgeslaan als de boolean waarde op waar is voor dat formaat.
        /// </summary>
        private void SaveFile()
        {
            RefreshPreviewMd();
            if(Messenger.ReturnMessage("Bent u zeker dat u file wilt opslaan?") == MessageBoxResult.Yes)
            {
                String pathToSaveFile = FileManager.PathToSaveFile();
                if(SaveMd) FileManager.SaveFileMd(pathToSaveFile, MarkDownText);
                if(SaveHtml) FileManager.SaveFileHtml(pathToSaveFile, MarkDownText);
            }
        }

        /// <summary>
        /// Methode om te bepalen of de file wel opgeslaan kan zijn. De voorwaarde is dat de checkboxes voor html of markdow formaat
        /// op waar zijn.
        /// </summary>
        /// <returns>Boolean</returns>
        private bool CanFileBeSaved() => SaveMd || SaveHtml;
        #endregion
    }
}