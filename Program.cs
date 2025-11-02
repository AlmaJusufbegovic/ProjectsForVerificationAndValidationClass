using System;
using ForBooks.Data;
using ForBooks.Services;

namespace ForBooks
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DbClass();
            var korisnikServis = new KorisnikServis(db);
            var knjigaServis   = new KnjigaServis(db);

            while (true)
            {
                Console.WriteLine("\n GLAVNI MENI: ");
                Console.WriteLine("1. Registracija");
                Console.WriteLine("2. Potvrdi registraciju");
                Console.WriteLine("3. Prijava");
                Console.WriteLine("4. Upravljanje knjigama");
                Console.WriteLine("5. Izlaz");
                Console.Write("Odabir: ");
                string izbor = Console.ReadLine()!;

                switch (izbor)
                {
                    case "1": korisnikServis.Registracija(); break;
                    case "2": korisnikServis.PotvrdaRegistracije(); break;
                    case "3": korisnikServis.Prijava(); break;
                    case "4": PodmeniKnjige(knjigaServis); break;
                    case "5": Console.WriteLine("Doviđenja!"); return;
                    default: Console.WriteLine("Neispravan unos!"); break;
                }
            }
        }

        static void PodmeniKnjige(KnjigaServis s)
        {
            while (true)
            {
                Console.WriteLine("\n UPRAVLJANJE KNJIGAMA: ");
                Console.WriteLine("1. Dodaj knjigu");
                Console.WriteLine("2. Prikaži sve knjige");
                Console.WriteLine("3. Ažuriraj knjigu");
                Console.WriteLine("4. Obriši knjigu");
                Console.WriteLine("5. Obriši neprimjerene recenzije");
                Console.WriteLine("6. Sortiraj knjige po datumu");
                Console.WriteLine("7. Pretraži knjige (naslov/autor)");
                Console.WriteLine("8. Nazad");
                Console.Write("Odabir: ");
                string izbor = Console.ReadLine()!;

                switch (izbor)
                {
                    case "1": s.DodajKnjigu(); break;
                    case "2": s.PrikaziSveKnjige(); break;
                    case "3": s.AzurirajKnjigu(); break;
                    case "4": s.ObrisiKnjigu(); break;
                    case "5": s.ObrisiNeprimjereneRecenzije(); break;
                    case "6": s.SortirajPoDatumu(); break;
                    case "7": s.PretragaKnjiga(); break;
                    case "8": return;
                    default: Console.WriteLine("Neispravan unos."); break;
                }
            }
        }
    }
}

