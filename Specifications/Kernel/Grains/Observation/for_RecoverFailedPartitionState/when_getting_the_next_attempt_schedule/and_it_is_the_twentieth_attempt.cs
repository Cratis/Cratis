namespace Aksio.Cratis.Kernel.Grains.Observation.for_ObserverSupervisor.for_RecoverFailedPartitionState.when_getting_the_next_attempt_schedule;

public class and_it_is_the_twentieth_attempt : Specification
{
    RecoverFailedPartitionState state;
    DateTimeOffset now;
    TimeSpan nextScheduledAttempt;

    Task Establish()
    {
        now = DateTimeOffset.UtcNow;
        state = new RecoverFailedPartitionState()
        {
            InitialError = EventSequenceNumber.First,
            CurrentError = EventSequenceNumber.First,
            NextSequenceNumberToProcess = 2,
            NumberOfAttemptsOnCurrentError = 20,
            NumberOfAttemptsOnSinceInitialised = 20
        };

        return Task.CompletedTask;
    }
    
    Task Because()
    {
        nextScheduledAttempt = state.GetNextAttemptSchedule();
        return Task.CompletedTask;
    }
    
    [Fact] void should_return_1_hour() => nextScheduledAttempt.ShouldEqual(TimeSpan.FromHours(1));
}