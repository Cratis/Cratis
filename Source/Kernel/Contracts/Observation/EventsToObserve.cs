// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Kernel.Contracts.Events;
using ProtoBuf;

namespace Cratis.Kernel.Contracts.Observation;

/// <summary>
/// Represents the payload when observing an event sequence.
/// </summary>
[ProtoContract]
public class EventsToObserve
{
    /// <summary>
    /// Gets or sets a collection of <see cref="AppendedEvent"/>.
    /// </summary>
    [ProtoMember(1)]
    public IEnumerable<AppendedEvent> Events { get; set; }
}
