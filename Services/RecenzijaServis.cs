using ForBooks.Data;
using ForBooks.Enums;
using ForBooks.Models;
using System;
using System.Collections.Generic;

namespace ForBooks.Services
{
    public class RecenzijaServis
    {
        private readonly DbClass _db;

        public RecenzijaServis(DbClass db)
        {
            _db = db;
        }

        // Dodavanje recenzije
        public void DodajRecenziju()
        {
            Console.WriteLine("\nDodavanje nove recenzije");

            // Traži ime korisnika
            Console.Write("Unesite ime korisnika: ");
            string imeKorisnika = Console.ReadLine()?.Trim() ?? "";

            var korisnik = _db.korisnici
                .FirstOrDefault(k => k.Ime.Equals(imeKorisnika, StringComparison.OrdinalIgnoreCase));

            if (korisnik == null)
            {
                Console.WriteLine("Korisnik sa tim imenom nije pronađen u bazi.");
                return;
            }

            Console.Write("Unesite ID knjige: ");
          
             if (!int.TryParse(Console.ReadLine(), out int knjigaId))
             {
                 Console.WriteLine("Neispravan ID knjige.");
                 return;
             }
             var knjiga = _db.GetKnjiga(knjigaId);
             if (knjiga == null)
             {
                 Console.WriteLine("Knjiga s tim nazivom nije pronađena u bazi.");
                 return;
             }

            //Unos ocjene
            Console.Write("Ocjena (1–5): ");
            if (!double.TryParse(Console.ReadLine(), out double ocjena) || ocjena < 1 || ocjena > 5)
            {
                Console.WriteLine("Neispravna ocjena.");
                return;
            }

            //Unos komentara
            Console.Write("Komentar: ");
            string komentar = Console.ReadLine() ?? "";

            //Kreiranje nove recenzije
            var novaRecenzija = new Recenzija(_db.Recenzije.Count + 1, korisnik, knjiga, ocjena, komentar);
            _db.AddRecenzija(novaRecenzija);

            Console.WriteLine($"Recenzija dodana za knjigu '{knjiga.NazivKnjige}' od korisnika '{korisnik.Ime}'.");
        }


        // Prikaz svih recenzija
        public void PrikaziSveRecenzije()
        {
            Console.WriteLine("\nSve recenzije");

            if (_db.Recenzije.Count == 0)
            {
                Console.WriteLine("Nema recenzija u bazi.");
                return;
            }

            foreach (var r in _db.Recenzije)
            {
                Console.WriteLine($"{r.ID}. {r.Korisnik?.Ime ?? "?"} - '{r.Knjiga?.NazivKnjige ?? "?"}' | Ocjena: {r.Ocjena}/5");
                Console.WriteLine($"   Komentar: {r.Komentar}");
            }
        }

        // Izračun prosjeka ocjena za knjigu
        public void IzracunajProsjekOcjena()
        {
            Console.Write("\nUnesi ID knjige: ");
            if (!int.TryParse(Console.ReadLine(), out int knjigaId))
            {
                Console.WriteLine("Neispravan ID.");
                return;
            }

            var knjiga = _db.GetKnjiga(knjigaId);
            if (knjiga == null)
            {
                Console.WriteLine("Knjiga nije pronađena.");
                return;
            }

            var recenzije = _db.GetRecenzijeZaKnjigu(knjigaId);
            if (recenzije.Count == 0)
            {
                Console.WriteLine("Nema recenzija za ovu knjigu.");
                return;
            }

            double suma = 0;
            foreach (var r in recenzije) suma += r.Ocjena;

            double prosjek = suma / recenzije.Count;
            Console.WriteLine($"Prosječna ocjena za '{knjiga.NazivKnjige}' je: {prosjek:0.00}");
        }

        // Preporučene knjige
        public void PrikaziPreporuceneKnjige()
        {
            Console.Write("\nUnesi ID korisnika: ");
            if (!int.TryParse(Console.ReadLine(), out int korisnikId))
            {
                Console.WriteLine("Neispravan ID korisnika.");
                return;
            }

            var korisnik = _db.GetKorisnik(korisnikId);
            if (korisnik == null)
            {
                Console.WriteLine("Korisnik nije pronađen.");
                return;
            }

            var preporuke = PreporuciKnjige(korisnik);

            Console.WriteLine($"\nPreporučene knjige za {korisnik.Ime}:");
            if (preporuke.Count == 0)
            {
                Console.WriteLine("Nema preporuka za ovog korisnika.");
                return;
            }

            foreach (var k in preporuke)
                Console.WriteLine($"- {k.NazivKnjige} ({k.VrstaKnjige})");
        }

        // Brisanje recenzije po ID-u
        public void ObrisiRecenziju()
        {
            Console.Write("\nUnesi ID recenzije koju želiš obrisati: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Neispravan unos.");
                return;
            }

            var rec = _db.Recenzije.Find(r => r.ID == id);
            if (rec == null)
            {
                Console.WriteLine("Recenzija nije pronađena.");
                return;
            }

            _db.Recenzije.Remove(rec);
            _db.UpdateKnjige(); // ažurira json
            Console.WriteLine("Recenzija uspješno obrisana.");
        }

        // Algoritam za preporuku (ručno sortiranje sa petljama)
        public List<Knjiga> PreporuciKnjige(Korisnik korisnik)
        {
            var sveKnjige = _db.knjige ?? new List<Knjiga>();
            var recenzijeKorisnika = new List<Recenzija>();

            foreach (var r in _db.Recenzije)
            {
                if (r.Korisnik != null && r.Korisnik.ID == korisnik.ID)
                    recenzijeKorisnika.Add(r);
            }

            // Ako korisnik nema recenzija — top 3 po prosjeku
            if (recenzijeKorisnika.Count == 0)
            {
                var sveKnjigeKopija = new List<Knjiga>(sveKnjige);

                // RUČNO SORTIRANJE – Selection sort po prosječnoj ocjeni
                for (int i = 0; i < sveKnjigeKopija.Count - 1; i++)
                {
                    for (int j = i + 1; j < sveKnjigeKopija.Count; j++)
                    {
                        double prosjekI = IzracunajProsjekOcjenaInterno(sveKnjigeKopija[i].ID);
                        double prosjekJ = IzracunajProsjekOcjenaInterno(sveKnjigeKopija[j].ID);

                        if (prosjekJ > prosjekI)
                        {
                            var temp = sveKnjigeKopija[i];
                            sveKnjigeKopija[i] = sveKnjigeKopija[j];
                            sveKnjigeKopija[j] = temp;
                        }
                    }
                }

                var top3 = new List<Knjiga>();
                for (int i = 0; i < sveKnjigeKopija.Count && i < 3; i++)
                    top3.Add(sveKnjigeKopija[i]);

                return top3;
            }

            // Inače prema žanru korisnika
            var brojZanrova = new Dictionary<VrstaKnjige, int>();
            foreach (var r in recenzijeKorisnika)
            {
                if (r.Knjiga != null)
                {
                    var zanr = r.Knjiga.VrstaKnjige;
                    if (!brojZanrova.ContainsKey(zanr))
                        brojZanrova[zanr] = 1;
                    else brojZanrova[zanr]++;
                }
            }

            var omiljeni = VrstaKnjige.NOVEL;
            int max = 0;
            foreach (var kvp in brojZanrova)
                if (kvp.Value > max) { omiljeni = kvp.Key; max = kvp.Value; }

            var kandidati = new List<Knjiga>();
            foreach (var k in sveKnjige)
            {
                if (k.VrstaKnjige == omiljeni &&
                    !recenzijeKorisnika.Exists(r => r.Knjiga?.ID == k.ID))
                    kandidati.Add(k);
            }

            // RUČNO SORTIRANJE kandidata – Selection sort
            for (int i = 0; i < kandidati.Count - 1; i++)
            {
                for (int j = i + 1; j < kandidati.Count; j++)
                {
                    double prosjekI = IzracunajProsjekOcjenaInterno(kandidati[i].ID);
                    double prosjekJ = IzracunajProsjekOcjenaInterno(kandidati[j].ID);

                    if (prosjekJ > prosjekI)
                    {
                        var temp = kandidati[i];
                        kandidati[i] = kandidati[j];
                        kandidati[j] = temp;
                    }
                }
            }

            var preporuke = new List<Knjiga>();
            for (int i = 0; i < kandidati.Count && i < 3; i++)
                preporuke.Add(kandidati[i]);

            return preporuke;
        }

        // pomoćna metoda
        private double IzracunajProsjekOcjenaInterno(int knjigaId)
        {
            var recenzije = _db.GetRecenzijeZaKnjigu(knjigaId);
            if (recenzije.Count == 0) return 0;
            double suma = 0;
            foreach (var r in recenzije) suma += r.Ocjena;
            return suma / recenzije.Count;
        }
    }
}
