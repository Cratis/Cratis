// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Kernel.Grains.Workers;
using Orleans.Runtime;

namespace Aksio.Cratis.Kernel.Grains.Jobs;

/// <summary>
/// Represents an implementation of <see cref="IJobStep{TRequest}"/>.
/// </summary>
/// <typeparam name="TRequest">Type of request for the step.</typeparam>
/// <typeparam name="TState">Type of state for the step.</typeparam>
public abstract class JobStep<TRequest, TState> : CpuBoundWorker<TRequest, object>, IJobStep<TRequest>, IJobObserver, IDisposable
    where TState : JobStepState
{
    readonly IPersistentState<TState> _state;
    readonly CancellationTokenSource _cancellationTokenSource = new();
    bool _running;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobStep{TRequest, TState}"/> class.
    /// </summary>
    /// <param name="state"><see cref="IPersistentState{TState}"/> for managing state of the job step.</param>
    protected JobStep(
        [PersistentState(nameof(JobStepState), WellKnownGrainStorageProviders.JobSteps)]
        IPersistentState<TState> state)
    {
        Job = new NullJob();
        ThisJobStep = null!;
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

    /// <summary>
    /// Gets the job step as a Grain reference.
    /// </summary>
    protected IJobStep<TRequest> ThisJobStep { get; private set; }

    /// <summary>
    /// Gets the state for the job step.
    /// </summary>
    protected TState State => _state.State;

    /// <inheritdoc/>
    public void Dispose() => _cancellationTokenSource.Dispose();

    /// <inheritdoc/>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Keep the Grain alive forever: Confirmed here: https://github.com/dotnet/orleans/issues/1721#issuecomment-216566448
        DelayDeactivation(TimeSpan.MaxValue);

        await base.OnActivateAsync(cancellationToken);

        _ = this.GetPrimaryKey(out var key);
        var jobStepKey = (JobStepKey)key;

        var type = GetType();
        var grainType = type.GetInterfaces().SingleOrDefault(_ => _.Name == $"I{type.Name}") ?? throw new InvalidGrainName(type);
        _state.State.Name = type.Name;
        _state.State.Id = new(jobStepKey.JobId, JobStepId);
        _state.State.Type = grainType;
        await _state.WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task Start(GrainId jobId, TRequest request)
    {
        _running = true;
        Job = GrainFactory.GetGrain(jobId).AsReference<IJob>();
        ThisJobStep = GrainFactory.GetGrain(GrainReference.GrainId).AsReference<IJobStep<TRequest>>();

        StatusChanged(JobStepStatus.Running);
        _state.State.Request = request!;
        await _state.WriteStateAsync();
        await Prepare(request);
        await Start(request, _cancellationTokenSource.Token);
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task Pause()
    {
        _cancellationTokenSource.Cancel();
        await Job.Unsubscribe(this.AsReference<IJobObserver>());
        StatusChanged(JobStepStatus.Paused);
    }

    /// <inheritdoc/>
    public Task Resume(GrainId grainId)
    {
        if (_running)
        {
            return Task.CompletedTask;
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task Stop()
    {
        _cancellationTokenSource.Cancel();
        await Job.Unsubscribe(this.AsReference<IJobObserver>());
        StatusChanged(JobStepStatus.Stopped);
        await _state.WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task ReportStatusChange(JobStepStatus status)
    {
        if (status == JobStepStatus.Succeeded)
        {
            await Job.OnStepSucceeded(JobStepId);
        }
        StatusChanged(status);
        await _state.WriteStateAsync();
    }

    /// <inheritdoc/>
    public async Task ReportFailure(IList<string> exceptionMessages, string exceptionStackTrace)
    {
        StatusChanged(JobStepStatus.Failed, exceptionMessages, exceptionStackTrace);
        await _state.WriteStateAsync();
        await Job.OnStepFailed(JobStepId);
    }

    /// <summary>
    /// Prepare the step before it starts.
    /// </summary>
    /// <param name="request">The request object for the step.</param>
    /// <returns>Awaitable task.</returns>
    public virtual Task Prepare(TRequest request) => Task.CompletedTask;

    /// <inheritdoc/>
    public void OnJobStopped() => _cancellationTokenSource.Cancel();

    /// <inheritdoc/>
    public void OnJobPaused() => _cancellationTokenSource.Cancel();

    /// <summary>
    /// The method that gets called when the step should do its work.
    /// </summary>
    /// <param name="request">The request object for the step.</param>
    /// <param name="cancellationToken">Cancellation token that can cancel the step work.</param>
    /// <returns>True if successful, false if not.</returns>
    protected abstract Task<JobStepResult> PerformStep(TRequest request, CancellationToken cancellationToken);

    /// <inheritdoc/>
    protected override async Task<object> PerformWork(TRequest request)
    {
        await ThisJobStep.ReportStatusChange(JobStepStatus.Running);

        var result = JobStepResult.Succeeded;
        try
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                result = await PerformStep(request, _cancellationTokenSource.Token);
            }
        }
        catch (Exception ex)
        {
            result = new JobStepResult(JobStepStatus.Failed, ex.GetAllMessages(), ex.StackTrace ?? string.Empty);
        }

        if (result.IsSuccess)
        {
            await ThisJobStep.ReportStatusChange(JobStepStatus.Succeeded);
        }
        else
        {
            await ThisJobStep.ReportFailure(result.Messages.ToList(), result.ExceptionStackTrace);
        }

        DeactivateOnIdle();

        return string.Empty;
    }

    void StatusChanged(JobStepStatus status, IEnumerable<string>? exceptionMessages = null!, string? exceptionStackTrace = null!)
    {
        _state.State.StatusChanges.Add(new JobStepStatusChanged
        {
            Status = status,
            Occurred = DateTimeOffset.UtcNow,
            ExceptionMessages = exceptionMessages ?? Enumerable.Empty<string>(),
            ExceptionStackTrace = exceptionStackTrace ?? string.Empty
        });
        _state.State.Status = status;
    }
}