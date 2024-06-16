using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CentrumObslugiKartPlatniczych
{
    public class Archiwum
    {
        private string logFilePath = "archiwum.txt";
        private string firmyFilePath = "firmy.txt";
        private string bankiFilePath = "banki.txt";
        private string kartyFilePath = "karty.txt";
        private string transakcjeFilePath = "transakcje.txt";

        public List<Firma> OdczytajFirmy()
        {
            List<Firma> firmy = new List<Firma>();
            if (File.Exists(firmyFilePath))
            {
                foreach (var line in File.ReadAllLines(firmyFilePath))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2)
                    {
                        string typ = parts[1];
                        Firma nowaFirma = null;
                        switch (typ)
                        {
                            case "sklep":
                                nowaFirma = new Sklep { Nazwa = parts[0] };
                                break;
                            case "zakład usługowy":
                                nowaFirma = new ZakladUslugowy { Nazwa = parts[0] };
                                break;
                            case "firma transportowa":
                                nowaFirma = new FirmaTransportowa { Nazwa = parts[0] };
                                break;
                            default:
                                Console.WriteLine($"Nieznany typ firmy: {typ}");
                                break;
                        }

                        if (nowaFirma != null)
                        {
                            firmy.Add(nowaFirma);
                        }
                    }
                }
            }
            return firmy;
        }

        public List<Bank> OdczytajBanki()
        {
            List<Bank> banki = new List<Bank>();
            if (File.Exists(bankiFilePath))
            {
                foreach (var line in File.ReadAllLines(bankiFilePath))
                {
                    banki.Add(new Bank { Nazwa = line });
                }
            }
            return banki;
        }

        public List<Karta> OdczytajKarty()
        {
            List<Karta> karty = new List<Karta>();
            if (File.Exists(kartyFilePath))
            {
                foreach (var line in File.ReadAllLines(kartyFilePath))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 4)
                    {
                        if (parts[3] == "debetowa")
                        {
                            karty.Add(new KartaDebetowa { Numer = parts[0], Wlasciciel = parts[1], NazwaBanku = parts[2] });
                        }
                        else if (parts[3] == "kredytowa")
                        {
                            karty.Add(new KartaKredytowa { Numer = parts[0], Wlasciciel = parts[1], NazwaBanku = parts[2] });
                        }
                    }
                }
            }
            return karty;
        }

        public List<Transakcja> OdczytajTransakcje()
        {
            List<Transakcja> transakcje = new List<Transakcja>();
            if (File.Exists(transakcjeFilePath))
            {
                foreach (var line in File.ReadAllLines(transakcjeFilePath))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 7)
                    {
                        transakcje.Add(new Transakcja
                        {
                            NazwaFirmy = parts[0],
                            NazwaBanku = parts[1],
                            NumerKarty = parts[2],
                            Wlasciciel = parts[3],
                            Kwota = decimal.Parse(parts[4]),
                            Data = DateTime.Parse(parts[5]),
                            Autoryzowana = bool.Parse(parts[6])
                        });
                    }
                }
            }
            return transakcje;
        }

        public void ZapiszFirmy(List<Firma> firmy)
        {
            using (StreamWriter writer = new StreamWriter(firmyFilePath))
            {
                foreach (var firma in firmy)
                {
                    writer.WriteLine($"{firma.Nazwa};{firma.Typ}");
                }
            }
        }

        public void ZapiszBanki(List<Bank> banki)
        {
            using (StreamWriter writer = new StreamWriter(bankiFilePath))
            {
                foreach (var bank in banki)
                {
                    writer.WriteLine(bank.Nazwa);
                }
            }
        }

        public void ZapiszKarty(List<Karta> karty)
        {
            using (StreamWriter writer = new StreamWriter(kartyFilePath))
            {
                foreach (var karta in karty)
                {
                    string typKarty = karta is KartaDebetowa ? "debetowa" : "kredytowa";
                    writer.WriteLine($"{karta.Numer};{karta.Wlasciciel};{karta.NazwaBanku};{typKarty}");
                }
            }
        }

        public void ZapiszTransakcje(List<Transakcja> transakcje)
        {
            using (StreamWriter writer = new StreamWriter(transakcjeFilePath))
            {
                foreach (var transakcja in transakcje)
                {
                    writer.WriteLine($"{transakcja.NazwaFirmy};{transakcja.NazwaBanku};{transakcja.NumerKarty};{transakcja.Wlasciciel};{transakcja.Kwota};{transakcja.Data};{transakcja.Autoryzowana}");
                }
            }
        }

        public void LogAction(string action)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {action}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zapisu do pliku logu: {ex.Message}");
            }
        }
    }

    public class CentrumObslugiKartPlatniczych
    {
        private List<Firma> firmy;
        private List<Bank> banki;
        private List<Karta> karty;
        private List<Transakcja> transakcje;

        private Archiwum archiwum = new Archiwum();

        public CentrumObslugiKartPlatniczych()
        {
            firmy = archiwum.OdczytajFirmy();
            banki = archiwum.OdczytajBanki();
            karty = archiwum.OdczytajKarty();
            transakcje = archiwum.OdczytajTransakcje();
        }

        public void ZapiszDane()
        {
            archiwum.ZapiszFirmy(firmy);
            archiwum.ZapiszBanki(banki);
            archiwum.ZapiszKarty(karty);
            archiwum.ZapiszTransakcje(transakcje);
        }

        public void DodajFirme()
        {
            Console.WriteLine("Podaj nazwę firmy:");
            string nazwaFirmy = Console.ReadLine();

            Console.WriteLine("Wybierz typ firmy:");
            Console.WriteLine("1. Sklep");
            Console.WriteLine("2. Zakład usługowy");
            Console.WriteLine("3. Firma transportowa");

            string wyborTypu = Console.ReadLine();

            Firma nowaFirma = null;
            switch (wyborTypu)
            {
                case "1":
                    nowaFirma = new Sklep { Nazwa = nazwaFirmy };
                    break;
                case "2":
                    nowaFirma = new ZakladUslugowy { Nazwa = nazwaFirmy };
                    break;
                case "3":
                    nowaFirma = new FirmaTransportowa { Nazwa = nazwaFirmy };
                    break;
                default:
                    Console.WriteLine("Nieprawidłowy wybór.");
                    return;
            }

            if (nowaFirma != null)
            {
                firmy.Add(nowaFirma);
                archiwum.LogAction($"Dodano firmę: {nowaFirma.Nazwa}, {nowaFirma.Typ}");
            }
        }



        public void UsunFirme(string nazwaFirmy)
        {
            firmy.RemoveAll(f => f.Nazwa == nazwaFirmy);
            archiwum.LogAction($"Usunięto firmę: {nazwaFirmy}");
        }

        public void PrzegladajFirmy()
        {
            firmy.ForEach(f => Console.WriteLine($"{f.Nazwa}, {f.Typ}"));
        }

        public void DodajBank(Bank bank)
        {
            banki.Add(bank);
            archiwum.LogAction($"Dodano bank: {bank.Nazwa}");
        }

        public void UsunBank(string nazwaBanku)
        {
            banki.RemoveAll(b => b.Nazwa == nazwaBanku);
            archiwum.LogAction($"Usunięto bank: {nazwaBanku}");
        }

        public void PrzegladajBanki()
        {
            banki.ForEach(b => Console.WriteLine($"{b.Nazwa}"));
        }



        public void DodajKarte()
        {
            Console.WriteLine("Wybierz typ karty do dodania:");
            Console.WriteLine("1. Debetowa");
            Console.WriteLine("2. Kredytowa");

            string wybor = Console.ReadLine();

            Console.WriteLine("Podaj numer karty:");
            string numer = Console.ReadLine();
            Console.WriteLine("Podaj nazwę właściciela:");
            string wlasciciel = Console.ReadLine();

            Console.WriteLine("Podaj nazwę banku:");
            string nazwaBanku = Console.ReadLine();

            
            var istniejeBank = banki.Any(b => b.Nazwa == nazwaBanku);
            if (!istniejeBank)
            {
                Console.WriteLine($"Dodanie karty nie powiodło się. Bank o nazwie '{nazwaBanku}' nie istnieje.");
                return;
            }

            switch (wybor)
            {
                case "1":
                    karty.Add(new KartaDebetowa { Numer = numer, Wlasciciel = wlasciciel, NazwaBanku = nazwaBanku });
                    archiwum.LogAction($"Dodano kartę debetową: {numer}, {wlasciciel}, {nazwaBanku}");
                    break;
                case "2":
                    karty.Add(new KartaKredytowa { Numer = numer, Wlasciciel = wlasciciel, NazwaBanku = nazwaBanku });
                    archiwum.LogAction($"Dodano kartę kredytową: {numer}, {wlasciciel}, {nazwaBanku}");
                    break;
                default:
                    Console.WriteLine("Nieprawidłowy wybór.");
                    break;
            }
        }


        public void UsunKarte(string numerKarty)
        {
            karty.RemoveAll(k => k.Numer == numerKarty);
            archiwum.LogAction($"Usunięto kartę: {numerKarty}");
        }

        public void PrzegladajKarty()
        {
            karty.ForEach(k => Console.WriteLine($"{k.Numer}, {k.Wlasciciel}, {k.NazwaBanku}, {k.TypKarty}"));
        }

        public void ZadanoAutoryzacjiTransakcji(string nazwaFirmy, string numerKarty, decimal kwota)
        {
            var karta = karty.FirstOrDefault(k => k.Numer == numerKarty);
            if (karta != null)
            {
                bool autoryzowana = AutoryzujTransakcje(karta, kwota);
                var transakcja = new Transakcja
                {
                    NazwaFirmy = nazwaFirmy,
                    NazwaBanku = karta.NazwaBanku,
                    NumerKarty = karta.Numer,
                    Wlasciciel = karta.Wlasciciel,
                    Kwota = kwota,
                    Data = DateTime.Now,
                    Autoryzowana = autoryzowana
                };
                transakcje.Add(transakcja);
                archiwum.LogAction($"Zadano autoryzacji transakcji: Firma: {nazwaFirmy}, Karta: {numerKarty}, Kwota: {kwota}, Autoryzowana: {autoryzowana}");
            }
        }

        private bool AutoryzujTransakcje(Karta karta, decimal kwota)
        {
            if (karta is KartaKredytowa)
            {
                return kwota <= 1000; 
            }
            else if (karta is KartaDebetowa)
            {
                return kwota <= 500; 
            }
            return false;
        }

        public void PrzegladajTransakcje()
        {
            transakcje.ForEach(t => Console.WriteLine($"{t.NazwaFirmy}, {t.NazwaBanku}, {t.NumerKarty}, {t.Wlasciciel}, {t.Kwota}, {t.Data}, {t.Autoryzowana}"));
        }

        public void WyszukajTransakcje(Func<Transakcja, bool> kryteria)
        {
            var wynik = transakcje.Where(kryteria).ToList();
            wynik.ForEach(t => Console.WriteLine($"{t.NazwaFirmy}, {t.NazwaBanku}, {t.NumerKarty}, {t.Wlasciciel}, {t.Kwota}, {t.Data}, {t.Autoryzowana}"));
        }
        public void WyszukajTransakcjeZlozone(string nazwaFirmy, string nazwaBanku, string numerKarty, string wlasciciel, decimal? kwotaMin = null, decimal? kwotaMax = null)
        {
            var wynik = transakcje.Where(t =>
                (string.IsNullOrEmpty(nazwaFirmy) || t.NazwaFirmy == nazwaFirmy) &&
                (string.IsNullOrEmpty(nazwaBanku) || t.NazwaBanku == nazwaBanku) &&
                (string.IsNullOrEmpty(numerKarty) || t.NumerKarty == numerKarty) &&
                (string.IsNullOrEmpty(wlasciciel) || t.Wlasciciel == wlasciciel) &&
                (!kwotaMin.HasValue || t.Kwota >= kwotaMin.Value) &&
                (!kwotaMax.HasValue || t.Kwota <= kwotaMax.Value)
            ).ToList();

            wynik.ForEach(t => Console.WriteLine($"{t.NazwaFirmy}, {t.NazwaBanku}, {t.NumerKarty}, {t.Wlasciciel}, {t.Kwota}, {t.Data}, {t.Autoryzowana}"));
        }

    }

    public abstract class Firma
    {
        public string Nazwa { get; set; }
        public abstract string Typ { get; }
    }

    public class Sklep : Firma
    {
        public override string Typ => "sklep";
    }

    public class ZakladUslugowy : Firma
    {
        public override string Typ => "zakład usługowy";
    }

    public class FirmaTransportowa : Firma
    {
        public override string Typ => "firma transportowa";
    }

    public class Bank
    {
        public string Nazwa { get; set; }
        public List<Klient> Klienci { get; set; } = new List<Klient>();
    }

    public class Klient
    {
        public string Nazwa { get; set; }
        public string TypKarty { get; set; } 
    }

    public abstract class Karta
    {
        public string Numer { get; set; }
        public string Wlasciciel { get; set; }
        public string NazwaBanku { get; set; }
        public abstract string TypKarty { get; }
    }

    public class KartaDebetowa : Karta
    {
        public override string TypKarty => "debetowa";
    }

    public class KartaKredytowa : Karta
    {
        public override string TypKarty => "kredytowa";
    }

    public class Transakcja
    {
        public string NazwaFirmy { get; set; }
        public string NazwaBanku { get; set; }
        public string NumerKarty { get; set; }
        public string Wlasciciel { get; set; }
        public decimal Kwota { get; set; }
        public DateTime Data { get; set; }
        public bool Autoryzowana { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CentrumObslugiKartPlatniczych centrum = new CentrumObslugiKartPlatniczych();

            
            while (true)
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Dodaj firmę");
                Console.WriteLine("2. Usuń firmę");
                Console.WriteLine("3. Przeglądaj firmy");
                Console.WriteLine("4. Dodaj bank");
                Console.WriteLine("5. Usuń bank");
                Console.WriteLine("6. Przeglądaj banki");
                Console.WriteLine("7. Dodaj kartę");
                Console.WriteLine("8. Usuń kartę");
                Console.WriteLine("9. Przeglądaj karty");
                Console.WriteLine("10. Autoryzacja");
                Console.WriteLine("11. Przeglądaj transakcje");
                Console.WriteLine("12. Wyszukaj transakcje");
                Console.WriteLine("0. Wyjdź");


                string wybor = Console.ReadLine();
                switch (wybor)
                {
                    case "1":
                        centrum.DodajFirme();
                        break;

                    case "2":
                        Console.WriteLine("Podaj nazwę firmy do usunięcia:");
                        string nazwaFirmyUsun = Console.ReadLine();
                        centrum.UsunFirme(nazwaFirmyUsun);
                        break;
                    case "3":
                        centrum.PrzegladajFirmy();
                        break;
                    case "4":
                        Console.WriteLine("Podaj nazwę banku:");
                        string nazwaBanku = Console.ReadLine();
                        centrum.DodajBank(new Bank { Nazwa = nazwaBanku });
                        break;
                    case "5":
                        Console.WriteLine("Podaj nazwę banku do usunięcia:");
                        string nazwaBankuUsun = Console.ReadLine();
                        centrum.UsunBank(nazwaBankuUsun);
                        break;
                    case "6":
                        centrum.PrzegladajBanki();
                        break;
                    case "7":
                        centrum.DodajKarte();
                        break;
                    case "8":
                        Console.WriteLine("Podaj numer karty do usunięcia:");
                        string numerKartyUsun = Console.ReadLine();
                        centrum.UsunKarte(numerKartyUsun);
                        break;
                    case "9":
                        centrum.PrzegladajKarty();
                        break;
                    
                    case "10":
                        Console.WriteLine("Podaj nazwę firmy:");
                        string firmaTransakcja = Console.ReadLine();
                        Console.WriteLine("Podaj numer karty:");
                        string numerKartyTransakcja = Console.ReadLine(); 
                        Console.WriteLine("Podaj kwotę:");
                        decimal kwotaTransakcja;
                        while (!decimal.TryParse(Console.ReadLine(), out kwotaTransakcja))
                        {
                            Console.WriteLine("Nieprawidłowy format kwoty. Podaj ponownie:");
                        }
                        centrum.ZadanoAutoryzacjiTransakcji(firmaTransakcja, numerKartyTransakcja, kwotaTransakcja);
                        break;

                    case "11":
                        centrum.PrzegladajTransakcje();
                        break;
                    case "12":
                        Console.WriteLine("Podaj nazwę firmy:");
                        string firmaWyszukaj = Console.ReadLine();
                        Console.WriteLine("Podaj nazwę banku:");
                        string bankTransakcja = Console.ReadLine();
                        Console.WriteLine("Podaj numer karty:");
                        string numerKartyTransakcjaWyszukaj = Console.ReadLine(); 
                        Console.WriteLine("Podaj właściciela karty:");
                        string wlascicielKartyTransakcja = Console.ReadLine();
                        Console.WriteLine("Podaj minimalną kwotę:");
                        decimal kwotaMinTransakcja;
                        while (!decimal.TryParse(Console.ReadLine(), out kwotaMinTransakcja))
                        {
                            Console.WriteLine("Nieprawidłowy format kwoty. Podaj ponownie:");
                        }
                        Console.WriteLine("Podaj maksymalną kwotę:");
                        decimal kwotaMaxTransakcja;
                        while (!decimal.TryParse(Console.ReadLine(), out kwotaMaxTransakcja))
                        {
                            Console.WriteLine("Nieprawidłowy format kwoty. Podaj ponownie:");
                        }
                        centrum.WyszukajTransakcjeZlozone(firmaWyszukaj, bankTransakcja, numerKartyTransakcjaWyszukaj, wlascicielKartyTransakcja, kwotaMinTransakcja, kwotaMaxTransakcja);
                        break;

                    case "0":
                        centrum.ZapiszDane();
                        Console.WriteLine("Koniec programu.");
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór.");
                        break;
                }
            }
        }
    }
}
