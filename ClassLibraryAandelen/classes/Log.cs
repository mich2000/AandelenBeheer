using ClassLibraryAandelen.basis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClassLibraryAandelen.classes
{
    /// <summary>
    /// enumeratie van de soorten logs dat er bestaat in het programma
    /// </summary>
    public enum LogOrigin { User, Portefeuille, Aandelen, None };
    
    [Table("TblLogs")]
    public class Log:Notifyable, IMarkdownable
    {
        #region fields
        private int _idGebruiker;
        private String _beschrijving;
        private LogOrigin _logOrigin;
        #endregion

        #region properties
        [Key]
        public int ID { get; set; }

        public DateTime DateLog { get; }

        public int IdGebruiker
        {
            get { return _idGebruiker; }
            set
            {
                if (_idGebruiker != value)
                {
                    _idGebruiker = value;
                    OnPropertyChanged();
                }
            }
        }

        public LogOrigin LogOrigin
        {
            get { return _logOrigin; }
            set
            {
                if (_logOrigin != value)
                {
                    _logOrigin = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public String Beschrijving
        {
            get { return _beschrijving; }
            set
            {
                if (_beschrijving != value)
                {
                    _beschrijving = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region constructors
        /// <summary>
        /// Constructor om een instantie te maken van Log
        /// </summary>
        /// <param name="idGebruiker">unieke id nummer van log</param>
        /// <param name="logOrigin">soort log</param>
        /// <param name="beschrijving">beschrijft wat er gebeurt</param>
        /// <param name="dateTime">wanneer de log gegenereert werd</param>
        internal Log(int idGebruiker, LogOrigin logOrigin, string beschrijving, DateTime dateTime)
        {
            IdGebruiker = idGebruiker;
            LogOrigin = logOrigin;
            Beschrijving = beschrijving;
            DateLog = dateTime;
        }

        public Log(int idGebruiker, LogOrigin logOrigin, string beschrijving):this(idGebruiker, logOrigin, beschrijving, DateTime.Now) { }

        public Log() : this(0, LogOrigin.None, "", DateTime.Now) { }
        #endregion

        #region methods
        /// <returns>Markdown beschrijving van deze aandeel</returns>
        public string GetReturnMarkdownDescription(bool includeOptions = true) => $"{IdGebruiker}|{DateLog}|{LogOrigin.ToString()}|{Beschrijving}";
        
        /// <returns>de string met de log soort en de beschrijving</returns>
        public override string ToString() => $"{LogOrigin.ToString()}: {Beschrijving}";

        /// <summary>
        /// retourneert een tuple van de minimum en maximum datum van de enumerable log lijst
        /// </summary>
        /// <param name="logLijst">lijst van logs</param>
        /// <returns>retourneert een tuple en minimum/maximum datum van log lijst</returns>
        public static (DateTime, DateTime) DateRangeLog(ref IEnumerable<Log> logLijst)
        {
            return (logLijst.Count() != 0) ? (logLijst.Min(l => l.DateLog), logLijst.Max(l => l.DateLog)):(DateTime.Now, DateTime.Now);
        }
        
        /// <summary>
        /// Retourneert de beschrijving van de hele log lijst dat doorgevoerdt werd. Geeft ook meta informatie van de log lijst.
        /// </summary>
        /// <param name="logLijst">lijst van alle logs</param>
        /// <param name="naamEigenaar">naam van de eigenaar van de logs</param>
        /// <returns>Markdown beschrijving van de alle logs</returns>
        public static String LogMarkdownReport(IEnumerable<Log> logLijst, string naamEigenaar)
        {
            StringBuilder builderMd = new StringBuilder(); 
            (DateTime,DateTime) rangeDate = DateRangeLog(ref logLijst);
            builderMd.AppendLine($"Log Boek van {naamEigenaar}".MdTitel());
            builderMd.AppendLine($"Log boek bereik: {rangeDate.Item1} - {rangeDate.Item2}".UnOrdered());
            builderMd.AppendLine($"Aantal logs: {logLijst.Count()}".UnOrdered());
            builderMd.AppendLine(Markdown.HR);
            builderMd.AppendLine("ID user | Datum en tijd log | Oorsprong log | Beschrijving");
            builderMd.AppendLine("--- |---|---|---");
            foreach (Log log in logLijst) builderMd.AppendLine(log.GetReturnMarkdownDescription());
            return builderMd.ToString();
        }
        #endregion
    }
}