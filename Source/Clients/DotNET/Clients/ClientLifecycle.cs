// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Types;
using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Clients;

/// <summary>
/// Represents an implementation of <see cref="IClientLifecycle"/>.
/// </summary>
[Singleton]
public class ClientLifecycle : IClientLifecycle
{
    readonly IInstancesOf<IParticipateInClientLifecycle> _participants;
    readonly ILogger<ClientLifecycle> _logger;

    /// <inheritdoc/>
    public bool IsConnected { get; private set; }

    /// <inheritdoc/>
    public ConnectionId ConnectionId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientLifecycle"/>.
    /// </summary>
    /// <param name="participants">The participants of the client lifecycle.</param>
    /// <param name="logger">Logger for logging.</param>
    public ClientLifecycle(
        IInstancesOf<IParticipateInClientLifecycle> participants,
        ILogger<ClientLifecycle> logger)
    {
        _participants = participants;
        _logger = logger;
        ConnectionId = ConnectionId.New();
    }

    /// <inheritdoc/>
    public async Task Connected()
    {
        var completedParticipants = new List<IParticipateInClientLifecycle>();
        var tcs = new TaskCompletionSource<bool>();

        IsConnected = true;
        await Parallel.ForEachAsync(_participants, async (participant, _) =>
        {
            try
            {
                await new ValueTask(participant.ClientConnected());
                completedParticipants.Add(participant);

                if (completedParticipants.Count == _participants.Count())
                {
                    tcs.SetResult(true);
                }
            }
            catch (Exception ex)
            {
                _logger.ParticipantFailedDuringConnected(participant!.GetType().FullName ?? participant!.GetType().Name, ex);
            }
        });

        await tcs.Task;
    }

    /// <inheritdoc/>
    public async Task Disconnected()
    {
        IsConnected = false;
        await Parallel.ForEachAsync(_participants, async (participant, _) =>
        {
            try
            {
                await new ValueTask(participant.ClientDisconnected());
            }
            catch (Exception ex)
            {
                _logger.ParticipantFailedDuringDisconnected(participant!.GetType().FullName ?? participant!.GetType().Name, ex);
            }
        });
        ConnectionId = ConnectionId.New();
    }
}
