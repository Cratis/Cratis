// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;

namespace Aksio.Cratis.Kernel.Orleans.StateMachines;

/// <summary>
/// Represents a base implementation of <see cref="IStateMachine{TStoredState}"/>.
/// </summary>
/// <typeparam name="TStoredState">Type of stored state.</typeparam>
public class StateMachine<TStoredState> : Grain<TStoredState>, IStateMachine<TStoredState>
{
    /// <summary>
    /// Gets the supported state transitions from this state.
    /// </summary>
    public virtual IImmutableList<Type> SupportedStateTransitions => ImmutableList<Type>.Empty;

    /// <inheritdoc/>
    public virtual Task<bool> CanTransitionTo<TState>()
        where TState : IState<TStoredState> => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task TransitionTo<TState>()
        where TState : IState<TStoredState> => throw new NotImplementedException();
}
