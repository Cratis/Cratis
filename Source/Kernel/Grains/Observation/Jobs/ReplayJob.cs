// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.Kernel.Grains.Jobs;
using Aksio.Cratis.Kernel.Grains.Observation.States;
using Aksio.Cratis.Kernel.Keys;

namespace Aksio.Cratis.Kernel.Grains.Observation.Jobs;

/// <summary>
/// Represents a job for replaying an observer.
/// </summary>
public class ReplayJob : Job<ReplayRequest, JobState<ReplayRequest>>, IReplayJob
{
    readonly IObserverKeyIndexes _observerKeyIndexes;
    ReplayRequest? _request;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplayJob"/> class.
    /// </summary>
    /// <param name="observerKeyIndexes"><see cref="IObserverKeyIndexes"/> for getting index for observer to replay.</param>
    public ReplayJob(IObserverKeyIndexes observerKeyIndexes)
    {
        _observerKeyIndexes = observerKeyIndexes;
    }

    /// <inheritdoc/>
    public override async Task OnCompleted()
    {
        if (_request == null) return;
        var observer = GrainFactory.GetGrain<IObserver>(_request.ObserverId, _request.ObserverKey);
        await observer.TransitionTo<Routing>();
    }

    /// <inheritdoc/>
    protected override async Task PrepareSteps(ReplayRequest request)
    {
        _request = request;
        var index = await _observerKeyIndexes.GetFor(
            request.ObserverId,
            request.ObserverKey);

        _ = Task.Run(async () =>
        {
            var keys = await index.GetKeys(EventSequenceNumber.First);

            await foreach (var key in keys)
            {
                await ThisJob.AddStep<IHandleEventsForPartition, HandleEventsForPartitionArguments>(
                    new HandleEventsForPartitionArguments(
                        request.ObserverId,
                        request.ObserverKey,
                        request.ObserverSubscription,
                        key,
                        EventSequenceNumber.First,
                        request.EventTypes));
            }
        });

        await WriteStateAsync();
    }
}
