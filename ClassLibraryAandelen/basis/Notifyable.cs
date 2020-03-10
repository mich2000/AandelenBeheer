using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClassLibraryAandelen.basis
{
    public class Notifyable : INotifyPropertyChanged
    {
        //event dat getriggert wordt wanneer er iets verandert
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// wordt gebruikt als je 1 property wilt laten weten dat er iets is verandert
        /// </summary>
        /// <param name="propertyName">property dat geupdatet worden, als deze leeg is wordt de callermembername</param>
        protected virtual void OnPropertyChanged([CallerMemberName]String propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// wordt gebruikt als je meerdere properties wilt laten weten dat er iets is verandert
        /// </summary>
        /// <param name="props">array properties that are going to update</param>
        public void NotifyProperties(params string[] props)
        {
            OnPropertyChanged();
            if(props!=null) foreach (string item in props) OnPropertyChanged(item);
        }
    }
}