﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [ServiceContract]
    public interface IServis
    {
        [OperationContract]
        void prijemDatoteke(MemoryStream datoteka, string nazivDatoteke);

        [OperationContract]
        void sviPodaciUcitani();

    }
}
