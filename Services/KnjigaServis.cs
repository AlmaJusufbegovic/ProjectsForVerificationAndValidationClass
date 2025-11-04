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
            Console.WriteLine("\n Sve knjige");
            if (_db.knjige.Count == 0)
            {
                Console.WriteLine("Nema knjiga.");
                return;
            }

            for (int i = 0; i < _db.knjige.Count; i++)
            {
                var k = _db.knjige[i];

                Console.WriteLine($"{k.ID}. {k.NazivKnjige} | {k.ISBN} | {k.VrstaKnjige} | {k.DatumObjavljivanja:dd.MM.yyyy} | Izdavač: {k.Izdavac?.NazivIzdavaca}");

                if (k.Autori != null && k.Autori.Count > 0)
                {
                    Console.Write("Autori: ");
                    for (int j = 0; j < k.Autori.Count; j++)
                    {
                        var a = k.Autori[j];
                        Console.Write($"{a.ImeAutora} {a.PrezimeAutora}");
                        if (j < k.Autori.Count - 1)
                            Console.Write(", ");
                    }
                    Console.WriteLine(); // novi red nakon autora
                }

                Console.WriteLine(); // razmak između knjiga
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
        

        // Preporuci knjigu 
        public void PredloziKnjiguMeni()
        {
            Console.WriteLine("Predloži mi knjigu\n");
            Console.WriteLine("Za personalizovane preporuke, molimo prijavite se:\n");

            Console.Write("Email: ");
            string email = Console.ReadLine()!;
            if (IsNullOrWhite(email))
            {
                Console.WriteLine("GREŠKA: Email ne može biti prazan.");
                return;
            }

            Console.Write("Šifra: ");
            string sifra = Console.ReadLine()!;
            if (IsNullOrWhite(sifra))
            {
                Console.WriteLine("GREŠKA: Šifra ne može biti prazna.");
                return;
            }

            // Pronađi korisnika
            Korisnik? korisnik = null;
            for (int i = 0; i < _db.korisnici.Count; i++)
            {
                if (_db.korisnici[i].Email == email && _db.korisnici[i].Lozinka == sifra)
                {
                    korisnik = _db.korisnici[i];
                    break;
                }
            }

            if (korisnik == null)
            {
                Console.WriteLine("\n Pogrešan email ili šifra. Pokušaj ponovo.");
                return;
            }

            Console.WriteLine($"\n Dobro došao/la, {korisnik.Ime} {korisnik.Prezime}!\n");

            // Pozovi funkciju za preporuke
            PreporuciKnjige(korisnik.ID);
        }

        // Preporuka knjiga za prijavljenog korisnika
        private void PreporuciKnjige(int korisnikId)
        {
            Console.WriteLine("═══════════════════════════════════════════════════\n");

            // Pronađi knjige koje je korisnik pročitao (na osnovu recenzija)
            var procitaneKnjige = new List<Knjiga>();
            for (int i = 0; i < _db.knjige.Count; i++)
            {
                var knjiga = _db.knjige[i];
                if (knjiga.Recenzije != null)
                {
                    for (int j = 0; j < knjiga.Recenzije.Count; j++)
                    {
                        if (knjiga.Recenzije[j].Korisnik?.ID == korisnikId)
                        {
                            procitaneKnjige.Add(knjiga);
                            break;
                        }
                    }
                }
            }

            if (procitaneKnjige.Count == 0)
            {
                Console.WriteLine("Nemate pročitanih knjiga u sistemu.");
                Console.WriteLine("Evo najpopularnijih knjiga:\n");
                PreporuciNajpopularnije();
                return;
            }

            Console.WriteLine($" Analiziram pročitane knjiga...\n");

            // Lista preporuka: [indeks_knjige, skor]
            var preporuke = new List<int[]>();
            var razloziPreporuka = new List<List<string>>();

            for (int i = 0; i < _db.knjige.Count; i++)
            {
                var knjiga = _db.knjige[i];

                // Preskoči već pročitane knjige
                bool procitana = false;
                for (int p = 0; p < procitaneKnjige.Count; p++)
                {
                    if (procitaneKnjige[p].ID == knjiga.ID)
                    {
                        procitana = true;
                        break;
                    }
                }
                if (procitana) continue;

                // Izračunaj sličnost
                int skor = 0;
                var razlozi = new List<string>();

                for (int p = 0; p < procitaneKnjige.Count; p++)
                {
                    var procitana_knjiga = procitaneKnjige[p];

                    
                    if (knjiga.VrstaKnjige == procitana_knjiga.VrstaKnjige)
                    {
                        skor += 3;
                        string razlog = $"Isti tip: {knjiga.VrstaKnjige}";
                        bool vec_postoji = false;
                        for (int r = 0; r < razlozi.Count; r++)
                        {
                            if (razlozi[r] == razlog) { vec_postoji = true; break; }
                        }
                        if (!vec_postoji) razlozi.Add(razlog);
                    }

                
                    if (knjiga.Autori != null && procitana_knjiga.Autori != null)
                    {
                        for (int a1 = 0; a1 < knjiga.Autori.Count; a1++)
                        {
                            for (int a2 = 0; a2 < procitana_knjiga.Autori.Count; a2++)
                            {
                                if (knjiga.Autori[a1].ID == procitana_knjiga.Autori[a2].ID)
                                {
                                    skor += 5;
                                    string autor = $"{knjiga.Autori[a1].ImeAutora} {knjiga.Autori[a1].PrezimeAutora}";
                                    string razlog = $"Autor: {autor}";
                                    bool vec_postoji = false;
                                    for (int r = 0; r < razlozi.Count; r++)
                                    {
                                        if (razlozi[r] == razlog) { vec_postoji = true; break; }
                                    }
                                    if (!vec_postoji) razlozi.Add(razlog);
                                }
                            }
                        }
                    }

                   
                    int razlikaGodina = knjiga.DatumObjavljivanja.Year - procitana_knjiga.DatumObjavljivanja.Year;
                    if (razlikaGodina < 0) razlikaGodina = -razlikaGodina;

                    if (razlikaGodina <= 3)
                    {
                        skor += 2;
                        string razlog = "Slična godina izdavanja";
                        bool vec_postoji = false;
                        for (int r = 0; r < razlozi.Count; r++)
                        {
                            if (razlozi[r] == razlog) { vec_postoji = true; break; }
                        }
                        if (!vec_postoji) razlozi.Add(razlog);
                    }

                   
                    if (knjiga.Izdavac != null && procitana_knjiga.Izdavac != null &&
                        knjiga.Izdavac.NazivIzdavaca == procitana_knjiga.Izdavac.NazivIzdavaca)
                    {
                        skor += 1;
                        string razlog = $"Izdavač: {knjiga.Izdavac.NazivIzdavaca}";
                        bool vec_postoji = false;
                        for (int r = 0; r < razlozi.Count; r++)
                        {
                            if (razlozi[r] == razlog) { vec_postoji = true; break; }
                        }
                        if (!vec_postoji) razlozi.Add(razlog);
                    }
                }

                if (skor > 0)
                {
                    preporuke.Add(new int[] { i, skor });
                    razloziPreporuka.Add(razlozi);
                }
            }

            // Sortiraj preporuke po skoru - bubble sort (silazno)
            for (int i = 0; i < preporuke.Count - 1; i++)
            {
                for (int j = 0; j < preporuke.Count - i - 1; j++)
                {
                    if (preporuke[j][1] < preporuke[j + 1][1])
                    {
                        var tmp = preporuke[j];
                        preporuke[j] = preporuke[j + 1];
                        preporuke[j + 1] = tmp;

                        var tmpRazlozi = razloziPreporuka[j];
                        razloziPreporuka[j] = razloziPreporuka[j + 1];
                        razloziPreporuka[j + 1] = tmpRazlozi;
                    }
                }
            }

            // Prikaži preporuke
            if (preporuke.Count > 0)
            {
                Console.WriteLine(" PERSONALIZOVANE PREPORUKE ZA VAS \n");
                int prikazano = 0;
                for (int i = 0; i < preporuke.Count && prikazano < 5; i++)
                {
                    int knjigaIndex = preporuke[i][0];
                    var knjiga = _db.knjige[knjigaIndex];
                    var razlozi = razloziPreporuka[i];

                    Console.WriteLine($"───────────────────────────────────────────────────");
                    Console.WriteLine($"{prikazano + 1}.  {knjiga.NazivKnjige}");
                    Console.WriteLine($"   ISBN: {knjiga.ISBN}");
                    Console.WriteLine($"   Tip: {knjiga.VrstaKnjige}");
                    Console.WriteLine($"   Objavljeno: {knjiga.DatumObjavljivanja:dd.MM.yyyy}");

                    if (knjiga.Autori != null && knjiga.Autori.Count > 0)
                    {
                        Console.Write("     Autori: ");
                        for (int a = 0; a < knjiga.Autori.Count; a++)
                        {
                            Console.Write($"{knjiga.Autori[a].ImeAutora} {knjiga.Autori[a].PrezimeAutora}");
                            if (a < knjiga.Autori.Count - 1) Console.Write(", ");
                        }
                        Console.WriteLine();
                    }

                    Console.WriteLine($"\n    Zašto preporučujemo:");
                    for (int r = 0; r < razlozi.Count; r++)
                        Console.WriteLine($"      • {razlozi[r]}");

                    Console.WriteLine();
                    prikazano++;
                }
                Console.WriteLine("═══════════════════════════════════════════════════");
            }
            else
            {
                Console.WriteLine("Nema knjiga sa sličnim karakteristikama.");
                Console.WriteLine("Evo najpopularnijih knjiga:\n");
                PreporuciNajpopularnije();
            }
        }

        // Preporuči najpopularnije knjige (po broju recenzija)
        private void PreporuciNajpopularnije()
        {
            // Lista: [indeks_knjige, broj_recenzija]
            var popularnost = new List<int[]>();

            for (int i = 0; i < _db.knjige.Count; i++)
            {
                var knjiga = _db.knjige[i];
                int brojRecenzija = knjiga.Recenzije?.Count ?? 0;
                popularnost.Add(new int[] { i, brojRecenzija });
            }

            // Bubble sort po broju recenzija (silazno)
            for (int i = 0; i < popularnost.Count - 1; i++)
            {
                for (int j = 0; j < popularnost.Count - i - 1; j++)
                {
                    if (popularnost[j][1] < popularnost[j + 1][1])
                    {
                        var tmp = popularnost[j];
                        popularnost[j] = popularnost[j + 1];
                        popularnost[j + 1] = tmp;
                    }
                }
            }

            // Prikaži top 5
            Console.WriteLine("🔥 TOP 5 NAJPOPULARNIJIH KNJIGA 🔥\n");
            bool imaPopularnih = false;

            for (int i = 0; i < popularnost.Count && i < 5; i++)
            {
                int brojRecenzija = popularnost[i][1];
                if (brojRecenzija == 0) break;

                imaPopularnih = true;
                int knjigaIndex = popularnost[i][0];
                var knjiga = _db.knjige[knjigaIndex];

                Console.WriteLine($"───────────────────────────────────────────────────");
                Console.WriteLine($"{i + 1}. 📕 {knjiga.NazivKnjige}");
                Console.WriteLine($"   ISBN: {knjiga.ISBN}");
                Console.WriteLine($"   Tip: {knjiga.VrstaKnjige}");
                Console.WriteLine($"   📊 Recenzije: {brojRecenzija}");

                if (knjiga.Autori != null && knjiga.Autori.Count > 0)
                {
                    Console.Write("   ✍️  Autori: ");
                    for (int a = 0; a < knjiga.Autori.Count; a++)
                    {
                        Console.Write($"{knjiga.Autori[a].ImeAutora} {knjiga.Autori[a].PrezimeAutora}");
                        if (a < knjiga.Autori.Count - 1) Console.Write(", ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            if (!imaPopularnih)
            {
                Console.WriteLine("Još nema knjiga sa recenzijama u sistemu.");
            }
            else
            {
                Console.WriteLine("═══════════════════════════════════════════════════");
            }
        }


    }
}
