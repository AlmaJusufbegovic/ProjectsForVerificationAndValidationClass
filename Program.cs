using System;
using ForBooks;
using ForBooks.Services;

namespace ForBooks
{
    class Program
    {
        static void Main(string[] args)
        {
            var korisnikServis = new KorisnikServis();

            while (true)
            {
                Console.WriteLine("\n=== MENI ===");
                Console.WriteLine("1. Registracija");
                Console.WriteLine("2. Potvrdi registraciju");
                Console.WriteLine("3. Prijava");
                Console.WriteLine("4. Izlaz");
                Console.Write("Odabir: ");
                string izbor = Console.ReadLine()!;

                switch (izbor)
                {
                    case "1":
                        korisnikServis.Registracija();
                        break;
                    case "2":
                        korisnikServis.PotvrdaRegistracije();
                        break;
                    case "3":
                        korisnikServis.Prijava();
                        break;
                    case "4":
                        Console.WriteLine("Doviđenja!");
                        return;
                    default:
                        Console.WriteLine("Neispravan unos!");
                        break;
                }
            }
        }
    }
}
