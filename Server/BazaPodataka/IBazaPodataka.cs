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

        void UpisUBazu(List<Load> loadList, Audit audit, int importedFileId, string nazivDatoteke, string tipBaze);
    }
}
