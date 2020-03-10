using ClassLibraryAandelen.classes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ClassLibraryAandelen.observables
{
    public class Portefeuillen : ObservableCollection<Portefeuillen>
    {
        public ObservableCollection<Portefeuille> GetPortefeuillen { get; set; }
        public Portefeuillen(ObservableCollection<Portefeuille> GetPortefeuillen)
        {
            CollectionChanged += Portefeuille_veranderingen;
            this.GetPortefeuillen = GetPortefeuillen;
        }

        private void Portefeuille_veranderingen(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Portefeuillen portefeuille in e.NewItems)
                    portefeuille.PropertyChanged += PortefeuilleVeranderdt;
        }

        private void PortefeuilleVeranderdt(object sender, PropertyChangedEventArgs e) 
            => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
