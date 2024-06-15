// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Chronicle.Aggregates.for_AggregateRootEventHandlers.when_creating_for;

public class AggregateRootWithHandlerMethodForInterfaceEventTypeWithDerivatives : AggregateRoot
{
    public Task Handle(IMyEvent @event) => Task.CompletedTask;
}
