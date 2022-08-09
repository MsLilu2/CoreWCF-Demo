using Contract;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DesktopServer
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
