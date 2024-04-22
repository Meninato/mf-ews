using Autofac;
using Flurl.Http;
using Flurl.Http.Configuration;
using Mf.Intr.Application.Services.SboServiceLayer;
using Mf.Intr.Application.Services.SboServiceLayer.Handlers;
using Mf.Intr.Core.Interfaces.Db;
using Mf.Intr.Core.Interfaces.Services.SboServiceLayer;
using Microsoft.Extensions.Logging;

namespace Mf.Intr.Application.Injection.SboServiceLayer;

public class SboServiceLayerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FlurlClientCache>().As<IFlurlClientCache>().SingleInstance();
        builder.RegisterType<FlurlBeforeCallHandler>();
        builder.RegisterType<FlurlOnErrorHandler>();
        builder.RegisterType<FlurlAfterCallHandler>();
        builder.RegisterType<FlurlOnRedirectHandler>();

        builder.Register(context =>
        {
            var slPool = new SboServiceLayerPoolService();

            var flurlClients = context.Resolve<IFlurlClientCache>();
            var unitOfWork = context.Resolve<IUnitOfWork>();

            var serviceLayers = unitOfWork.SboServiceLayerConnectionRepository.GetAll();

            if (serviceLayers.Any())
            {
                foreach (var sl in serviceLayers)
                {
                    var flurlClient = flurlClients.GetOrAdd(sl.Name, sl.Address, builder =>
                    {
                        builder.ConfigureInnerHandler(handler =>
                        {
                            handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
                        });
                        builder.ConfigureHttpClient(httpClient =>
                        {
                            httpClient.DefaultRequestHeaders.ExpectContinue = false;
                        });
                        builder.WithSettings(settings =>
                        {
                            settings.JsonSerializer = new DefaultJsonSerializer(AppDefaults.DefaultJsonSerializerOptions);
                        });
                        builder.EventHandlers.Add((FlurlEventType.BeforeCall, context.Resolve<FlurlBeforeCallHandler>()));
                        builder.EventHandlers.Add((FlurlEventType.OnError, context.Resolve<FlurlOnErrorHandler>()));
                        builder.EventHandlers.Add((FlurlEventType.AfterCall, context.Resolve<FlurlAfterCallHandler>()));
                        builder.EventHandlers.Add((FlurlEventType.OnRedirect, context.Resolve<FlurlOnRedirectHandler>()));
                    });

                    var logger = context.Resolve<ILogger<SboServiceLayerConnection>>();
                    slPool.Add(sl.Name, new SboServiceLayerConnection(flurlClient, logger, sl));
                }
            }

            return slPool;
        }).As<ISboServiceLayerPoolService>().SingleInstance();
    }
}
