using System;

namespace ClassLibraryAandelen.exceptions
{
    public class DuplicateEigenaarException : Exception
    {
        public DuplicateEigenaarException(string message) : base(message) { }
    }
}
