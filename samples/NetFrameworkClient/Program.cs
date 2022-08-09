using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Contract;

namespace NetFrameworkClient
{
    public class Program
    {
        public const int NETTCP_PORT = 8089;

        static async Task Main(string[] args)
        {
            Console.Title = "WCF .Net Framework Client";

            var targetIp = "the ip of the server that your service is running on";
            await CallEchoService(targetIp);

            var guid = await CallScriptExecutionServiceAsync(targetIp);
            Console.WriteLine(guid);

            CallFileTransferService(targetIp);
        }

        private static async Task CallEchoService(string hostAddr)
        {
            IClientChannel channel = null;

            var netTcpBinding = new NetTcpBinding(SecurityMode.None)
            {
                TransferMode = TransferMode.Streamed,
                MaxReceivedMessageSize = Int64.MaxValue,
                MaxBufferSize = 1024 * 1024,
                ReaderQuotas =
                {
                    MaxDepth = 32,
                    MaxStringContentLength = 10485760,
                    MaxArrayLength = 16384,
                    MaxBytesPerRead = 4096,
                    MaxNameTableCharCount = 16384
                },
                SendTimeout = TimeSpan.FromHours(2),
                ReceiveTimeout = TimeSpan.FromHours(2),
                OpenTimeout = TimeSpan.FromHours(2),
                CloseTimeout = TimeSpan.FromHours(2),
            };
            var binding = new CustomBinding(netTcpBinding);

            var endpointFormat = "net.tcp://{0}:8089/EchoService";
            var factory = new ChannelFactory<IEchoService>(binding, new EndpointAddress(new Uri(string.Format(endpointFormat, hostAddr))));
            await Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, null);
            try
            {
                IEchoService client = factory.CreateChannel();
                channel = client as IClientChannel;
                await Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, null);
                var result = await client.Echo("Hello World!");
                await Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, null);
                Console.WriteLine(result);
            }
            finally
            {
                await Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, null);
            }
        }

        private static void CallFileTransferService(string hostAddr)
        {
            var targetAddress = IPAddress.Parse(hostAddr);
            using (var client = FileTransferClient.CreateInstance(targetAddress, SecurityMode.None))
            {
                var result = client.UploadString("Hello World!");
                Console.WriteLine(result);
            }
        }

        private static async Task<Guid> CallScriptExecutionServiceAsync(string hostAddr)
        {
            var targetAddress = IPAddress.Parse(hostAddr);
            using (var client = ScriptExecutionClient.CreateInstance(targetAddress, SecurityMode.None))
            {
                try
                {
                    Guid jobId = await client.ExecuteScriptAsync(
                        "Write-Host 'Hello World'",
                        null,
                        null,
                        null,
                        string.Empty);

                    return jobId;
                }
                catch (Exception exception)
                {
                    throw new Exception("Encountered retriable error: " + exception);
                }
            }
        }
    }
}