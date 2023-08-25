using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BazaPodataka
{
    [ServiceContract]
    public interface IBazaPodataka
    {
        [OperationContract]

        void UpisUXmlBazu(List<Load> loadList, Audit audit, string nazivDatoteke);

        [OperationContract]

        void UpisUInMemoryBazu(List<Load> loadList, Audit audit, string nazivDatoteke);

        [OperationContract]
        void CitanjeXmlBaze(out List<Load> loadList);

    }
}
