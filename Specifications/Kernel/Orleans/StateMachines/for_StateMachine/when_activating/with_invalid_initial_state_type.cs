// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aksio.Cratis.Kernel.Orleans.StateMachines.when_activating;

public class with_invalid_initial_state_type : Specification
{
    Exception exception;

    async Task Because() => exception = await Catch.Exception(async () =>
    {
        var stateMachine = new StateMachineWithInvalidInitialStateType();
        await stateMachine.OnActivateAsync(CancellationToken.None);
    });

    [Fact] void should_throw_invalid_type_for_state_exception() => exception.ShouldBeOfExactType<InvalidTypeForState>();
}
