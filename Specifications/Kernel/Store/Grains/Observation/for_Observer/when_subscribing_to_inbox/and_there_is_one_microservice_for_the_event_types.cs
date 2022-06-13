// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events.Schemas.Grains;
using Aksio.Cratis.Events.Store.Grains.Inboxes;
using Aksio.Cratis.Events.Store.Inboxes;
using Aksio.Cratis.Execution;

namespace Aksio.Cratis.Events.Store.Grains.Observation.for_Observer.when_subscribing_to_inbox;

public class and_there_is_one_microservice_for_the_event_types : given.an_observer_and_two_event_types
{
    Mock<ISchemaStore> schema_store;
    Mock<IInbox> inbox;
    MicroserviceId source_microservice = "4f09158a-cb2d-4a9c-af2a-f006c505b06b";

    protected override EventSequenceId event_sequence_id => EventSequenceId.Inbox;

    void Establish()
    {
        schema_store = new();
        inbox = new();
        var inboxKey = new InboxKey(tenant_id, source_microservice);
        grain_factory.Setup(_ => _.GetGrain<ISchemaStore>(Guid.Empty, null)).Returns(schema_store.Object);
        grain_factory.Setup(_ => _.GetGrain<IInbox>(microservice_id, inboxKey, null)).Returns(inbox.Object);

        event_sequence_storage_provider.Setup(_ => _.GetTailSequenceNumber(event_sequence_id, event_types, null)).Returns(Task.FromResult((EventSequenceNumber)0));

        schema_store.Setup(_ => _.GetMicroservicesThatProduceEventType(IsAny<EventType>())).Returns(Task.FromResult(new[] { source_microservice }.AsEnumerable()));
    }

    async Task Because() => await observer.Subscribe(event_types, observer_namespace);

    [Fact] void should_start_inbox() => inbox.Verify(_ => _.Start(), Once());
}
