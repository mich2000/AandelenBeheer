using System;
using System.Windows;

namespace ClassLibraryAandelen.classes
{
    public static class Messenger
    {
        //Retourneert een Textbox voor successvolle zaken met een tekst dat werd doorgevoert als parameter
        public static void SuccessMessage(String message)
            => MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        
        //Retourneert een Textbox voor onsuccessvolle zaken met een fout dat werd doorgevoert als parameter
        public static void ErrorMessage(String error) => MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        //Retourneert een Textbox voor zaken waar er opgelet moet worden, tekst dat er is, werd doorgevoerd als parameter
        public static void WarnMessage(String warning) => MessageBox.Show(warning, "Waarschuwing", MessageBoxButton.OK, MessageBoxImage.Warning);

        /// <summary>
        /// Retourneert de keuze die gemaakt werd door de gebruiker door op de button te klikken
        /// </summary>
        /// <param name="message">Bericht van de messagebox</param>
        /// <returns>Resultaat van de keuze van de gebruiker</returns>
        public static MessageBoxResult ReturnMessage(String message)
            => MessageBox.Show(message, "Yes or no", MessageBoxButton.YesNo, MessageBoxImage.Information);
    }
}
