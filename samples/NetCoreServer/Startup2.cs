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
    public class Startup2
    {
        public const int HTTP_PORT = 8088;
        public const int HTTPS_PORT = 8443;
        public const int NETTCP_PORT = 8090;
        // Only used on case that UseRequestHeadersForMetadataAddressBehavior is not used
        public const string HOST_IN_WSDL = "localhost";

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
                netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

                var binding = new CustomBinding(netTcpBinding);
                var tcpTransport = binding.Elements.Find<TcpTransportBindingElement>();
                if (tcpTransport != null)
                {
                    tcpTransport.ConnectionBufferSize = 1024 * 1024;
                }

                // Add the Echo Service
                builder.AddService<EchoService>()
                //builder.AddService<EchoService>(serviceOptions =>
                //{
                //    // Set the default host name:port in generated WSDL and the base path for the address 
                //    serviceOptions.BaseAddresses.Add(new Uri($"http://{HOST_IN_WSDL}/EchoService"));
                //    serviceOptions.BaseAddresses.Add(new Uri($"https://{HOST_IN_WSDL}/EchoService"));
                //})
                //// Add a BasicHttpBinding endpoint
                //.AddServiceEndpoint<EchoService, IEchoService>(new BasicHttpBinding(), "/basichttp")
                //.AddServiceEndpoint<EchoService, IEchoService>(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "/basichttp")

                //// Add WSHttpBinding endpoints
                //.AddServiceEndpoint<EchoService, IEchoService>(new WSHttpBinding(SecurityMode.None), "/wsHttp")
                //.AddServiceEndpoint<EchoService, IEchoService>(new WSHttpBinding(SecurityMode.Transport), "/wsHttp")

                // Add NetTcpBinding
                .AddServiceEndpoint<EchoService, IEchoService>(binding, $"net.tcp://localhost:{NETTCP_PORT}/netTcp");
                //.ConfigureServiceHostBase<EchoService>(serviceHost => serviceHost.Credentials.ServiceCertificate.SetCertificate());

                //// Configure WSDL to be available over http & https
                //var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
                //serviceMetadataBehavior.HttpGetEnabled = serviceMetadataBehavior.HttpsGetEnabled = true;
            });
        }
    }

}
