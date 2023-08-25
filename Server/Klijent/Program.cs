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
                            Console.WriteLine($"Poslata datoteka: {Path.GetFileName(csvDatoteka)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Greška prilikom slanja datoteke {Path.GetFileName(csvDatoteka)}: {ex.Message}");
                    }
                }

                Console.WriteLine("Sve datoteke su poslate.\n\n");
            }
            //Console.ReadLine();
        }
    }
}
