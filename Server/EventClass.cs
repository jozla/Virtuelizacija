using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //delegat za upis proracunatih podataka u bazu
    public delegate void AzuriranjeHandler(List<Load> loadList, out string uspesnoUpisivanje);
   
    //klasa koja definise event
    public class EventClass
    {
        public event AzuriranjeHandler AzuriranjeEvent;

        //metoda za pokretanje eventa
        public void pokreniEvent(List<Load> loadList)
        {
            if (AzuriranjeEvent != null)
            {
                string uspesnoUpisivanje;
                AzuriranjeEvent(loadList, out uspesnoUpisivanje);
                Console.WriteLine(uspesnoUpisivanje);
            }
        }
    }
}
