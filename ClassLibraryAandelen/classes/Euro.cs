using System;

namespace ClassLibraryAandelen.classes
{
    public static class Euro
    {
        /// <summary>
        /// Verwerkt een double bedrag tot een euro compatible gedrag met 2 decimalen na de komma
        /// </summary>
        /// <param name="onVerwerkteBedrag">Bedrag dat verwerkt zal worden</param>
        /// <returns>Verwerkt euro compatible bedrag</returns>
        public static Double toEuro(this Double onVerwerkteBedrag) => Math.Floor(onVerwerkteBedrag * 100) / 100;
        
        /// <returns>bedrag dat een euro teken zal krijgen</returns>
        public static String EuroSum(this Double euroBedrag) => $"{euroBedrag.toEuro()} €";
    }
}
