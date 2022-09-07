// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Execution;
using Orleans;

namespace Aksio.Cratis.Events.Store.Grains;

/// <summary>
/// Defines a system for working with connecting microservices.
/// </summary>
/// <remarks>
/// Its responsibility is to setup the correct inbox consumer for the consuming microservice.
/// It will connect this based on producing microservice(s).
/// </remarks>
public interface IMicroserviceExchange : IGrainWithGuidKey
{
    /// <summary>
    /// Set a producing microservice and the event types it will be producing.
    /// </summary>
    /// <param name="microserviceId"><see cref="MicroserviceId"/> to register for.</param>
    /// <param name="eventTypes">Collection of <see cref="EventType">event types</see> to associate.</param>
    /// <returns>Awaitable task.</returns>
    Task SetProducerAndEventTypes(MicroserviceId microserviceId, IEnumerable<EventType> eventTypes);

    /// <summary>
    /// set a consuming microservice and the event types it will be consuming.
    /// </summary>
    /// <param name="microserviceId"><see cref="MicroserviceId"/> to register for.</param>
    /// <param name="eventTypes">Collection of <see cref="EventType">event types</see> to associate.</param>
    /// <returns>Awaitable task.</returns>
    Task SetConsumerAndEventTypes(MicroserviceId microserviceId, IEnumerable<EventType> eventTypes);
}
