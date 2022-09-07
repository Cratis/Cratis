// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Events;

public class when_getting_event_type_from_type_with_event_type_attribute : Specification
{
    const string EventTypeId = "9e51f0bc-0e65-4616-8fdf-78e2fc012d2c";

    [EventType(EventTypeId, 1)]
    record TypeWithoutEventTypeAttribute();

    EventType result;

    void Because() => result = typeof(TypeWithoutEventTypeAttribute).GetEventType();

    [Fact] void should_return_event_type() => result.Id.ToString().ShouldEqual(EventTypeId);
}
