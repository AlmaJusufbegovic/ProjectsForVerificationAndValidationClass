using ForBooks.Data;
using ForBooks.Enums;
using ForBooks.Models;
using System;
using System.Collections.Generic;

namespace ForBooks.Services
{
    public class KnjigaServis
    {
        private readonly DbClass _db;

        public KnjigaServis(DbClass db)
        {
            _db = db;
        }

        // Pomoćne validacije 
        private bool IsNullOrWhite(string s)
        {
            if (s == null) return true;
            for (int i = 0; i < s.Length; i++)
                if (!char.IsWhiteSpace(s[i])) return false;
            return true;
        }

        private bool CaseInsensitiveContains(string haystack, string needle)
        {
            if (needle == null || haystack == null) return false;

            string a = haystack.ToLower();
            string b = needle.ToLower();

            // ručna implementacija "contains"
            for (int i = 0; i <= a.Length - b.Length; i++)
            {
                int k = 0;
                while (k < b.Length && a[i + k] == b[k]) k++;
                if (k == b.Length) return true;
            }
            return false;
        }

        // Dodaj knjigu 
        public void DodajKnjigu()
        {
            Console.WriteLine("\n Dodavanje nove knjige ");

            Console.Write("ISBN: ");
            string isbn = Console.ReadLine()!;
            if (IsNullOrWhite(isbn)) { Console.WriteLine("GREŠKA: Neispravan ISBN."); return; }

            Console.Write("Naziv knjige: ");
            string naziv = Console.ReadLine()!;
            if (IsNullOrWhite(naziv)) { Console.WriteLine("GREŠKA: Neispravan naziv."); return; }

            Console.Write("Datum objavljivanja (dd.mm.yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var datum))
            {
                Console.WriteLine(" GREŠKA: Neispravan format datuma. Pokušaj ponovo (npr. 15.03.2021)");
                return;
            }

            Console.WriteLine("Odaberi vrstu knjige (0-MEMOIR, 1-SELF_HELP, 2-POETRY, 3-NOVEL, 4-SHORT_STORIES, 5-NOVELLA): ");
            if (!int.TryParse(Console.ReadLine(), out var vrstaIndex) || vrstaIndex < 0 || vrstaIndex > 5)
            { Console.WriteLine(" GREŠKA: Neispravan izbor vrste."); return; }
            VrstaKnjige vrsta = (VrstaKnjige)vrstaIndex;

            Console.Write("Naziv izdavača: ");
            string izdavacNaziv = Console.ReadLine()!;
            if (IsNullOrWhite(izdavacNaziv)) { Console.WriteLine("GREŠKA: Neispravan izdavač."); return; }

            var izdavac = new Izdavac(_db.knjige.Count + 1, izdavacNaziv);

            // Unos autora 
            Console.Write("Koliko autora ima knjiga? ");
            if (!int.TryParse(Console.ReadLine(), out var brojAutora) || brojAutora <= 0)
            {
                Console.WriteLine("GREŠKA: Neispravan broj autora.");
                return;
            }

            var autoriKnjige = new List<Autor>();

            for (int i = 0; i < brojAutora; i++)
            {
                Console.WriteLine($"\n Autor {i + 1} ");
                Console.Write("Ime autora: ");
                string imeA = Console.ReadLine()!;
                Console.Write("Prezime autora: ");
                string prezA = Console.ReadLine()!;
                Console.Write("Datum rođenja (dd.mm.yyyy): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var datumA))
                {
                    Console.WriteLine("GREŠKA: Neispravan format datuma (npr. 09.10.1892). Autor preskočen.");
                    continue;
                }

                var autor = new Autor(_db.autori.Count + i + 1, imeA, prezA, datumA);
                autoriKnjige.Add(autor);
                _db.AddAutor(autor);
            }

            var nova = new Knjiga(_db.knjige.Count + 1, isbn, naziv, datum, vrsta, izdavac)
            {
                Autori = autoriKnjige
            };

            _db.AddKnjiga(nova);
            Console.WriteLine($" Knjiga '{naziv}' dodana i sačuvana.");
        }

        // Prikaz knjige 
        public void PrikaziSveKnjige()
        {
            Console.WriteLine("\n Sve knjige ");
            if (_db.knjige.Count == 0) { Console.WriteLine("Nema knjiga."); return; }

            for (int i = 0; i < _db.knjige.Count; i++)
            {
                var k = _db.knjige[i];
                Console.WriteLine($"{k.ID}. {k.NazivKnjige} | {k.ISBN} | {k.VrstaKnjige} | {k.DatumObjavljivanja:dd.MM.yyyy} | Izdavač: {k.Izdavac?.NazivIzdavaca}");

                if (k.Autori != null && k.Autori.Count > 0)
                {
                    Console.Write(" Autori: ");
                    for (int a = 0; a < k.Autori.Count; a++)
                    {
                        var au = k.Autori[a];
                        Console.Write($"{au.ImeAutora} {au.PrezimeAutora}");
                        if (a < k.Autori.Count - 1) Console.Write(", ");
                    }
                    Console.WriteLine();
                }
            }
        }

        // Azuriranje knjige 
        public void AzurirajKnjigu()
        {
            Console.WriteLine("\n Ažuriranje knjige ");
            Console.Write("Unesite ID knjige: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("GREŠKA: Neispravan ID."); return; }

            var k = _db.GetKnjiga(id);
            if (k == null) { Console.WriteLine("GREŠKA: Knjiga nije pronađena."); return; }

            Console.Write("Novi naziv (Enter za preskočiti): ");
            string n = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(n)) k.PromijeniNaziv(n);

            Console.Write("Novi ISBN (Enter za preskočiti): ");
            string i = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(i)) k.PromijeniISBN(i);

            Console.Write("Novi datum (dd.mm.yyyy, Enter za preskočiti): ");
            string d = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(d) && DateTime.TryParseExact(d, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out var noviDatum))
                k.DatumObjavljivanja = noviDatum;

            Console.Write("Novi izdavač (Enter za preskočiti): ");
            string iz = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(iz))
                k.Izdavac = new Izdavac(k.Izdavac?.ID ?? 1, iz);

            _db.UpdateKnjige();
            Console.WriteLine("Ažurirano i sačuvano.");
        }

        // Obriši knjigu 
        public void ObrisiKnjigu()
        {
            Console.WriteLine("\n Brisanje knjige ");
            Console.Write("Unesite ID knjige: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine(" Neispravan ID."); return; }

            var k = _db.GetKnjiga(id);
            if (k == null) { Console.WriteLine("GREŠKA: Knjiga nije pronađena."); return; }

            int idx = -1;
            for (int x = 0; x < _db.knjige.Count; x++)
                if (_db.knjige[x].ID == id) { idx = x; break; }

            if (idx >= 0)
            {
                _db.knjige.RemoveAt(idx);
                _db.UpdateKnjige();
                Console.WriteLine(" Knjiga obrisana i JSON ažuriran.");
            }
        }

        // Brisanje neprimjerenih recenzija 
        public void ObrisiNeprimjereneRecenzije()
        {
            Console.WriteLine("\n Brisanje neprimjerenih recenzija ");
            Console.Write("Unesite ID knjige: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("GREŠKA: Neispravan ID."); return; }

            var k = _db.GetKnjiga(id);
            if (k == null) { Console.WriteLine("GREŠKA: Knjiga nije pronađena."); return; }
            if (k.Recenzije == null || k.Recenzije.Count == 0) { Console.WriteLine("Nema recenzija."); return; }

            Console.Write("Unesi ključne riječi (npr. uvredljive), odvojene zarezom: ");
            string kljucne = Console.ReadLine() ?? "";
            var tokens = kljucne.Split(',', StringSplitOptions.RemoveEmptyEntries);

            int removed = 0;
            for (int i = k.Recenzije.Count - 1; i >= 0; i--)
            {
                var komentar = k.Recenzije[i].Komentar ?? "";
                bool neprimjeren = false;

                for (int t = 0; t < tokens.Length; t++)
                {
                    var token = tokens[t].Trim();
                    if (CaseInsensitiveContains(komentar, token))
                    { neprimjeren = true; break; }
                }

                if (neprimjeren) { k.Recenzije.RemoveAt(i); removed++; }
            }

            _db.UpdateKnjige();
            Console.WriteLine($" Uklonjeno {removed} recenzija.");
        }

        // Algoritmi

        // Bubble sort po datumu (najstarije - najnovije)
        public void SortirajPoDatumu()
        {
            Console.WriteLine("\n Sortiranje po datumu ");
            int n = _db.knjige.Count;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (_db.knjige[j].DatumObjavljivanja > _db.knjige[j + 1].DatumObjavljivanja)
                    {
                        var tmp = _db.knjige[j];
                        _db.knjige[j] = _db.knjige[j + 1];
                        _db.knjige[j + 1] = tmp;
                    }
                }
            }
            _db.UpdateKnjige();
            Console.WriteLine(" Sortirano i sačuvano.");
            PrikaziSveKnjige();
        }

        // Linearni search po naslovu ili autoru (bez LINQ-a)
        public void PretragaKnjiga()
        {
            Console.WriteLine("\n Pretraga knjiga ");
            Console.Write("Traži po (1) Naslovu ili (2) Autoru? ");
            string mod = Console.ReadLine() ?? "1";

            Console.Write("Unesi pojam: ");
            string pojam = Console.ReadLine() ?? "";
            if (IsNullOrWhite(pojam)) { Console.WriteLine("GREŠKA: Prazan pojam."); return; }

            int pronadjeno = 0;

            if (mod == "1")
            {
                for (int i = 0; i < _db.knjige.Count; i++)
                {
                    var k = _db.knjige[i];
                    if (CaseInsensitiveContains(k.NazivKnjige ?? "", pojam))
                    {
                        Console.WriteLine($"{k.ID}. {k.NazivKnjige} | {k.ISBN}");
                        pronadjeno++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _db.knjige.Count; i++)
                {
                    var k = _db.knjige[i];
                    if (k.Autori == null) continue;

                    bool match = false;
                    for (int a = 0; a < k.Autori.Count; a++)
                    {
                        var au = k.Autori[a];
                        string punoIme = $"{au.ImeAutora} {au.PrezimeAutora}";
                        if (CaseInsensitiveContains(punoIme, pojam))
                        { match = true; break; }
                    }

                    if (match)
                    {
                        Console.WriteLine($"{k.ID}. {k.NazivKnjige} | {k.ISBN}");
                        pronadjeno++;
                    }
                }
            }

            if (pronadjeno == 0) Console.WriteLine("Nema rezultata.");
        }
    }
}
