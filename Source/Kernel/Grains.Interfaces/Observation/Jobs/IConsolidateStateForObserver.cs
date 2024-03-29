// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Kernel.Grains.Jobs;
using Cratis.Observation;

namespace Cratis.Kernel.Grains.Observation.Jobs;

/// <summary>
/// Defines a step in the <see cref="IConsolidateStateForObservers"/> job.
/// </summary>
public interface IConsolidateStateForObserver : IJobStep<ObserverIdAndKey, object>;
