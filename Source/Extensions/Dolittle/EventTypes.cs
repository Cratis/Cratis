// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias SDK;

using System.Reflection;
using Cratis.Reflection;
using Cratis.Types;
using Dolittle.SDK.Events;
using EventType = Cratis.Events.EventType;
using EventTypeId = Cratis.Events.EventTypeId;

namespace Cratis.Extensions.Dolittle
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventTypes"/> for Dolittle.
    /// </summary>
    public class EventTypes : SDK::Cratis.Events.IEventTypes
    {
        readonly IDictionary<EventType, Type> _typesByEventType;

        /// <summary>
        /// Initializes a new instance of <see cref="EventTypes"/>.
        /// </summary>
        /// <param name="types"><see cref="ITypes"/> for type discovery.</param>
        public EventTypes(ITypes types)
        {
            _typesByEventType = types.All
                            .Where(_ => _.HasAttribute<EventTypeAttribute>())
                            .ToDictionary(_ =>
                            {
                                var eventType = _.GetCustomAttribute<EventTypeAttribute>()!;
                                return new EventType(eventType.Identifier.Value, eventType.Generation.Value);
                            }, _ => _);

            All = _typesByEventType.Keys.ToArray();
        }

        /// <inheritdoc/>
        public IEnumerable<EventType> All { get; }

        /// <inheritdoc/>
        public bool HasFor(EventTypeId eventTypeId) => _typesByEventType.Any(_ => _.Key.Id == eventTypeId);

        /// <inheritdoc/>
        public EventType GetEventTypeFor(Type clrType)
        {
            TypeIsMissingEventType.ThrowIfMissingEventType(clrType);
            return _typesByEventType.Single(_ => _.Value == clrType).Key;
        }

        /// <inheritdoc/>
        public bool HasFor(Type clrType) => _typesByEventType.Any(_ => _.Value == clrType);

        /// <inheritdoc/>
        public Type GetClrTypeFor(EventTypeId eventTypeId) => _typesByEventType.Single(_ => _.Key.Id == eventTypeId).Value;
    }
}
