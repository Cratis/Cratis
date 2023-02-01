// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.DependencyInversion;
using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Execution;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Kernel.Grains.EventSequences.Streaming;

/// <summary>
/// Represents an implementation of <see cref="IEventSequenceCache"/>.
/// </summary>
public class EventSequenceCache : IEventSequenceCache
{
    readonly object _lock = new();

    readonly SortedSet<AppendedEvent> _events;
    readonly SortedSet<AppendedEventByDate> _eventsByDate;
    readonly MicroserviceId _microserviceId;
    readonly TenantId _tenantId;
    readonly EventSequenceId _eventSequenceId;
    readonly IExecutionContextManager _executionContextManager;
    readonly ProviderFor<IEventSequenceStorageProvider> _eventSequenceStorageProvider;
    readonly ILogger<EventSequenceCache> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventSequenceCache"/> class.
    /// </summary>
    /// <param name="microserviceId">The <see cref="MicroserviceId"/> the cache is for.</param>
    /// <param name="tenantId">The <see cref="TenantId"/> the cache is for.</param>
    /// <param name="eventSequenceId">The <see cref="EventSequenceId"/> the cache is for.</param>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for working with the execution context.</param>
    /// <param name="eventSequenceStorageProvider">Provider for <see cref="IEventSequenceStorageProvider"/> for working with the event store.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public EventSequenceCache(
        MicroserviceId microserviceId,
        TenantId tenantId,
        EventSequenceId eventSequenceId,
        IExecutionContextManager executionContextManager,
        ProviderFor<IEventSequenceStorageProvider> eventSequenceStorageProvider,
        ILogger<EventSequenceCache> logger)
    {
        _events = new(new AppendedEventComparer());
        _eventsByDate = new(new AppendedEventByDateComparer());
        _microserviceId = microserviceId;
        _tenantId = tenantId;
        _eventSequenceId = eventSequenceId;
        _executionContextManager = executionContextManager;
        _eventSequenceStorageProvider = eventSequenceStorageProvider;
        _logger = logger;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _events.Clear();
        _eventsByDate.Clear();
    }

    /// <inheritdoc/>
    public void Add(AppendedEvent @event)
    {
        lock (_lock)
        {
            if (_events.Contains(@event))
            {
                return;
            }

            _events.Add(@event);
            _eventsByDate.Add(new(@event, DateTimeOffset.UtcNow));
        }
    }

    /// <inheritdoc/>
    public SortedSet<AppendedEvent> GetView(EventSequenceNumber from, EventSequenceNumber? to = null)
    {
        lock (_lock)
        {
            var fromEvent = AppendedEvent.EmptyWithEventSequenceNumber(from);
            var toEvent = AppendedEvent.EmptyWithEventSequenceNumber(to ?? EventSequenceNumber.Max);
            return _events.GetViewBetween(fromEvent, toEvent);
        }
    }

    /// <inheritdoc/>
    public void Prime(EventSequenceNumber from)
    {
        var to = from + 1000;
        _logger.Priming(from, to);
        _executionContextManager.Establish(_tenantId, CorrelationId.New(), _microserviceId);
        var eventCursor = _eventSequenceStorageProvider().GetRange(_eventSequenceId, from, to).GetAwaiter().GetResult();
        while (eventCursor.MoveNext().GetAwaiter().GetResult())
        {
            foreach (var @event in eventCursor.Current)
            {
                lock (_lock)
                {
                    Add(@event);
                }
            }
        }
    }
}