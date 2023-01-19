// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Configuration;
using Aksio.Cratis.Events.Store;
using Aksio.Cratis.Events.Store.EventSequences;
using Aksio.Cratis.Events.Store.MongoDB;
using Aksio.Cratis.Events.Store.MongoDB.Observation;
using Aksio.Cratis.Events.Store.MongoDB.Tenants;
using Aksio.Cratis.Events.Store.Observation;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using Orleans.Runtime;
using Orleans.Storage;

namespace Orleans.Hosting;

/// <summary>
/// Extension methods for <see cref="ISiloBuilder"/> for configuring event sequence stream.
/// </summary>
public static class SiloBuilderExtensions
{
    /// <summary>
    /// Add event sequence stream support.
    /// </summary>
    /// <param name="builder"><see cref="ISiloBuilder"/> to add for.</param>
    /// <returns><see cref="ISiloBuilder"/> for builder continuation.</returns>
    public static ISiloBuilder AddEventSequenceStream(this ISiloBuilder builder)
    {
        builder.AddMemoryGrainStorage("PubSubStore");
        builder.ConfigureServices(services =>
        {
            services.AddSingletonNamedService<IGrainStorage>(EventSequenceState.StorageProvider, (serviceProvider, _) => serviceProvider.GetService<EventSequencesStorageProvider>()!);
            services.AddSingletonNamedService<IGrainStorage>(ObserverState.StorageProvider, (serviceProvider, _) => serviceProvider.GetService<ObserverStorageProvider>()!);
            services.AddSingletonNamedService<IGrainStorage>(ClientObserversState.StorageProvider, (serviceProvider, _) => serviceProvider.GetService<ClientObserversStorageProvider>()!);
            services.AddSingletonNamedService<IGrainStorage>(TenantConfigurationState.StorageProvider, (serviceProvider, _) => serviceProvider.GetService<TenantConfigurationStorageProvider>()!);
        });

        builder.AddPersistentStreams(
            WellKnownProviders.EventSequenceStreamProvider,
            EventSequenceQueueAdapterFactory.Create,
            _ =>
            {
                _.Configure<HashRingStreamQueueMapperOptions>(ob => ob.Configure(options => options.TotalQueueCount = 8));
                _.ConfigureStreamPubSub();
            });
        return builder;
    }
}