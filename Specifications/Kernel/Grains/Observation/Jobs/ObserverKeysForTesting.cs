// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Kernel.Keys;
using Cratis.Kernel.Storage.Keys;

namespace Cratis.Kernel.Grains.Observation.Jobs;

public class ObserverKeysForTesting : IObserverKeys
{
    readonly IEnumerable<Key> _keys;

    public ObserverKeysForTesting(IEnumerable<Key> keys)
    {
        _keys = keys;
    }

    public async IAsyncEnumerator<Key> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        foreach (var key in _keys)
        {
            yield return key;
        }
        await Task.CompletedTask;
    }
}
