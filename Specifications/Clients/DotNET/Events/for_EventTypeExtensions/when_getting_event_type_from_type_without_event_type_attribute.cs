// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events;

public class when_getting_event_type_from_type_without_event_type_attribute : Specification
{
    record TypeWithoutEventTypeAttribute();

    Exception result;

    void Because() => result = Catch.Exception(() => typeof(TypeWithoutEventTypeAttribute).GetEventType());

    [Fact] void should_throw_missing_event_type_attribute() => result.ShouldBeOfExactType<MissingEventTypeAttribute>();
}
