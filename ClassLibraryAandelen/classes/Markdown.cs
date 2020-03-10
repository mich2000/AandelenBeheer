using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace ClassLibraryAandelen.classes
{
    public static class Markdown
    {
        /// <summary>
        /// Retourneert een markdown titel
        /// </summary>
        /// <param name="text">Markdown titel</param>
        /// <param name="MDlevel">titel niveau</param>
        /// <returns>markdown titel met een bepaalde titel niveau</returns>
        public static String MdTitel(this String text, int MDlevel = 1) => $"{new string('#', MDlevel)} {text}";

        //Retourneert ongeorderde lijst punt
        public static String UnOrdered(this String text) => $"* {text}";

        //horizontale lijn in markdown
        public static String HR => "***";
    }
}
