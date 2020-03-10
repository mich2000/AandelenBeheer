using ClassLibraryAandelen.classes;
using ClassLibraryAandelen.exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;

namespace WpfAandelenBeheer
{
    public class AandelenRepo
    {
        AandelenContext ctx;
        public AandelenRepo()
        {
            ctx = new AandelenContext();
        }

        //============== EIGENAARS ========================
        public ObservableCollection<Eigenaar> Eigenaars => new ObservableCollection<Eigenaar>(ctx.Eigenaars.Include(e => e.Portefeuilles));

        public Eigenaar GetEigenaar(int ID) => ctx.Eigenaars.FirstOrDefault(e => e.Id == ID);

        public int GetEigenaarID(string naamGebruiker) => Eigenaars.FirstOrDefault(e => e.Naam == naamGebruiker).Id;

        public void AddEigenaar(String naamEigenaar = "")
        {
            if(naamEigenaar != "")
            {
                foreach (Eigenaar item in ctx.Eigenaars)
                {
                    if (naamEigenaar.Equals(item.Naam)) throw new DuplicateEigenaarException("Er mogen geen 2 eigenaars zijn met dezelfde naam.");
                }
                ctx.Eigenaars.Add(new Eigenaar(naamEigenaar));
                ctx.SaveChanges();
                AddLog(new Log(GetEigenaarID(naamEigenaar), LogOrigin.User, $"Nieuwe gebruiker met naam {naamEigenaar}."));
            }
        }

        //============== PORTEFEUILLES ========================
        public ObservableCollection<Portefeuille> GetPortefeuilles(int ID) => GetEigenaar(ID).Portefeuilles;

        public int AantalPortefeuilles(int ID) => ctx.Eigenaars.Where(e => e.Id == ID).SelectMany(p => p.Portefeuilles).Count();

        public void AddPortefeuille(int ID,String Naam)
        {
            if(Naam != "")
            {
                GetPortefeuilles(ID).Add(new Portefeuille(Naam));
                ctx.Logs.Add(new Log(ID, LogOrigin.Portefeuille, $"Portefeuille {Naam} is toegevoegd."));
                ctx.SaveChanges();
            }
        }

        public void RemovePortefeuille(int ID, Portefeuille portefeuille)
        {
            Portefeuille portefeuilleToBeDelted = GetPortefeuilles(ID).FirstOrDefault(p => p.ID == portefeuille.ID);
            if (portefeuilleToBeDelted != null)
            {
                GetPortefeuilles(ID).Remove(portefeuilleToBeDelted);
                ctx.Logs.Add(new Log(ID, LogOrigin.Portefeuille, $"Portefeuille {portefeuille.Naam} is verwijdert."));
                ctx.SaveChanges();
            }
        }

        public void UpdatePortefeuille(int ID, Portefeuille portefeuille, String nieuweNaam)
        {
            Portefeuille portefeuilleToBeUpdated = GetPortefeuilles(ID).FirstOrDefault(p => p.ID == portefeuille.ID);
            if(portefeuilleToBeUpdated != null)
            {
                portefeuilleToBeUpdated.Naam = nieuweNaam;
                ctx.Logs.Add(new Log(ID, LogOrigin.Portefeuille, $"Portefeuille {portefeuille.Naam} is gewijzigt naar {nieuweNaam}."));
                ctx.SaveChanges();
            }
        }
        //============== AANDELEN ========================
        public int AantalAandelen(int IdGebruiker)
        {
            return ctx.Eigenaars.Where(e => e.Id == IdGebruiker)
                .SelectMany(o => o.Portefeuilles)
                .SelectMany(o => o.Aandelen).Count();
        }

        public ObservableCollection<Aandeel> GetAandelen(int IdGebruiker, Portefeuille portefeuille)
        {
            return GetPortefeuilles(IdGebruiker).FirstOrDefault(p => p == portefeuille).Aandelen;
        }

        public void AddAandeel(int IdGebruiker, Portefeuille portefeuille, Aandeel aandeel)
        {
            GetPortefeuilles(IdGebruiker).FirstOrDefault(p => p.ID == portefeuille.ID).Aandelen.Add(aandeel);
            ctx.Logs.Add(new Log(IdGebruiker, LogOrigin.Aandelen, $"Aan portefeuille {portefeuille.Naam} wordt aandeel {aandeel.Bedrijfsnaam} toegevoegd."));
            ctx.SaveChanges();
        }

        public void RemoveAandeel(int IdGebruiker, Portefeuille portefeuille, Aandeel aandeel)
        {
            GetPortefeuilles(IdGebruiker).FirstOrDefault(p => p == portefeuille).Aandelen.Remove(aandeel);
            ctx.Logs.Add(new Log(IdGebruiker, LogOrigin.Aandelen, $"Aan portefeuille {portefeuille.Naam} wordt aandeel {aandeel.Bedrijfsnaam} verwijdert."));
            ctx.SaveChanges();
        }

        public void UpdateAandeel(int IdGebruiker, Portefeuille portefeuille, Aandeel aandeel, String BedrijfsNaam,
            Double BeginWaarde, Double ActueleWaarde)
        {
            Aandeel aandeelUpdate = GetPortefeuilles(IdGebruiker).FirstOrDefault(p => p == portefeuille).Aandelen
                .FirstOrDefault(a => a.ID == aandeel.ID);
            if (aandeelUpdate != null)
            {
                if (BedrijfsNaam != "" && aandeelUpdate.Bedrijfsnaam != BedrijfsNaam)
                {
                    ctx.Logs.Add(new Log(IdGebruiker, LogOrigin.Aandelen, $"Aandeel {aandeel.Bedrijfsnaam} wijzigt aan portefeuille {portefeuille.Naam}, " +
                        $"Aandeel's naam wordt {BedrijfsNaam}"));
                    aandeelUpdate.Bedrijfsnaam = BedrijfsNaam;
                }
                if (BeginWaarde != 0 && aandeelUpdate.BeginWaarde != BeginWaarde)
                {
                    ctx.Logs.Add(new Log(IdGebruiker, LogOrigin.Aandelen, $"Aandeel {aandeel.Bedrijfsnaam} wijzigt aan portefeuille {portefeuille.Naam}, " +
                        $"Aandeel's beginwaarde wordt {BeginWaarde}"));
                    aandeelUpdate.BeginWaarde = BeginWaarde;
                }
                if (ActueleWaarde != 0 && aandeelUpdate.ActueleWaarde != ActueleWaarde)
                {
                    ctx.Logs.Add(new Log(IdGebruiker, LogOrigin.Aandelen, $"Aandeel {aandeel.Bedrijfsnaam} wijzigt aan portefeuille {portefeuille.Naam}, " +
                        $"Aandeel's actuele waarde wordt {ActueleWaarde}"));
                    aandeelUpdate.ActueleWaarde = ActueleWaarde;
                }
                ctx.SaveChanges();
            }
        }

        //================= LOGS ================
        public IEnumerable<Log> LogLijst(int idGebruiker) => ctx.Logs.Where(l => l.IdGebruiker == idGebruiker);

        public void AddLog(Log log)
        {
            ctx.Logs.Add(log);
            ctx.SaveChanges();
        }
    }
}