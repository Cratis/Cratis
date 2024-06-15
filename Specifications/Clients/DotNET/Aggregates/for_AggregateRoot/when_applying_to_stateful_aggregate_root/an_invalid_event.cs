// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Chronicle.Aggregates.for_AggregateRoot.when_applying_to_stateful_aggregate_root;

public class an_invalid_event : given.a_stateful_aggregate_root
{
    Exception result;

    void Because() => result = Catch.Exception(() => aggregate_root.Apply(new object()));

    [Fact] void should_throw_missing_event_type_attribute() => result.ShouldBeOfExactType<MissingEventTypeAttribute>();
}
