using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract
{
    [ServiceContract]
    public interface IScriptExecutionService
    {
        [OperationContract]
        Task<Guid> ExecuteScriptAsync(
           string script,
           Dictionary<string, object> namedArguments,
           object[] positionalArguments,
           string[] libraryScripts,
           string signedHash);
    }
}
