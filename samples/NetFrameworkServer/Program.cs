using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace DesktopServer
{
    class Program
    {
        static void Main()
        {
            var host = new ServiceHost(typeof(EchoService));
            host.AddServiceEndpoint(typeof(Contract.IEchoService), GetBinding(SecurityMode.None), new Uri("net.tcp://localhost:8089/EchoService"));
            host.AddServiceEndpoint(typeof(Contract.IEchoService), GetBinding(SecurityMode.Transport), new Uri("net.tcp://localhost:8090/EchoService"));
            host.Open();
            LogHostUrls(host);

            var host1 = new ServiceHost(typeof(FileTransferService));
            host1.AddServiceEndpoint(typeof(Contract.IFileTransferService), GetBinding(SecurityMode.None), new Uri("net.tcp://localhost:8089/FileTransferService"));
            host1.AddServiceEndpoint(typeof(Contract.IFileTransferService), GetBinding(SecurityMode.Transport), new Uri("net.tcp://localhost:8090/FileTransferService"));
            host1.Open();
            LogHostUrls(host1);

            var host2 = new ServiceHost(typeof(ScriptExecutionService));
            host2.AddServiceEndpoint(typeof(Contract.IScriptExecutionService), GetBinding(SecurityMode.None), new Uri("net.tcp://localhost:8089/ScriptExecutionService"));
            host2.AddServiceEndpoint(typeof(Contract.IScriptExecutionService), GetBinding(SecurityMode.Transport), new Uri("net.tcp://localhost:8090/ScriptExecutionService"));
            host2.Open();
            LogHostUrls(host2);

            Console.WriteLine("Hit enter to close");
            Console.ReadLine();

            host.Close();
            host1.Close();
            host2.Close();
        }

        private static void LogHostUrls(ServiceHost host)
        {
            foreach (System.ServiceModel.Description.ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("Listening on " + endpoint.ListenUri.ToString());
            }
        }

        private static CustomBinding GetBinding(SecurityMode mode)
        {
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

            return binding;
        }

    }
}
