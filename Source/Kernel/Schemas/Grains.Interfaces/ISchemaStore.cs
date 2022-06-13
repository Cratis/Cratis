// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Execution;
using NJsonSchema;
using Orleans;

namespace Aksio.Cratis.Events.Schemas.Grains;

/// <summary>
/// Defines the store for event schemas.
/// </summary>
public interface ISchemaStore : IGrainWithGuidKey
{
    /// <summary>
    /// Register a <see cref="JsonSchema"/> for a specific <see cref="EventType"/>.
    /// </summary>
    /// <param name="type"><see cref="EventType"/> to register for.</param>
    /// <param name="friendlyName">A friendly name to identify the event with.</param>
    /// <param name="schema"><see cref="JsonSchema"/> to register.</param>
    /// /// <returns>Async task.</returns>
    Task Register(EventType type, string friendlyName, string schema);

    /// <summary>
    /// Get the identifiers of microservices that produce a specific event type.
    /// </summary>
    /// <param name="type"><see cref="EventType"/> to get for.</param>
    /// <returns>Collection of <see cref="MicroserviceId"/>.</returns>
    /// <remarks>
    /// If there are none that produces the event type, it will return an empty collection.
    /// </remarks>
    Task<IEnumerable<MicroserviceId>> GetMicroservicesThatProduceEventType(EventType type);
}
