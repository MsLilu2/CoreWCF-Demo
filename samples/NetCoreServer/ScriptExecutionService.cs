using Contract;
using CoreWCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ScriptExecutionService : IScriptExecutionService
    {
        public Task<Guid> ExecuteScriptAsync(string script, Dictionary<string, object> namedArguments, object[] positionalArguments, string[] libraryScripts, string signedHash)
        {
            return Task.Run(() =>
            {
                var guid = Guid.NewGuid();
                Console.WriteLine($"ScriptExecutionService: Received {script} from client!");
                return guid;
            });
        }
    }
}
