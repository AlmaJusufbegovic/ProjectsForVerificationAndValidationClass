using ForBooks.Models;
using System.Collections.Generic;
using System.Linq;

namespace ForBooks.Data
{
    public class DbClass
    {
        public List<Korisnik> korisnici { get; set; }
        public List<Knjiga> knjige { get; set; }
        public List<Autor> autori { get; set; }
        public List<Recenzija> Recenzije { get; set; }


        public DbClass()
        {
            korisnici = FileHelper.LoadFromFile<Korisnik>("korisnici");
            knjige = FileHelper.LoadFromFile<Knjiga>("knjige");
            autori = FileHelper.LoadFromFile<Autor>("autori");
            Recenzije = FileHelper.LoadFromFile<Recenzija>("recenzije");

            foreach (var r in Recenzije)
            {
                r.Knjiga = knjige.FirstOrDefault(k => k.ID == r.KnjigaID);
                r.Korisnik = korisnici.FirstOrDefault(u => u.ID == r.KorisnikID);
            }

        }

        // Korisnici
        public void AddKorisnik(Korisnik korisnik)
        {
            korisnici.Add(korisnik);
            FileHelper.SaveToFile("korisnici", korisnici);
        }

        public Korisnik? GetKorisnik(int korisnikId)
        {
            return korisnici.FirstOrDefault(u => u.ID == korisnikId);
        }

        public void UpdateKorisnici()
        {
            FileHelper.SaveToFile("korisnici", korisnici);
        }

        // Knjige
        public void AddKnjiga(Knjiga knjiga)
        {
            knjige.Add(knjiga);
            FileHelper.SaveToFile("knjige", knjige);
        }

        public Knjiga? GetKnjiga(int knjigaId)
        {
            return knjige.FirstOrDefault(b => b.ID == knjigaId);
        }

        public void UpdateKnjige()
        {
            FileHelper.SaveToFile("knjige", knjige);
        }

        // Autori
        public void AddAutor(Autor autor)
        {
            autori.Add(autor);
            FileHelper.SaveToFile("autori", autori);
        }

        public Autor? GetAutor(int autorId)
        {
            return autori.FirstOrDefault(a => a.ID == autorId);
        }

        public void UpdateAutori()
        {
            FileHelper.SaveToFile("autori", autori);
        }
        // Recenzije
        public void AddRecenzija(Recenzija recenzija)
        {
            Recenzije.Add(recenzija);
            FileHelper.SaveToFile("recenzije", Recenzije);
        }

        public List<Recenzija> GetRecenzijeZaKnjigu(int knjigaId)
        {
            return Recenzije
                .Where(r => r.Knjiga != null && r.Knjiga.ID == knjigaId)
                .ToList();
        }

    }
}
