using System.Data.Entity;

namespace ClassLibraryAandelen.classes
{
    class DropCreateDatabaseAlwaysAandelenContext : DropCreateDatabaseAlways<AandelenContext>
    {
        /**
         * Bij het opstarten van het programma zal de database aanmaken met al een eigenaar met een vooropgestelde naam.
         * De naam van de eigenaar kan verandert zijn door de menu item verander naam eigenaar
         * **/
        protected override   void Seed(AandelenContext context)
        {
            base.Seed(context);
            context.Eigenaars.Add(new Eigenaar());
            context.SaveChanges();
        }
    }
}
