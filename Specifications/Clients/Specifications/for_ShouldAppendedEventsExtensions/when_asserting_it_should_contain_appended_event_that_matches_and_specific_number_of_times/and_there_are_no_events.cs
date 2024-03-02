// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit.Sdk;

namespace Aksio.Cratis.Specifications.when_asserting_it_should_contain_appended_event_that_matches_and_specific_number_of_times;

public class and_there_are_no_events : given.no_events
{
    Exception result;

    void Because() => result = Catch.Exception(() => events.ShouldContainEvent<MyEvent>((ev, _) => ev.SomeInteger == 42, Exactly(2)));

    [Fact] void should_assert_that_the_event_should_contain() => result.ShouldBeOfExactType<TrueException>();
}
