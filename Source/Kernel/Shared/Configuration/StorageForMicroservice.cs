// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Configuration;

/// <summary>
/// Represents all storage configurations for all <see cref="MicroserviceId">microservices</see> in the system.
/// </summary>
public class StorageForMicroservice
{
    /// <summary>
    /// The shared database connection configurations for the microservice.
    /// </summary>
    public StorageTypes Shared { get; init; } = [];

    /// <summary>
    /// The tenant specific configuration.
    /// </summary>
    public StorageForTenants Tenants { get; init; } = [];
}
