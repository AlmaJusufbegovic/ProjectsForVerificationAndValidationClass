using ForBooks.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForBooks.Models
{
    public class Izvjestaj
    {
        public int ID { get; set; }
        public Korisnik Korisnik { get; set; }
        public List<int> BrojStranicaNedovrsenihKnjiga { get; set; } = new();
        public string NajcitanijiZanr { get; set; }
        public List<Knjiga> PrijedloziNovihKnjiga { get; set; } = new();
        public VrstaIzvjestaja Vrsta { get; set; }

        public Izvjestaj(int id, Korisnik korisnik, VrstaIzvjestaja vrsta)
        {
            ID = id;
            Korisnik = korisnik;
            Vrsta = vrsta;
        }
    }
}
