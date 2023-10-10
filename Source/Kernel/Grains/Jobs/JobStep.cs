// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.SyncWork;

namespace Aksio.Cratis.Kernel.Grains.Jobs;

/// <summary>
/// Represents an implementation of <see cref="IJobStep{TRequest}"/>.
/// </summary>
/// <typeparam name="TRequest">Type of request for the step.</typeparam>
/// <typeparam name="TState">Type of state for the step.</typeparam>
public abstract class JobStep<TRequest, TState> : SyncWorker<TRequest, object>, IJobStep<TRequest>
    where TState : JobStepState
{
    readonly IPersistentState<TState> _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobStep{TRequest, TState}"/> class.
    /// </summary>
    /// <param name="state"><see cref="IPersistentState{TState}"/> for managing state of the job step.</param>
    /// <param name="taskScheduler"><see cref="LimitedConcurrencyLevelTaskScheduler"/> to use for scheduling.</param>
    /// <param name="logger"><see cref="ILogger"/> for logging.</param>
    protected JobStep(
        [PersistentState(nameof(JobStepState), WellKnownGrainStorageProviders.JobSteps)] IPersistentState<TState> state,
        LimitedConcurrencyLevelTaskScheduler taskScheduler,
        ILogger logger) : base(logger, taskScheduler)
    {
        Job = new NullJob();
        _state = state;
    }

    /// <summary>
    /// Gets the <see cref="JobStepId"/> for this job step.
    /// </summary>
    public JobStepId JobStepId => this.GetPrimaryKey();

    /// <summary>
    /// Gets the parent job.
    /// </summary>
    protected IJob Job { get; private set; }

    /// <inheritdoc/>
    public async Task Start(GrainId jobId, TRequest request)
    {
        Job = GrainFactory.GetGrain(jobId).AsReference<IJob>();

        StatusChanged(JobStepStatus.Running);
        await _state.WriteStateAsync();
        await PrepareStep(request);
        await Start(request);
    }

    /// <inheritdoc/>
    public Task Stop() => throw new NotImplementedException();

    /// <summary>
    /// Prepare the step before it starts.
    /// </summary>
    /// <param name="request">The request object for the step.</param>
    /// <returns>Awaitable task.</returns>
    protected virtual Task PrepareStep(TRequest request) => Task.CompletedTask;

    /// <summary>
    /// The method that gets called when the step should do its work.
    /// </summary>
    /// <param name="request">The request object for the step.</param>
    /// <returns>True if successful, false if not.</returns>
    protected abstract Task<bool> PerformStep(TRequest request);

    /// <inheritdoc/>
    protected override async Task<object> PerformWork(TRequest request)
    {
        StatusChanged(JobStepStatus.Running);
        await _state.WriteStateAsync();

        var result = await PerformStep(request);
        if (result)
        {
            await Job.OnStepSuccessful(JobStepId);
            StatusChanged(JobStepStatus.Succeeded);
        }
        else
        {
            await Job.OnStepFailed(JobStepId);
            StatusChanged(JobStepStatus.Failed);
        }

        await _state.WriteStateAsync();
        return "OK";
    }

    void StatusChanged(JobStepStatus status)
    {
        _state.State.StatusChanges.Add(new JobStepStatusChanged
        {
            Status = status,
            Occurred = DateTimeOffset.UtcNow
        });
        _state.State.Status = status;
    }
}
