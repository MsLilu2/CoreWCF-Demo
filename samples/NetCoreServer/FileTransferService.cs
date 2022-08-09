using Contract;
using CoreWCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FileTransferService : IFileTransferService
    {
        public bool UploadString(string s)
        {
            return DownloadString(s);
        }

        public Task<bool> UploadStringAsync(string s)
        {
            return Task.Run(() => DownloadString(s));
        }

        private bool DownloadString(string s)
        {
            System.Console.WriteLine($"FileTransferService: Received {s} from client!");
            return true;
        }

        public void Dispose()
        {
            // no-op; just needed to satisfy IFileTransferService
        }
    }
}
