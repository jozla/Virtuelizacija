using BazaPodataka;
using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Servis : IServis
    {
        private int importedFileId = 100; // Inicijalni ID za importedFile i Audit
        private int loadId = 0;     //inicijalni ID za load
        //koriste se da bi znali da li su svi potrebni podaci procitani
        private bool forecast = false;
        private bool measured = false;
        public void prijemDatoteke(MemoryStream datoteka, string nazivDatoteke)
        {
            using (StreamReader reader = new StreamReader(datoteka))
            {
                List<string> lines = new List<string>();
                Audit audit;
                List<Load> loadList = new List<Load>();


                //procitamo sve redove iz datoteke
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                //ukoliko broj redova nije odgovarajuci greska i kreiramo audit objekat
                if (lines.Count < 23 || lines.Count > 25)
                {
                    audit = new Audit(importedFileId, DateTime.Now, MsgType.Error, $"U datoteci {nazivDatoteke} nalazi se neodgovarajući broj redova: {lines.Count}");
                    importedFileId++;
                }

                //u suprotnom za svaki red obradjujemo podatke i kreiramo listu audit objekata
                else
                {
                    //zbog razlike u datotekama
                    if (lines[0].Split(',')[0].Equals("TIME_STAMP"))
                        lines.RemoveAt(0);

                    loadList = new List<Load>();
                    
                    //ako je forecast datoteka upisujemo forecast vrednost
                    //u novi load objekat
                    if (nazivDatoteke.Contains("forecast"))
                    {
                        foreach (string line in lines)
                        {
                            //izgled reda u datoteci datum,vreme,vrednost
                            //parsiramo po zarezu i dalje rukujemo podacima
                            string[] parts = line.Split(',');

                            if (parts.Length >= 3)
                            {
                                DateTime timestamp;
                                if (DateTime.TryParse(parts[0] + " " + parts[1], out timestamp))
                                {
                                    string forecastValue = parts[2];
                                    string measuredValue = "N/A";
                                    string absolutePercentageDeviation = "N/A";
                                    string squaredDeviation = "N/A";

                                    Load load = new Load(loadId, timestamp, forecastValue, measuredValue, absolutePercentageDeviation, squaredDeviation, importedFileId);
                                    loadList.Add(load);
                                    loadId++;
                                }
                            }
                        }
                        //uspesno smo poslali forecast podatke
                        forecast = true;
                    }

                    //ako je measured datoteka upisujemo measured vrednost
                    //u novi load objekat
                    if (nazivDatoteke.Contains("measured"))
                    {
                        foreach (string line in lines)
                        {
                            //izgled reda u datoteci datum vreme,vrednost
                            //parsiramo po zarezu i dalje rukujemo podacima
                            string[] parts = line.Split(',');

                            if (parts.Length >= 2)
                            {
                                string[] timestampParts = parts[0].Split(' ');
                                DateTime timestamp;
                                if (DateTime.TryParse(timestampParts[0] + " " + timestampParts[1], out timestamp))
                                {
                                    string forecastValue = "N/A";
                                    string measuredValue = parts[1];
                                    string absolutePercentageDeviation = "N/A";
                                    string squaredDeviation = "N/A";

                                    Load load = new Load(loadId, timestamp, forecastValue, measuredValue, absolutePercentageDeviation, squaredDeviation, importedFileId);
                                    loadList.Add(load);
                                    loadId++;
                                }
                            }
                        }
                        //uspesno smo poslali measured podatke
                        measured = true;
                    }
                    //audit o uspesnom kreiranju datoteke
                    audit = new Audit(importedFileId, DateTime.Now, MsgType.Info, $"Datoteka {nazivDatoteke} je uspesno procitana");
                    importedFileId++;
                }

                //saljemo potrebne podatke radi upisa u bazu
                ChannelFactory<IBazaPodataka> factory = new ChannelFactory<IBazaPodataka>("BazaPodataka");
                IBazaPodataka channel = factory.CreateChannel();

                //u odnosu na izabrani tip baze u App.config fajlu pozivamo metodu za upis
                if(ConfigurationManager.AppSettings["tipBaze"].Equals("xml"))
                    channel.UpisUXmlBazu(loadList, audit, nazivDatoteke);

                if (ConfigurationManager.AppSettings["tipBaze"].Equals("inMemory"))
                    channel.UpisUInMemoryBazu(loadList, audit, nazivDatoteke);

                //kada znamo da su upisane i forecast i measured vrednosti
                if (forecast && measured)
                {
                    //citamo podatke iz odgovarajuce baze
                    if (ConfigurationManager.AppSettings["tipBaze"].Equals("xml"))
                    {
                        channel.CitanjeXmlBaze(out List<Load> procitaniPodaci);
                    }
                }
            }
        }
    }
}
