using ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace NetFrameworkClient
{
    public class ScriptExecutionClient : ClientBase<IScriptExecutionService>
    {
        private ScriptExecutionClient(Binding binding, EndpointAddress address)
            : base(binding, address)
        {
        }

        public const string FileTransferServiceEndPoint = "net.tcp://{0}:8089/ScriptExecutionService";

        public async Task<Guid> ExecuteScriptAsync(
            string script,
            Dictionary<string, object> namedArguments,
            object[] positionalArguments,
            string[] libraryScripts,
            string signedHash)
        {
            try
            {
                return await this.Channel.ExecuteScriptAsync(
                    script,
                    namedArguments,
                    positionalArguments,
                    libraryScripts,
                    signedHash);
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

        public static ScriptExecutionClient CreateInstance(IPAddress remoteEndpoint, SecurityMode securityMode)
        {
            var binding = new NetTcpBinding(securityMode)
            {
                TransferMode = TransferMode.Streamed,
                MaxReceivedMessageSize = Int64.MaxValue,
                MaxBufferSize = 65536,
                ReaderQuotas =
                {
                    MaxStringContentLength = 10485760,
                    MaxArrayLength = 16384,
                    MaxBytesPerRead = 4096,
                    MaxNameTableCharCount = 16384
                },
                SendTimeout = TimeSpan.FromMinutes(5),
                ReceiveTimeout = TimeSpan.FromMinutes(5),
                OpenTimeout = TimeSpan.FromMinutes(5),
                CloseTimeout = TimeSpan.FromMinutes(5),
            };

            EndpointAddress endPointAddress = new EndpointAddress(new Uri(string.Format(FileTransferServiceEndPoint, remoteEndpoint)));

            return new ScriptExecutionClient(binding, endPointAddress);
        }

    }
}
