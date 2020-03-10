using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfAandelenBeheer.value_converter
{
    class AchtergrondKleurVolgensPortefeuilleWaarde : IValueConverter
    {
        /// <summary>
        /// Volgens de waarde dat werd doorgegeven als parameter wordt er een specifieke kleur doorgegeven.
        /// kleiner dan 500 euro: rode kleur
        /// Tussen 500 en 1000 euro: gele kleur
        /// Groter/gelijk aan 1000 euro: groene kleur
        /// </summary>
        /// <returns>Een Brush</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double totale_waarde = (Double)value;
            if (totale_waarde >= 1000) return Brushes.Green;
            else if (totale_waarde < 1000 && totale_waarde >= 500) return Brushes.Gold;
            else return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
