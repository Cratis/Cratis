// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#nullable disable

using Aksio.Cratis.Execution;

namespace Aksio.Cratis.Events.Store;

/// <summary>
/// Represents the state used by the microservice exchange.
/// </summary>
public class MicroserviceExchangeState
{
    /// <summary>
    /// The name of the storage provider used for working with this type of state.
    /// </summary>
    public const string StorageProvider = "microservice-exchange-state";

    /// <summary>
    /// Gets or sets the producing microservices and the event types they produce.
    /// </summary>
    public Dictionary<MicroserviceId, IEnumerable<EventType>> ProducersAndEventTypes { get; set; } = new();

    /// <summary>
    /// Gets or sets the consuming microservices and the event types they consume.
    /// </summary>
    public Dictionary<MicroserviceId, IEnumerable<EventType>> ConsumersAndEventTypes { get; set; } = new();
}
