using ForBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForBooks.Data
{
    public class DbClass
    {
        public List<Korisnik> korisnici { get; set; }
        public List<Knjiga> knjige { get; set; }
        public List<Autor> autori { get; set; }

        public DbClass()
        {
            korisnici = new List<Korisnik>();
            knjige = new List<Knjiga>();
            autori = new List<Autor>();
        }

        public void AddKorisnik(Korisnik korisnik)
        {
            korisnici.Add(korisnik);
        }

        public void AddKnjiga(Knjiga loan)
        {
            knjige.Add(loan);
        }

        public void AddAutor(Autor payment)
        {
            autori.Add(payment);
        }

        public Korisnik GetKorisnik(int korisnikId)
        {
            return korisnici.FirstOrDefault(u => u.ID == korisnikId);
        }

        public Knjiga GetKnjiga(int knjigaId)
        {
            return knjige.FirstOrDefault(b => b.ID == knjigaId);
        }

        public Autor GetAutor(int autorId)
        {
            return autori.FirstOrDefault(a => a.ID == autorId);
        }
    }

}
