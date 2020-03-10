using System;

namespace ClassLibraryAandelen.basis
{
    /// <summary>
    /// Klasse dat van de klasse Notifyable overerft.
    /// 
    /// property Titel: titel van de usercontrol
    /// </summary>
    public class BaseUserControl : Notifyable
    {
        private String _Titel;

        public String Titel
        {
            get { return _Titel; }
            set
            {
                if (_Titel != value) _Titel = value;
                OnPropertyChanged();
            }
        }

        public BaseUserControl() { }
    }
}
