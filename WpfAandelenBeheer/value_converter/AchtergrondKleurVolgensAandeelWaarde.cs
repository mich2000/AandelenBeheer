using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfAandelenBeheer.value_converter
{
    public class AchtergrondKleurVolgensAandeelWaarde : IValueConverter
    {
        /// <summary>
        /// Volgens de waarde dat werd doorgegeven als parameter wordt er een specifieke kleur doorgegeven.
        /// negatieve waarde: rode kleur
        /// 0 - 20 %: gele kleur
        /// groter dan 20 %: groene kleur
        /// </summary>
        /// <returns>Een Brush</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double proc = (Double)value;
            if (proc >= 20) return Brushes.Green;
            else if (proc < 20 && proc >= 0) return Brushes.Gold;
            else return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
