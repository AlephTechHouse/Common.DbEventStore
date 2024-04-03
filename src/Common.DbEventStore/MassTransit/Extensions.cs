using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common.DbEventStore.Settings;
using Common.DbEventStore.Settings.Service;
using Common.DbEventStore.Settings.RabbitMQ;
using Common.DbEventStore.Helpers;

namespace Common.DbEventStore.MassTransit
{
    public static class Extensions
    {
        private const string ServiceSettingsSectionName = nameof(ServiceSettings);
        private const string RabbitMQSettingsSectionName = nameof(RabbitMQSettings);

        public static IServiceCollection AddMassTransitWithRabbitMq(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var serviceSettings = configuration.GetConfiguration<ServiceSettings>(ServiceSettingsSectionName);
            var rabbitMQSettings = configuration.GetConfiguration<RabbitMQSettings>(RabbitMQSettingsSectionName);

            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly());

                configure.UsingRabbitMq((context, configurator) =>
                {
                    configurator.Host(rabbitMQSettings.Host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    configurator.UseMessageRetry(retryConfigigurator =>
                    {
                        retryConfigigurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            return services;
        }
    }
};