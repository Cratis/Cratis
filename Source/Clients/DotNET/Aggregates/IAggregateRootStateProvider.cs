// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Aggregates;

/// <summary>
/// Defines a system that can manage state for an <see cref="AggregateRoot"/>.
/// </summary>
public interface IAggregateRootStateProvider
{
    /// <summary>
    /// Handle state for an <see cref="AggregateRoot"/>.
    /// </summary>
    /// <returns>State provided.</returns>
    Task<object?> Provide();

    /// <summary>
    /// Update the state of an <see cref="AggregateRoot"/> with events.
    /// </summary>
    /// <param name="events">The events to update with.</param>
    /// <returns>Updated state.</returns>
    Task<object?> Update(IEnumerable<object> events);
}
