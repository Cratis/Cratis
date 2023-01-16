// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Aksio.Cratis.Clients;
using Aksio.Cratis.Events;
using Aksio.Cratis.Execution;
using Aksio.Cratis.Types;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Observation;

/// <summary>
/// Represents an implementation of <see cref="Observers"/>.
/// </summary>
public class Observers : IParticipateInClientLifecycle
{
    readonly IEnumerable<ObserverHandler> _observerHandlers;
    readonly IExecutionContextManager _executionContextManager;
    readonly IClient _client;
    readonly ILogger<Observers> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="Observers"/>.
    /// </summary>
    /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for establishing execution context.</param>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> to get instances of types.</param>
    /// <param name="middlewares"><see cref="IObserverMiddlewares"/> to call.</param>
    /// <param name="eventTypes">Registered <see cref="IEventTypes"/>.</param>
    /// <param name="eventSerializer"><see cref="IEventSerializer"/> for serializing of events.</param>
    /// <param name="types"><see cref="ITypes"/> for type discovery.</param>
    /// <param name="client"><see cref="IClient"/> for working with kernel.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    public Observers(
        IExecutionContextManager executionContextManager,
        IServiceProvider serviceProvider,
        IObserverMiddlewares middlewares,
        IEventTypes eventTypes,
        IEventSerializer eventSerializer,
        ITypes types,
        IClient client,
        ILogger<Observers> logger)
    {
        _observerHandlers = types.AllObservers()
                            .Select(_ =>
                            {
                                var observer = _.GetCustomAttribute<ObserverAttribute>()!;
                                return new ObserverHandler(
                                    observer.ObserverId,
                                    _.FullName ?? $"{_.Namespace}.{_.Name}",
                                    observer.EventSequenceId,
                                    eventTypes,
                                    new ObserverInvoker(serviceProvider, eventTypes, middlewares, _),
                                    eventSerializer);
                            });
        _executionContextManager = executionContextManager;
        _client = client;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task Connected()
    {
        _logger.RegisterObservers();

        foreach (var observerHandler in _observerHandlers)
        {
            _logger.RegisterObserver(
                observerHandler.ObserverId,
                observerHandler.Name,
                observerHandler.EventSequenceId);
        }

        var microserviceId = _executionContextManager.Current.MicroserviceId;
        var route = $"/api/events/store/{microserviceId}/observers/register/{_client.ConnectionId}";
        var registrations = _observerHandlers.Select(_ => new ClientObserverRegistration(
            _.ObserverId,
            _.Name,
            _.EventSequenceId,
            _.EventTypes
        ));
        await _client.PerformCommand(route, registrations);
    }

    /// <inheritdoc/>
    public Task Disconnected() => Task.CompletedTask;
}