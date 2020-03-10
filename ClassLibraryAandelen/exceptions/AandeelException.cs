using System;

namespace ClassLibraryAandelen.exceptions
{
    public class AandeelException : Exception
    {
        public AandeelException(string message) : base(message) { }
    }
}
