using ClassLibraryAandelen.basis;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ClassLibraryAandelen.classes
{
    /**
     * Klasse dat de portefeuille van een eigenaar voorstelt. Elke portefeuille heeft een lijst van aandelen
     * die hierna beheert kunnen worden.
     * **/
    public class Portefeuille : Notifyable, IMarkdownable
    {
        #region fields
        [Key]
        public int ID { get; set; }
        [Required]
        private String _naam;
        [Required]
        public ObservableCollection<Aandeel> Aandelen { get; set; }
        #endregion
        #region constructor
        /// <summary>
        /// instantieert een Portefeuille met zijn eigen id en naam
        /// </summary>
        /// <param name="ID">Unieke id nummer</param>
        /// <param name="Naam">Naam portefeuille</param>
        internal Portefeuille(int ID, String Naam)
        {
            this.ID = ID;
            this.Naam = Naam;
            Aandelen = new ObservableCollection<Aandeel>();
        }

        public Portefeuille(String Naam) : this(0, Naam) { }

        public Portefeuille() : this(0, "") { }
        #endregion
        #region properties
        /**
         * Wanneer de property verandert worden dan wordt de onpropertychanged gecalled op de Naam Property en op de Identity.
         * Het wordt ook op de Identity property getriggert omdat deze altijd up to date.
         * **/
        public String Naam
        {
            get { return _naam; }
            set
            {
                if (_naam != value) _naam = value;
                OnPropertyChanged();
                OnPropertyChanged("Identity");
            }
        }

        public Double TotaleWaarde
        {
            get
            {
                Double TotaleWaarde = 0;
                if (Aandelen.Count != 0) Aandelen.ToList().ForEach(a => TotaleWaarde += a.ActueleWaarde);
                return TotaleWaarde;
            }
        }
        public String Identity => $"Portefeuille {Naam} - Waarde: {TotaleWaarde}";
        #endregion

        #region methods
        /// <summary>
        /// Notifieert dat de totale waarde en Identity properties is verandert, in methode gestoke om buiten de classe
        /// uitgevoert te worden.
        /// </summary>
        public void updatePortefeuille()
        {
            NotifyProperties("TotaleWaarde", "Identity");
        }

        /// <summary>
        /// Retourneert een beschrijving in Markdown formaat over deze portefeuille met de optie om markdown bescrijvingen van 
        /// de aandelen in Markdown beschrijving of niet.
        /// </summary>
        /// <param name="IncludeAandelen">boolean om te zeggen of je de aandelen erbij wilt in Markdown</param>
        /// <returns>Beschrijving van de portefeuilles inhoud</returns>
        public string GetReturnMarkdownDescription(Boolean IncludeAandelen = true)
        {
            StringBuilder builderString = new StringBuilder();
            builderString.AppendLine($"Portefeuille {Naam}".MdTitel(3));
            builderString.AppendLine($"Totaal aandelen: {Aandelen.Count}".UnOrdered());
            builderString.AppendLine($"Totaal waarde: {TotaleWaarde.EuroSum()}".UnOrdered());
            if (Aandelen.Count != 0 && IncludeAandelen)
            {
                builderString.AppendLine();
                builderString.AppendLine("Aandeel | Begin waarde | Actuele waarde | %-verschil actuele & begin waarde");
                builderString.AppendLine("--- |---|---|---");
                foreach (Aandeel aandeel in Aandelen)
                {
                    builderString.AppendLine(aandeel.GetReturnMarkdownDescription());
                }
            }
            builderString.AppendLine(Markdown.HR);
            return builderString.ToString();
        }
        #endregion
    }
}