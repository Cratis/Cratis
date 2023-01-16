// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Aksio.Cratis.Configuration;
using Aksio.Cratis.Execution;
using Aksio.Cratis.Timers;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Clients;

/// <summary>
/// Extension methods for configuring the Cratis Kernel.
/// </summary>
public static class ClientServiceProviderExtensions
{
    /// <summary>
    /// Add the Cratis Kernel client.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add to.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    /// <returns><see cref="IServiceCollection"/> for continuation.</returns>
    public static IServiceCollection AddCratisClient(this IServiceCollection services, ILogger logger)
    {
        services.AddSingleton<IClient>(_ =>
        {
            var configuration = _.GetRequiredService<ClientConfiguration>();
            var httpClientFactory = _.GetRequiredService<IHttpClientFactory>();
            var serializerOptions = _.GetRequiredService<JsonSerializerOptions>();
            serializerOptions = new JsonSerializerOptions(serializerOptions);
            var singleKernelClientLogger = _.GetRequiredService<ILogger<SingleKernelClient>>();
            var webSocketConnectionLogger = _.GetRequiredService<ILogger<WebSocketConnection>>();
            var server = _.GetRequiredService<IServer>();
            var addresses = server.Features.Get<IServerAddressesFeature>();
            var clientLifecycle = _.GetRequiredService<IClientLifecycle>();
            var executionContextManager = _.GetRequiredService<IExecutionContextManager>();
            var timerFactory = _.GetRequiredService<ITimerFactory>();

            var options = configuration.GetSingleKernelOptions();
            if (options.AdvertisedClientEndpoint is null && addresses!.Addresses.Count == 0)
            {
                throw new UnableToResolveClientUri();
            }
            var clientEndpoint = options.AdvertisedClientEndpoint ?? new Uri(addresses!.Addresses.First());

            var client = configuration.ClusterType switch
            {
                ClusterType.Single => new SingleKernelClient(
                    httpClientFactory,
                    timerFactory,
                    executionContextManager,
                    options,
                    clientEndpoint,
                    clientLifecycle,
                    serializerOptions,
                    singleKernelClientLogger,
                    webSocketConnectionLogger),
                _ => throw new UnknownClusterType()
            };

            logger.ConnectingToKernel();
            client.Connect().Wait();
            logger.ConnectedToKernel();

            return client;
        });

        return services;
    }

    /// <summary>
    /// Adds a inside silo Kernel client.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add to.</param>
    /// <returns><see cref="IServiceCollection"/> for continuation.</returns>
    public static IServiceCollection AddCratisInsideSiloClient(this IServiceCollection services)
    {
        services.AddSingleton<IClient, InsideSiloClient>();
        return services;
    }
}