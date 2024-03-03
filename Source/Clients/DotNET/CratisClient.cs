// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Types;
using Cratis.Auditing;
using Cratis.Compliance;
using Cratis.Connections;
using Cratis.Schemas;
using Microsoft.Extensions.Logging;

namespace Cratis;

/// <summary>
/// Represents an implementation of <see cref="ICratisClient"/>.
/// </summary>
public class CratisClient : ICratisClient, IDisposable
{
    readonly CratisOptions _options;
    readonly ICratisConnection? _connection;
    readonly ICausationManager _causationManager;
    readonly IJsonSchemaGenerator _jsonSchemaGenerator;
    readonly IComplianceMetadataResolver _complianceMetadataResolver;
    readonly IConnectionLifecycle _connectionLifecycle;

    /// <summary>
    /// Initializes a new instance of the <see cref="CratisClient"/> class.
    /// </summary>
    /// <param name="connectionString">Connection string to use.</param>
    public CratisClient(string connectionString)
        : this(new CratisUrl(connectionString))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CratisClient"/> class.
    /// </summary>
    /// <param name="url"><see cref="CratisUrl"/> to connect with.</param>
    public CratisClient(CratisUrl url)
        : this(CratisOptions.FromUrl(url))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CratisClient"/> class.
    /// </summary>
    /// <param name="options"><see cref="CratisOptions"/> to use.</param>
    public CratisClient(CratisOptions options)
    {
        _options = options;
        _causationManager = new CausationManager();
        _complianceMetadataResolver = new ComplianceMetadataResolver(
            new InstancesOf<ICanProvideComplianceMetadataForType>(Types.Instance, options.ServiceProvider),
            new InstancesOf<ICanProvideComplianceMetadataForProperty>(Types.Instance, options.ServiceProvider));
        _jsonSchemaGenerator = new JsonSchemaGenerator(_complianceMetadataResolver);
        _connectionLifecycle = new ConnectionLifecycle(options.LoggerFactory.CreateLogger<ConnectionLifecycle>());
        _connection = new CratisConnection(
            options,
            _connectionLifecycle,
            new Tasks.Tasks(),
            options.LoggerFactory.CreateLogger<CratisConnection>(),
            CancellationToken.None);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _connection?.Dispose();
    }

    /// <inheritdoc/>
    public IEventStore GetEventStore(EventStoreName name, EventStoreNamespaceName? @namespace = null) =>
        new EventStore(
            name,
            @namespace ?? EventStoreNamespaceName.Default,
            _connection!,
            _options.ArtifactsProvider,
            _causationManager,
            _options.IdentityProvider,
            _jsonSchemaGenerator,
            _options.ModelNameConvention,
            _options.ServiceProvider,
            _options.JsonSerializerOptions,
            _options.LoggerFactory);

    /// <inheritdoc/>
    public IAsyncEnumerable<EventStoreName> ListEventStores(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}