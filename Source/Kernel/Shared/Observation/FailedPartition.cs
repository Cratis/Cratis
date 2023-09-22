// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;
using Aksio.Cratis.EventSequences;
using Aksio.Cratis.Observation;

namespace Aksio.Cratis.Kernel.Observation;

/// <summary>
/// Holds the state for the job to recover a failed partition.
/// </summary>
public class FailedPartition
{
    /// <summary>
    /// Unique identifier of the failed partition.
    /// </summary>
    public FailedPartitionId Id { get; set; } = FailedPartitionId.New();

    /// <summary>
    /// Gets or sets the partition that is failed.
    /// </summary>
    public EventSourceId Partition { get; set; } = EventSourceId.Unspecified;

    /// <summary>
    /// Gets or sets the <see cref="EventSequenceId"/> the failed partition is on.
    /// </summary>
    public EventSequenceId EventSequenceId { get; set; } = EventSequenceId.Unspecified;

    /// <summary>
    /// Gets or sets the <see cref="ObserverId"/> for which this is a failed partition.
    /// </summary>
    public ObserverId ObserverId { get; set; } = ObserverId.Unspecified;

    /// <summary>
    /// The event types that are being processed by the failed partition.
    /// </summary>
    public IEnumerable<EventType> EventTypes { get; set; } = Enumerable.Empty<EventType>();

    /// <summary>
    /// Gets or sets a collection of <see cref="FailedPartitionAttempt"/> for the failed partition.
    /// </summary>
    public IEnumerable<FailedPartitionAttempt> Attempts { get; set; } = Enumerable.Empty<FailedPartitionAttempt>();

    /// <summary>
    /// Gets or sets whether or not the failure is resolved.
    /// </summary>
    public bool IsResolved { get; set; }

    /// <summary>
    /// Add an attempt to the failed partition.
    /// </summary>
    /// <param name="attempt">Attempt to add.</param>
    public void AddAttempt(FailedPartitionAttempt attempt)
    {
        Attempts = Attempts.Append(attempt);
    }
}
