// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.Properties;

namespace Aksio.Cratis.Integration.for_ImportBuilderExtensions;

public class when_appending_event_from_callback : given.no_changes
{
    const string event_content = "Hello world";

    [EventType("0ec5597f-a183-4ab4-bb4a-bade3b590201", 1)]
    record EventToAppend(string Content);

    void Establish() => action_builder.AppendEvent(_ => new EventToAppend(event_content));

    void Because() => subject.OnNext(new ImportContext<Model, ExternalModel>(new AdapterProjectionResult<Model>(new(0, string.Empty), Array.Empty<PropertyPath>(), 0), changeset, events_to_append));

    [Fact] void should_append_event() => (events_to_append.First() as EventToAppend)!.Content.ShouldEqual(event_content);
}
