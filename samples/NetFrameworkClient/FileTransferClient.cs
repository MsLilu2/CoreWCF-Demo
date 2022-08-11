using Contract;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace NetFrameworkClient
{
    public class FileTransferClient : ClientBase<IFileTransferService>, IFileTransferService
    {
        private FileTransferClient(Binding binding, EndpointAddress address)
            : base(binding, address)
        {
        }

        public const string FileTransferServiceEndPoint = "net.tcp://{0}:8089/FileTransferService";
        public const string FileTransferServiceEndPointSecure = "net.tcp://{0}:8090/FileTransferService";

        public bool UploadString(string s)
        {
            return this.UploadStringAsync(s).Result;
        }

        public Task<bool> UploadStringAsync(string s)
        {
            try
            {
                return this.Channel.UploadStringAsync(s);
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

        public static FileTransferClient CreateInstance(IPAddress remoteEndpoint, SecurityMode securityMode)
        {
            var netTcpBinding = new NetTcpBinding(securityMode)
            {
                //TransferMode = TransferMode.Streamed,
                //MaxReceivedMessageSize = Int64.MaxValue,
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

            if (securityMode == SecurityMode.Transport)
            {
                var endPointAddress_Secure = new EndpointAddress(new Uri(string.Format(FileTransferServiceEndPointSecure, remoteEndpoint)));
                return new FileTransferClient(binding, endPointAddress_Secure);
            }

            var endPointAddress = new EndpointAddress(new Uri(string.Format(FileTransferServiceEndPoint, remoteEndpoint)));
            return new FileTransferClient(binding, endPointAddress);
        }
    }
}
