// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Benchmark.Model;
using Cratis.EventSequences;
using Cratis.Kernel.Grains.Observation;
using Cratis.Kernel.Storage.MongoDB;
using Cratis.Observation;

namespace Benchmarks.Projections;

public class ProjectionJob : BenchmarkJob
{
    protected IObserver Observer { get; private set; } = null!;

    [IterationSetup]
    public void CleanEventStore()
    {
        Database?.DropCollection(WellKnownCollectionNames.Observers);
    }

    protected override void Setup()
    {
        base.Setup();

        var key = new ObserverKey(GlobalVariables.MicroserviceId, GlobalVariables.TenantId, EventSequenceId.Log);
        Observer = GrainFactory.GetGrain<IObserver>(Guid.Parse(CartProjection.ActualIdentifier), key);
    }
}
