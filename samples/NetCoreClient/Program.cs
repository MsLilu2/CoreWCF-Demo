using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Contract;

namespace NetCoreClient
{
    public class Program
    {
        public const int NETTCP_PORT = 8089;

        static async Task Main(string[] args)
        {
            Console.Title = "WCF .Net Core Client";
            var targetIp = "the ip of the server that your service is running on";

            //await CallEchoService(targetIp, SecurityMode.None);

            //var guid = await CallScriptExecutionServiceAsync(targetIp, SecurityMode.None);
            //Console.WriteLine(guid);

            //CallFileTransferService(targetIp, SecurityMode.None);

            await CallEchoService(targetIp, SecurityMode.Transport);

            var guid = await CallScriptExecutionServiceAsync(targetIp, SecurityMode.Transport);
            Console.WriteLine(guid);

            CallFileTransferService(targetIp, SecurityMode.Transport);
        }

        private static async Task CallEchoService(string hostAddr, SecurityMode mode)
        {
            IClientChannel channel = null;

            var netTcpBinding = new NetTcpBinding(mode)
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
            netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            var binding = new CustomBinding(netTcpBinding);
            var tcpTransport = binding.Elements.Find<TcpTransportBindingElement>();
            if (tcpTransport != null)
            {
                tcpTransport.ConnectionBufferSize = 1024 * 1024;
            }

            string endpointFormat;
            if (mode == SecurityMode.Transport)
            {
                endpointFormat = "net.tcp://{0}:8090/EchoService";
            }
            else
            {
                endpointFormat = "net.tcp://{0}:8089/EchoService";
            }

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

        private static void CallFileTransferService(string hostAddr, SecurityMode mode)
        {
            var targetAddress = IPAddress.Parse(hostAddr);
            using (var client = FileTransferClient.CreateInstance(targetAddress, mode))
            {
                var result = client.UploadString("Hello World!");
                Console.WriteLine(result);
            }
        }

        private static async Task<Guid> CallScriptExecutionServiceAsync(string hostAddr, SecurityMode mode)
        {
            var targetAddress = IPAddress.Parse(hostAddr);
            using (var client = ScriptExecutionClient.CreateInstance(targetAddress, mode))
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
