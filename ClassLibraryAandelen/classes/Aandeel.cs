using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ClassLibraryAandelen.basis;
using ClassLibraryAandelen.exceptions;

namespace ClassLibraryAandelen.classes
{
    //klasse dat dient om alle aandelen voor te stellen
    [Table("tblAandelen")]
    public class Aandeel : Notifyable, IMarkdownable
    {
        #region fields
        //_beginWaarde is de waarde waarmee het aandeel begint
        //_actueleWaarde is de waarde op het moment zelf
        private Double _beginWaarde;
        private Double _actueleWaarde;
        private String _Bedrijfsnaam;
        #endregion

        #region properties
        [Key]
        public int ID { get; set; }
        //Naam van het bedrijf van de aandeel
        public String Bedrijfsnaam
        {
            get { return _Bedrijfsnaam; }
            set
            {
                if (_Bedrijfsnaam != value)
                {
                    _Bedrijfsnaam = value;
                    NotifyProperties("Identity");
                }
            }
        }

        public Double PercentageVerschilActueleBeginWaarde => Math.Round((ActueleWaarde - BeginWaarde) / BeginWaarde, 2) * 100;

        //Een beginwaarde van een aandeel kan niet negatief zijn anders zal deze een NegatieveBeginWaardeAandeelException gooien
        public Double BeginWaarde
        {
            get { return _beginWaarde; }
            set
            {
                if (value > 0)
                {
                    _beginWaarde = value.toEuro();
                    NotifyProperties("Identity", "PercentageVerschilActueleBeginWaarde");
                }
                else throw new NegatieveBeginWaardeAandeelException("Begin waarde van een aandeel kan niet negatief zijn");
            }
        }

        /**
         * Een Actuele waarde van een aandeel kan niet negatief zal anders zijn NegatieveActueleWaardeException gooien
         * **/
        public Double ActueleWaarde
        {
            get { return _actueleWaarde; }
            set
            {
                if (value > 0)
                {
                    _actueleWaarde = value.toEuro();
                    NotifyProperties("Identity", "PercentageVerschilActueleBeginWaarde");
                }
                else throw new NegatieveActueleWaardeException("Actuele waarde van een aandeel kan niet negatief zijn");
            }
        }

        public String Identity => $"Aandeel {ID}: {Bedrijfsnaam} \nBegin waarde: {BeginWaarde} - Actuele waarde: {ActueleWaarde}";
        #endregion

        #region constructor
        /// <summary>
        /// Stelt voor een aandeel met een unieke id nummer, een bedrijfsnaam, een begin en een actuele waarde.
        /// </summary>
        /// <param name="iD">uniek id van de aandeel</param>
        /// <param name="bedrijfsnaam">naam van het bedijf van de aandeel</param>
        /// <param name="beginWaarde">Begin waarde van de andeel</param>
        /// <param name="actueleWaarde">Actuele waarde van de aandeel</param>
        internal Aandeel(int iD, string bedrijfsnaam, double beginWaarde, double actueleWaarde)
        {
            ID = iD;
            Bedrijfsnaam = bedrijfsnaam;
            _beginWaarde = beginWaarde;
            ActueleWaarde = actueleWaarde;
        }

        public Aandeel(string bedrijfsnaam, double beginWaarde, double actueleWaarde) : this(0, bedrijfsnaam, beginWaarde, actueleWaarde) { }

        public Aandeel() : this(null, 0, 0) { }
        #endregion

        #region methods
        /// <summary>
        /// Methode om markdown beschrijving terug te geven van de aandeel voor in tabel vorm.
        /// </summary>
        /// <param name="includeOptions"></param>
        /// <returns>aandeel beschrijving in markdown tabel vorm</returns>
        public string GetReturnMarkdownDescription(bool includeOptions = true)
        {
            return $"{Bedrijfsnaam} | {BeginWaarde.EuroSum()} | {ActueleWaarde.EuroSum()} | {PercentageVerschilActueleBeginWaarde} %";
        }
        #endregion
    }
}