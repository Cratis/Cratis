// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Kernel.Contracts.Events;
using Cratis.Kernel.Contracts.EventSequences;
using Cratis.Kernel.Contracts.Observation;

namespace Cratis;

/// <summary>
/// Represents an implementation of <see cref="IServices"/>.
/// </summary>
/// <param name="EventSequences"><see cref="IEventSequences"/> instance.</param>
/// <param name="EventTypes"><see cref="IEventTypes"/> instance.</param>
/// <param name="Observers"><see cref="IObservers"/> instance.</param>
/// <param name="ClientObservers"><see cref="IClientObservers"/> instance.</param>
public record Services(
    IEventSequences EventSequences,
    IEventTypes EventTypes,
    IObservers Observers,
    IClientObservers ClientObservers) : IServices;
