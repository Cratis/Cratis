// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Kernel.Grains.EventSequences;
using Cratis.Kernel.Grains.Jobs;
using Cratis.Kernel.Grains.Observation;
using Cratis.Kernel.Grains.Recommendations;
using Cratis.Kernel.Storage.Observation;
using Orleans.Storage;

namespace Cratis.Kernel.Server;

/// <summary>
/// Extension methods for configuring storage for the Kernel.
/// </summary>
public static class StorageConfigurationExtensions
{
    /// <summary>
    /// Configure storage for the Kernel.
    /// </summary>
    /// <param name="builder"><see cref="ISiloBuilder"/> to configure for.</param>
    /// <returns><see cref="ISiloBuilder"/> for continuation.</returns>
    public static ISiloBuilder ConfigureStorage(this ISiloBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddKeyedSingleton<IGrainStorage>(WellKnownGrainStorageProviders.EventSequences, (serviceProvider, _) => serviceProvider.GetRequiredService<EventSequencesStorageProvider>());
            services.AddKeyedSingleton<IGrainStorage>(WellKnownGrainStorageProviders.Observers, (serviceProvider, _) => serviceProvider.GetRequiredService<ObserverGrainStorageProvider>());
            services.AddKeyedSingleton<IGrainStorage>(WellKnownGrainStorageProviders.FailedPartitions, (serviceProvider, _) => serviceProvider.GetRequiredService<FailedPartitionGrainStorageProvider>());
            services.AddKeyedSingleton<IGrainStorage>(WellKnownGrainStorageProviders.Jobs, (serviceProvider, _) => serviceProvider.GetRequiredService<JobGrainStorageProvider>());
            services.AddKeyedSingleton<IGrainStorage>(WellKnownGrainStorageProviders.JobSteps, (serviceProvider, _) => serviceProvider.GetRequiredService<JobStepGrainStorageProvider>());
            services.AddKeyedSingleton<IGrainStorage>(WellKnownGrainStorageProviders.Recommendations, (serviceProvider, _) => serviceProvider.GetRequiredService<RecommendationGrainStorageProvider>());
        });

        return builder;
    }
}
