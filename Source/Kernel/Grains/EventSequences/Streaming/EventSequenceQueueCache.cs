// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Execution;
using Aksio.Cratis.Kernel.EventSequences;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace Aksio.Cratis.Kernel.Grains.EventSequences.Streaming;

/// <summary>
/// Represents an implementation of <see cref="IQueueCache"/> for MongoDB event log.
/// </summary>
public class EventSequenceQueueCache : IQueueCache
{
    readonly IEventSequenceCaches _caches;
    readonly ILogger _logger;
    readonly QueueId _queueId;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSequenceQueueCache"/> class.
    /// </summary>
    /// <param name="caches"></param>
    /// <param name="logger"></param>
    /// <param name="queueId"></param>
    public EventSequenceQueueCache(
        IEventSequenceCaches caches,
        ILogger logger,
        QueueId queueId)
    {
        _caches = caches;
        _logger = logger;
        _queueId = queueId;
    }

    /// <inheritdoc/>
    public void AddToCache(IList<IBatchContainer> messages)
    {
        foreach (var message in messages.Where(_ => !_.SequenceToken.IsWarmUp()))
        {
            if (message is EventSequenceBatchContainer batchContainer)
            {
                var microserviceAndTenant = (MicroserviceAndTenant)message.StreamNamespace;
                foreach (var (@event, _) in batchContainer.GetEvents<AppendedEvent>())
                {
                    _caches.GetFor(microserviceAndTenant.MicroserviceId, microserviceAndTenant.TenantId, (EventSequenceId)message.StreamGuid).Add(@event);
                }
            }
        }
    }

    /// <inheritdoc/>
    public IQueueCacheCursor GetCacheCursor(IStreamIdentity streamIdentity, StreamSequenceToken token)
    {
        if (token is null)
        {
            return new EmptyEventSequenceQueueCacheCursor();
        }

        if (token.IsWarmUp())
        {
            return new EmptyEventSequenceQueueCacheCursor();
        }

        var microserviceAndTenant = (MicroserviceAndTenant)streamIdentity.Namespace;
        return new EventSequenceQueueCacheCursor(
            _caches.GetFor(
                microserviceAndTenant.MicroserviceId,
                microserviceAndTenant.TenantId,
                (EventSequenceId)streamIdentity.Guid),
            microserviceAndTenant.MicroserviceId,
            microserviceAndTenant.TenantId,
            (EventSequenceId)streamIdentity.Guid,
            (ulong)token.SequenceNumber,
            _logger,
            _queueId);
    }

    /// <inheritdoc/>
    public int GetMaxAddCount() => int.MaxValue;

    /// <inheritdoc/>
    public bool IsUnderPressure() => false;

    /// <inheritdoc/>
    public bool TryPurgeFromCache(out IList<IBatchContainer> purgedItems)
    {
        purgedItems = null!;
        return false;
    }
}