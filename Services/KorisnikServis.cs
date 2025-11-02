using ForBooks.Data;
using ForBooks.Models;
using System;

namespace ForBooks.Services
{
    public class KorisnikServis
    {
        private readonly DbClass _db;

        public KorisnikServis(DbClass db)
        {
            _db = db;
        }

        public void Registracija()
        {
            Console.WriteLine("\n Registracija: ");
            Console.Write("Ime: ");
            string ime = Console.ReadLine()!;
            Console.Write("Prezime: ");
            string prezime = Console.ReadLine()!;
            Console.Write("Korisničko ime: ");
            string username = Console.ReadLine()!;
            Console.Write("Email: ");
            string email = Console.ReadLine()!;
            Console.Write("Broj telefona: ");
            string telefon = Console.ReadLine()!;
            Console.Write("Lozinka: ");
            string lozinka = Console.ReadLine()!;

            // provjera postoji li email (ručno)
            for (int i = 0; i < _db.korisnici.Count; i++)
            {
                if (_db.korisnici[i].Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Korisnik sa ovim emailom već postoji!");
                    return;
                }
            }

            var korisnik = new Korisnik(_db.korisnici.Count + 1, ime, prezime, username, email, lozinka, telefon);
            _db.AddKorisnik(korisnik);

            Console.WriteLine($"Email za potvrdu poslan na {email} (simulacija).");
            Console.WriteLine("Potvrdite registraciju u roku od 5 minuta!");
        }

        public void PotvrdaRegistracije()
        {
            Console.WriteLine("\n Potvrda registracije: ");
            Console.Write("Unesite email: ");
            string email = Console.ReadLine()!;

            Korisnik? korisnik = null;
            for (int i = 0; i < _db.korisnici.Count; i++)
            {
                if (_db.korisnici[i].Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    korisnik = _db.korisnici[i];
                    break;
                }
            }

            if (korisnik == null)
            {
                Console.WriteLine("Korisnik nije pronađen.");
                return;
            }

            TimeSpan proslo = DateTime.Now - korisnik.VrijemeRegistracije;
            if (proslo.TotalMinutes <= 5)
            {
                korisnik.Potvrdjen = true;
                Console.WriteLine("Registracija uspješno potvrđena!");
            }
            else
            {
                // ukloni korisnika
                for (int i = 0; i < _db.korisnici.Count; i++)
                {
                    if (_db.korisnici[i].Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                    {
                        _db.korisnici.RemoveAt(i);
                        break;
                    }
                }
                Console.WriteLine("Vrijeme za potvrdu isteklo. Registracija poništena.");
            }

            _db.UpdateKorisnici();
        }

        public Korisnik? Prijava()
        {
            Console.WriteLine("\n Prijava: ");
            Console.Write("Email: ");
            string email = Console.ReadLine()!;
            Console.Write("Lozinka: ");
            string lozinka = Console.ReadLine()!;

            Korisnik? korisnik = null;
            for (int i = 0; i < _db.korisnici.Count; i++)
            {
                if (_db.korisnici[i].Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    korisnik = _db.korisnici[i];
                    break;
                }
            }

            if (korisnik == null)
            {
                Console.WriteLine("Korisnik ne postoji.");
                return null;
            }

            if (!korisnik.Potvrdjen)
            {
                Console.WriteLine("Registracija nije potvrđena.");
                return null;
            }

            if (korisnik.Lozinka != lozinka)
            {
                Console.WriteLine("Pogrešna lozinka.");
                return null;
            }

            Console.WriteLine($"Dobrodošli, {korisnik.Ime} {korisnik.Prezime}!");
            return korisnik;
        }
    }
}
