using System;

namespace ClassLibraryAandelen.basis
{
    public interface IMarkdownable
    {
        String GetReturnMarkdownDescription(Boolean includeOptions = true);
    }
}
