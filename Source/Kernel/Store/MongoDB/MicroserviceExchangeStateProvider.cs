// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.MongoDB;
using MongoDB.Driver;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;

namespace Aksio.Cratis.Events.Store.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IGrainStorage"/> for handling <see cref="MicroserviceExchangeState"/> storage.
/// </summary>
public class MicroserviceExchangeStateProvider : IGrainStorage
{
    const string CollectionName = "microservice-exchange";

    readonly IClusterDatabase _database;

    IMongoCollection<MicroserviceExchangeState> Collection => _database.GetCollection<MicroserviceExchangeState>(CollectionName);

    /// <summary>
    /// Initializes a new instance of the <see cref="MicroserviceExchangeStateProvider"/> class.
    /// </summary>
    /// <param name="database"></param>
    public MicroserviceExchangeStateProvider(IClusterDatabase database)
    {
        _database = database;
    }

    /// <inheritdoc/>
    public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {
        var filter = Builders<MicroserviceExchangeState>.Filter.Eq(new StringFieldDefinition<MicroserviceExchangeState, Guid>("_id"), Guid.Empty);
        var cursor = await Collection.FindAsync(filter);
        grainState.State = await cursor.FirstOrDefaultAsync() ?? new MicroserviceExchangeState();
    }

    /// <inheritdoc/>
    public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
    {
        var filter = Builders<MicroserviceExchangeState>.Filter.Eq(new StringFieldDefinition<MicroserviceExchangeState, Guid>("_id"), Guid.Empty);
        await Collection.ReplaceOneAsync(filter, (grainState.State as MicroserviceExchangeState)!, new ReplaceOptions { IsUpsert = true });
    }
}
