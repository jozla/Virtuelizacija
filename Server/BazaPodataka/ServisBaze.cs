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
        public Dictionary<int, Audit> auditBaza = new Dictionary<int, Audit>();
        public Dictionary<int, ImportedFile> importedBaza = new Dictionary<int, ImportedFile>();
        public void UpisUBazu(List<Load> loadList, Audit audit, string nazivDatoteke, string tipBaze)
        {
            //kreiramo importedFile
            ImportedFile impf = new ImportedFile(audit.Id, nazivDatoteke);

            //ukoliko smo u podesavanjima izabrali xml bazu
            if (tipBaze.Equals("xml"))
            {
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
                            if(!noviLoad.ForecastValue.Equals("N/A"))
                                postojeciLoadElement.Element("FORECAST_VALUE").Value = noviLoad.ForecastValue;
                            if (!noviLoad.MeasuredValue.Equals("N/A"))
                                postojeciLoadElement.Element("MEASURED_VALUE").Value = noviLoad.MeasuredValue;
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

            //ukoliko smo u podesavanjima izabrali inMemory bazu
            if (tipBaze.Equals("inMemory"))
            {
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
                        postojeciLoad.ImportedFileId = noviLoad.ImportedFileId;
                        loadBaza[postojeciLoad.Id] = postojeciLoad;
                    }
                    //u suprotnom dodajemo novi objekat
                    else
                    {
                        loadBaza.Add(noviLoad.Id, noviLoad);
                    }
                }

                //upis audit i importedFile objekata
                auditBaza.Add(audit.Id, audit);
                importedBaza.Add(audit.Id, impf);
            }
        }
    }
}
