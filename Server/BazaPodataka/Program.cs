using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BazaPodataka
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost svc = new ServiceHost(typeof(ServisBaze));
            svc.Open();
            Console.WriteLine("Baza je pokrenuta!!!");

            Console.ReadLine();
        }
    }
}
