using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Contract
{
    [ServiceContract]
    public interface IFileTransferService
    {
        [OperationContract]
        bool UploadString(string s);

        [OperationContract]
        Task<bool> UploadStringAsync(string s);
    }
}
