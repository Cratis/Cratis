// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Grains.Jobs;

/// <summary>
/// Holds the state of a <see cref="IJob{TRequest}"/>.
/// </summary>
public class JobState
{
    /// <summary>
    /// Gets or sets the <see cref="JobStatus"/>.
    /// </summary>
    public JobStatus Status { get; set; }

    /// <summary>
    /// Gets or sets collection of status changes that happened to the job.
    /// </summary>
    public IList<JobStatusChanged> StatusChanges { get; set; } = new List<JobStatusChanged>();

    /// <summary>
    /// Gets or sets the <see cref="JobProgress"/>.
    /// </summary>
    public JobProgress Progress { get; set; } = new();
}
