using CoreWCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contract
{
    [ServiceContract]
    public interface IFileTransferService
    {
        bool UploadString(string s);

        [OperationContract]
        Task<bool> UploadStringAsync(string s);
    }
}
