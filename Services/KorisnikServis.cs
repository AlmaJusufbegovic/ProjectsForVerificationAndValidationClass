using ForBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForBooks.Services
{
    public class KorisnikServis
    {
        private readonly Dictionary<string, Korisnik> korisnici = new();

        public void Registracija()
        {
            Console.WriteLine("\n--- Registracija ---");
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

            if (korisnici.ContainsKey(email))
            {
                Console.WriteLine("Korisnik sa ovim emailom već postoji!");
                return;
            }

            var korisnik = new Korisnik(korisnici.Count + 1, ime, prezime, username, email, lozinka, telefon);
            korisnici[email] = korisnik;

            Console.WriteLine($"Email za potvrdu poslan na {email} (simulacija).");
            Console.WriteLine("Potvrdite registraciju u roku od 5 minuta!");
        }

        public void PotvrdaRegistracije()
        {
            Console.WriteLine("\n--- Potvrda registracije ---");
            Console.Write("Unesite email: ");
            string email = Console.ReadLine()!;

            if (!korisnici.TryGetValue(email, out var korisnik))
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
                korisnici.Remove(email);
                Console.WriteLine("Vrijeme za potvrdu isteklo. Registracija poništena.");
            }
        }

        public Korisnik? Prijava()
        {
            Console.WriteLine("\n--- Prijava ---");
            Console.Write("Email: ");
            string email = Console.ReadLine()!;
            Console.Write("Lozinka: ");
            string lozinka = Console.ReadLine()!;

            if (!korisnici.TryGetValue(email, out var korisnik))
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
