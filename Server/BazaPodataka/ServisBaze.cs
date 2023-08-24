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
        public void UpisUBazu(List<Load> loadList, Audit audit, int importedFileId, string nazivDatoteke)
        {
            //ukoliko smo u podesavanjima izabrali xml bazu
            if (ConfigurationManager.AppSettings["tipBaze"].Equals("xml"))
            {
                //uzimamo sve potrebne putanje
                string putanjaLoad = ConfigurationManager.AppSettings["tblLoad"];
                string putanjaAudit = ConfigurationManager.AppSettings["tblAudit"];
                string putanjaImported = ConfigurationManager.AppSettings["tblImported"];

                //kreiramo importedFile
                ImportedFile impf = new ImportedFile(importedFileId, nazivDatoteke);

                //ukoliko vec imamo datoteku u kojoj se cuvaju podaci
                if (File.Exists(putanjaLoad))
                {
                    //ucitamo xml dokument
                    XDocument loadDoc = XDocument.Load(putanjaLoad);

                    //za svaki dobijeni podatak
                    foreach (Load newLoad in loadList)
                    {
                        //ako postoji podatak u bazi koji ima isti timestamp
                        XElement existingLoadElement = loadDoc.Root.Elements("row")
                            .FirstOrDefault(elem => (DateTime)elem.Element("TIME_STAMP") == newLoad.Timestamp);

                        //azuriramo taj podatak
                        if (existingLoadElement != null)
                        {
                            existingLoadElement.Element("FORECAST_VALUE").Value = newLoad.ForecastValue;
                            existingLoadElement.Element("MEASURED_VALUE").Value = newLoad.MeasuredValue;
                            existingLoadElement.Element("ABSOLUTE_PERCENTAGE_DEVIATION").Value = newLoad.AbsolutePercentageDeviation;
                            existingLoadElement.Element("SQUARED_DEVIATION").Value = newLoad.SquaredDeviation;
                            existingLoadElement.Element("IMPORTED_FILE_ID").Value = newLoad.ImportedFileId.ToString();
                        }
                        else
                        {
                            // Dodamo novi element u suprotnom
                            XElement newLoadElement = newLoad.LoadToXElement();
                            loadDoc.Root.Add(newLoadElement);
                        }
                    }

                    loadDoc.Save(putanjaLoad);
                }
                //ako nema datoteke kreiramo novu
                else
                {
                    XDocument loadDoc = new XDocument(new XElement("rows"));

                    //sve podatke cuvamo u datoteku
                    foreach (Load newLoad in loadList)
                    {
                        XElement newLoadElement = newLoad.LoadToXElement();
                        loadDoc.Root.Add(newLoadElement);
                    }

                    loadDoc.Save(putanjaLoad);
                }
            }
        }
    }
}
