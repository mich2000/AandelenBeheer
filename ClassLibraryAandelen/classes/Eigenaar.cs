using ClassLibraryAandelen.basis;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClassLibraryAandelen.classes
{
    /**
     * Klasse dat de eigenaar voorstelt. Met deze klasse heeft de eigenaar een naam en een lijst portefeuilles waarin alle aandelen zijn.
     * **/
    [Table("TblEigenaars")]
    public class Eigenaar: IMarkdownable
    {
        #region properties
        [Key]
        public int Id { get; private set; }
        public String Naam { get; set; }
        public ObservableCollection<Portefeuille> Portefeuilles { get; set; }
        #endregion properties

        #region constructor
        /// <summary>
        /// Initialiseert de klasse eigenaar met zijn id en zijn naam
        /// </summary>
        /// <param name="id">unieke id nummer van eigenaar</param>
        /// <param name="naam">naam van eigenaar</param>
        internal Eigenaar(int id, String naam)
        {
            Id = id;
            Naam = naam;
        }

        public Eigenaar(String naam) : this(0, naam)
        {
            Portefeuilles = new ObservableCollection<Portefeuille>();
        }

        public Eigenaar():this("") { }
        #endregion constructor

        #region methods
        /// <summary>
        /// Geeft een markdown beschrijving van de eigenaar, en laat de optie open om de aandelen mee te geven als markdown beschrijving
        /// </summary>
        /// <param name="IncludeAandelen">boolean om te bepalen of de aandelen mee moeten gaan of niet</param>
        /// <returns>Markdown beschrijving van de eigenaar</returns>
        public String GetReturnMarkdownDescription(Boolean IncludeAandelen = true)
        {
            StringBuilder builderString = new StringBuilder();
            builderString.AppendLine($"Eigenaar {Naam}".MdTitel(1));
            builderString.AppendLine($"Totaal aantal portefeuilles: {Portefeuilles.Count}".UnOrdered());
            builderString.AppendLine($"Totaal aantal aandelen: {TotaleAandelen()}".UnOrdered());
            builderString.AppendLine($"Totaal aantal waarde: {TotaleWaarde().EuroSum()}".UnOrdered());
            builderString.AppendLine($"Portefeuilles".MdTitel(2));
            if (Portefeuilles.Count != 0)
            {
                foreach (Portefeuille portefeuille in Portefeuilles)
                {
                    builderString.AppendLine(portefeuille.GetReturnMarkdownDescription(IncludeAandelen));
                }
            }
            return builderString.ToString();
        }

        /// <summary>
        /// controleert het aantal portefeuilles en als het niet gelijk aan nul is, dan wordt het aantal aandelen terug gegeven anders is het een 0
        /// </summary>
        /// <returns>Aantal aandelen</returns>
        private int TotaleAandelen() => (Portefeuilles.Count != 0) ? Portefeuilles.Sum(p => p.Aandelen.Count) : 0;

        /// <summary>
        /// controleert het aantal portefeuilles en als het niet gelijk aan nul is, dan wordt de totale waarde van alle portefeuilles
        /// terug gegeven anders is het een 0
        /// </summary>
        /// <returns>Totale waarde van alle portefeuilles</returns>
        private Double TotaleWaarde() => (Portefeuilles.Count != 0) ? Portefeuilles.Sum(p => p.Aandelen.Sum(a => a.ActueleWaarde)) : 0;
        #endregion
    }
}