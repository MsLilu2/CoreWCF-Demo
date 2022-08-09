using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Contract
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
