using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BazaPodataka
{
    public class ServisBaze : IBazaPodataka
    {
        public static Dictionary<int, Load> loadBaza = new Dictionary<int, Load>();
        public static Dictionary<int, Audit> auditBaza = new Dictionary<int, Audit>();
        public static Dictionary<int, ImportedFile> importedBaza = new Dictionary<int, ImportedFile>();
 
        public void UpisUXmlBazu(List<Load> loadList, Audit audit, string nazivDatoteke)
        {
            ImportedFile impf = null;
            //kreiramo imported file ako ucitavamo datoteke
            //u suportnom ne kreiramo zato sto samo azuriramo load bazu
            if (!nazivDatoteke.Equals(""))
                impf = new ImportedFile(audit.Id, nazivDatoteke);
            

            //uzimamo sve potrebne putanje
            string putanjaLoad = ConfigurationManager.AppSettings["tblLoad"];
            string putanjaAudit = ConfigurationManager.AppSettings["tblAudit"];
            string putanjaImported = ConfigurationManager.AppSettings["tblImported"];

            //ukoliko vec imamo datoteku u kojoj se cuvaju podaci
            if (File.Exists(putanjaLoad))
            {
                //ucitamo xml dokument
                XDocument loadDoc = XDocument.Load(putanjaLoad);

                //za svaki dobijeni podatak
                foreach (Load noviLoad in loadList)
                {
                    //ako postoji podatak u bazi koji ima isti timestamp
                    XElement postojeciLoadElement = loadDoc.Root.Elements("row")
                        .FirstOrDefault(elem => (DateTime)elem.Element("TIME_STAMP") == noviLoad.Timestamp);

                    //azuriramo taj podatak
                    if (postojeciLoadElement != null)
                    {
                        if (!noviLoad.ForecastValue.Equals("N/A"))
                            postojeciLoadElement.Element("FORECAST_VALUE").Value = noviLoad.ForecastValue;
                        if (!noviLoad.MeasuredValue.Equals("N/A"))
                            postojeciLoadElement.Element("MEASURED_VALUE").Value = noviLoad.MeasuredValue;
                        if (!noviLoad.AbsolutePercentageDeviation.Equals("N/A"))
                            postojeciLoadElement.Element("ABSOLUTE_PERCENTAGE_DEVIATION").Value = noviLoad.AbsolutePercentageDeviation;
                        if (!noviLoad.SquaredDeviation.Equals("N/A"))
                            postojeciLoadElement.Element("SQUARED_DEVIATION").Value = noviLoad.SquaredDeviation;
                        postojeciLoadElement.Element("IMPORTED_FILE_ID").Value = noviLoad.ImportedFileId.ToString();
                    }
                    else
                    {
                        // Dodamo novi element u suprotnom
                        XElement noviLoadElement = noviLoad.LoadToXElement();
                        loadDoc.Root.Add(noviLoadElement);
                    }
                }

                loadDoc.Save(putanjaLoad);
            }
            //ako nema datoteke kreiramo novu
            else
            {
                XDocument loadDoc = new XDocument(new XElement("rows"));

                //sve podatke cuvamo u datoteku
                foreach (Load noviLoad in loadList)
                {
                    XElement noviLoadElement = noviLoad.LoadToXElement();
                    loadDoc.Root.Add(noviLoadElement);
                }

                loadDoc.Save(putanjaLoad);
            }

            //ako ucitavamo datoteku onda imamo upis u audit i importedFile bazu
            if (audit != null && impf != null)
            {
                //ako postoji audit xml datoteka ucitavamo je i dodajemo novi element
                if (File.Exists(putanjaAudit))
                {
                    XDocument auditDoc = XDocument.Load(putanjaAudit);

                    XElement noviAuditElement = audit.AuditToXElement();
                    auditDoc.Root.Add(noviAuditElement);

                    auditDoc.Save(putanjaAudit);
                }
                //ukoliko ne postoji pravimo novu
                else
                {
                    XDocument auditDoc = new XDocument(new XElement("STAVKE"));

                    XElement noviAuditElement = audit.AuditToXElement();
                    auditDoc.Root.Add(noviAuditElement);

                    auditDoc.Save(putanjaAudit);
                }

                //isto kao kod audit datoteke ako postoji ucitavamo je i dodajemo
                if (File.Exists(putanjaImported))
                {
                    XDocument importedDoc = XDocument.Load(putanjaImported);


                    XElement noviImportedFileElement = impf.ImportedToXElement();
                    importedDoc.Root.Add(noviImportedFileElement);

                    importedDoc.Save(putanjaImported);
                }
                //ukoliko ne postoji pravimo novu
                else
                {
                    XDocument importedDoc = new XDocument(new XElement("STAVKE"));

                    XElement noviImportedFileElement = impf.ImportedToXElement();
                    importedDoc.Root.Add(noviImportedFileElement);

                    importedDoc.Save(putanjaImported);
                }
            }           
    }
        public void UpisUInMemoryBazu(List<Load> loadList, Audit audit, string nazivDatoteke)
        {
            //kreiramo imported file ako ucitavamo datoteke
            //u suportnom ne kreiramo zato sto samo azuriramo load bazu
            ImportedFile impf = null;
            if (!nazivDatoteke.Equals(""))
                impf = new ImportedFile(audit.Id, nazivDatoteke);

            foreach (Load noviLoad in loadList)
            {
                //provera da li postjoi objekat sa istim timestamp
                Load postojeciLoad = loadBaza.Values.FirstOrDefault(l => l.Timestamp == noviLoad.Timestamp);

                //ukoliko vec imamo objekat sa tim timestamp azuriramo samo njegovu vrednost
                if (postojeciLoad != null)
                {
                    if (!noviLoad.ForecastValue.Equals("N/A"))
                        postojeciLoad.ForecastValue = noviLoad.ForecastValue;
                    if (!noviLoad.MeasuredValue.Equals("N/A"))
                        postojeciLoad.MeasuredValue = noviLoad.MeasuredValue;
                    if (!noviLoad.AbsolutePercentageDeviation.Equals("N/A"))
                        postojeciLoad.AbsolutePercentageDeviation = noviLoad.AbsolutePercentageDeviation;
                    if (!noviLoad.SquaredDeviation.Equals("N/A"))
                        postojeciLoad.SquaredDeviation = noviLoad.SquaredDeviation;
                    postojeciLoad.ImportedFileId = noviLoad.ImportedFileId;

                    loadBaza[postojeciLoad.Id] = postojeciLoad;
                }
                //u suprotnom dodajemo novi objekat
                else
                {
                    loadBaza.Add(noviLoad.Id, noviLoad);
                }
            }

            //ako ucitavamo datoteku onda imamo upis u audit i importedFile bazu
            if (audit != null && impf != null)
            {
                //upis audit i importedFile objekata
                auditBaza.Add(audit.Id, audit);
                importedBaza.Add(audit.Id, impf);
            }
        }

        //inicijalni id za load objekte
        private int xmlId = 0;
        public void CitanjeXmlBaze(out List<Load> loadList)
        {
            //putanja do tabele sa load objektima
            string putanjaLoad = ConfigurationManager.AppSettings["tblLoad"];
            
            //ucitamo dokument
            XDocument loadDoc = XDocument.Load(putanjaLoad);

            loadList = new List<Load>();

            //prodjemo kroz svaki red i napravimo od podataka novi load objekat
            foreach (XElement rowElement in loadDoc.Descendants("row"))
            {
                Load load = new Load
                    (
                    xmlId,
                    DateTime.Parse(rowElement.Element("TIME_STAMP").Value),
                    rowElement.Element("FORECAST_VALUE").Value,
                    rowElement.Element("MEASURED_VALUE").Value,
                    rowElement.Element("ABSOLUTE_PERCENTAGE_DEVIATION").Value,
                    rowElement.Element("SQUARED_DEVIATION").Value,
                    int.Parse(rowElement.Element("IMPORTED_FILE_ID").Value)
                    );

                //dodamo objekat u listu koju vracamo na server
                loadList.Add(load);
                xmlId++;
            }
        }

        public void CitanjeInMemoryBaze(out List<Load> loadList)
        {
            loadList = new List<Load>();
            foreach(Load podatak in loadBaza.Values)
            {
                loadList.Add(podatak);
            }
        }
    }
}
