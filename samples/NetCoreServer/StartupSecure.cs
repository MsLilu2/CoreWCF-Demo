using System;
using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Channels;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Contract;
using CoreWCF.Description;

namespace NetCoreServer
{
    public class StartupSecure
    {
        public const int NETTCP_PORT = 8090;
        // Only used on case that UseRequestHeadersForMetadataAddressBehavior is not used

        public void ConfigureServices(IServiceCollection services)
        {
            // Enable CoreWCF Services, enable metadata
            // Use the Url used to fetch WSDL as that service endpoint address in generated WSDL 
            services.AddServiceModelServices()
                    .AddServiceModelMetadata()
                    .AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseServiceModel(builder =>
            {
                var netTcpBinding = new NetTcpBinding(SecurityMode.Transport)
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

                // Add the Echo Service
                builder.AddService<EchoService>()
                .AddServiceEndpoint<EchoService, IEchoService>(binding, $"net.tcp://localhost:{NETTCP_PORT}/EchoService");

                builder.AddService<FileTransferService>()
                .AddServiceEndpoint<FileTransferService, IFileTransferService>(binding, $"net.tcp://localhost:{NETTCP_PORT}/FileTransferService");

                builder.AddService<ScriptExecutionService>()
                .AddServiceEndpoint<ScriptExecutionService, IScriptExecutionService>(binding, $"net.tcp://localhost:{NETTCP_PORT}/ScriptExecutionService");
                //// Configure WSDL to be available over http & https
                //var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
                //serviceMetadataBehavior.HttpGetEnabled = serviceMetadataBehavior.HttpsGetEnabled = true;
            });
        }
    }

}
