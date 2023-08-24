using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaPodataka
{
    public class ServisBaze : IBazaPodataka
    {
        public void UpisUBazu(string text)
        {
            Console.WriteLine(text);
        }
    }
}
