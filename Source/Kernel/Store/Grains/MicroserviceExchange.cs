// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Execution;
using Orleans;
using Orleans.Providers;

namespace Aksio.Cratis.Events.Store.Grains;

[StorageProvider(ProviderName = MicroserviceExchangeState.StorageProvider)]
public class MicroserviceExchange : Grain<MicroserviceExchangeState>, IMicroserviceExchange
{
    /// <inheritdoc/>
    public async Task SetProducerAndEventTypes(MicroserviceId microserviceId, IEnumerable<EventType> eventTypes)
    {
        State.ProducersAndEventTypes[microserviceId] = eventTypes;
        await WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task SetConsumerAndEventTypes(MicroserviceId microserviceId, IEnumerable<EventType> eventTypes)
    {
        State.ConsumersAndEventTypes[microserviceId] = eventTypes;
        await WriteStateAsync();
    }
}
