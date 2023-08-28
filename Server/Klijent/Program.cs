using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Klijent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IServis> factory = new ChannelFactory<IServis>("Servis");
            IServis channel = factory.CreateChannel();

            //koriste se da bi znali da li su svi potrebni podaci procitani
            bool forecast = false;
            bool measured = false;

            while (true)
            {
                Console.Write("Unesite putanju do foldera: ");
                string putanja = Console.ReadLine();

                //U slucaju da folder na toj putanji ne postoji ili nema csv datoteke
                //trazimo ponovni unos
                while (!Directory.Exists(putanja) || Directory.GetFiles(putanja, "*.csv").Length == 0)
                {
                    Console.WriteLine("Folder nije ispravan. Unesite ponovo:");
                    putanja = Console.ReadLine();
                }

                //sve csv datoteke u datom folderu
                string[] csvDatoteke = Directory.GetFiles(putanja, "*.csv");

                //svaku datoteku pojedinacno putem MemoryStream saljemo na server
                foreach (string csvDatoteka in csvDatoteke)
                {
                    try
                    {
                        using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(csvDatoteka)))
                        {
                            channel.prijemDatoteke(stream, Path.GetFileName(csvDatoteka));
                            if (Path.GetFileName(csvDatoteka).Contains("measured"))
                                measured = true;
                            if (Path.GetFileName(csvDatoteka).Contains("forecast"))
                                forecast = true;
                            Console.WriteLine($"Poslata datoteka: {Path.GetFileName(csvDatoteka)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Greška prilikom slanja datoteke {Path.GetFileName(csvDatoteka)}: {ex.Message}");
                    }
                }

                Console.WriteLine("Sve datoteke su poslate.\n\n");
                //ako smo ucitali sve potrebne podatke prelazimo na proracun
                if(measured && forecast)
                    channel.sviPodaciUcitani();
            }
        }
    }
}
