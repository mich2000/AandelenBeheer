using System;
using System.Collections.Generic;

namespace ClassLibraryAandelen.basis
{
    public class Predicator
    {
        #region constructor
        /// <summary>
        /// Initialiseert een instantie van de klasse Predicator, deze initialiseert de property PredList. Dit is een lijst van Func's
        /// die een object als parameter innemen en geeft terug een boolean.
        /// </summary>
        public Predicator()
        {
            PredList = new List<Func<object, bool>>();
        }
        #endregion

        #region properties
        public List<Func<object, bool>> PredList { get; set; }
        #endregion

        #region methods
        /// <summary>
        /// Voegt een Func toe als de voorwaarde op true is.
        /// </summary>
        /// <param name="condition">Voorwaarde om de Func bij de Func lijst toe te voegen</param>
        /// <param name="predicate">Func die kan toegevoegd worden bij de lijst</param>
        public void Add(bool condition, Func<Object, bool> predicate)
        {
            if (condition) PredList.Add(predicate);
        }

        public void Add(Func<bool> condition, Func<Object, bool> predicate)
        {
            if (condition.Invoke()) PredList.Add(predicate);
        }

        /// <summary>
        /// enumereert over de hele Func lijst en zet de object in de invokatie methode om te zien of de parameter gefiltert moet zijn of niet.
        /// </summary>
        /// <param name="obj">parameter dat zal gebruikt worden om te zien of deze gefiltert zou moeten worden of niet.</param>
        /// <returns>Of de parameter dat doorgevoert wordt om gefiltert te worden.</returns>
        //public bool Filter(object obj)
        //{
        //    bool passes = true;
        //    PredList.ForEach(predicate =>
        //    {
        //        if (passes) passes = predicate.Invoke(obj);
        //    });
        //    return passes;
        //}

        public bool Filter(object obj)
        {
            foreach (Func<object, bool> predicate in PredList) 
                if (!predicate.Invoke(obj)) return false;
            return true;
        }

        /// <summary>
        /// Geeft aan of de Func lijst leeg is of niet.
        /// </summary>
        /// <returns>Boolean dat aangeeft of de Func lijst is leeg of niet.</returns>
        public bool IsEmpty() => PredList.Count != 0;
        #endregion
    }
}