// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting;

namespace Cratis.Events.Observation
{
    /// <summary>
    /// Represents a <see cref="IHostedService"/> for working with observers.
    /// </summary>
    public class ObserversService : IHostedService
    {
        readonly IObservers _observers;

        public ObserversService(IObservers observers)
        {
            _observers = observers;
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _observers.StartObserving();
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
