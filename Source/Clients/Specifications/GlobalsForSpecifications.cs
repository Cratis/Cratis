// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Compliance;
using Cratis.Events;
using Cratis.Schemas;
using Cratis.Types;

namespace Cratis.Specifications;

/// <summary>
/// Holds the global instances used for testing.
/// </summary>
public static class GlobalsForSpecifications
{
    static GlobalsForSpecifications()
    {
        ClientArtifactsProvider = new DefaultClientArtifactsProvider(
            new CompositeAssemblyProvider(ProjectReferencedAssemblies.Instance, PackageReferencedAssemblies.Instance));

        EventTypes = new Cratis.Events.EventTypes(
            new NullEventStore(),
            new JsonSchemaGenerator(new ComplianceMetadataResolver(
                new KnownInstancesOf<ICanProvideComplianceMetadataForType>(),
                new KnownInstancesOf<ICanProvideComplianceMetadataForProperty>())),
            ClientArtifactsProvider);
    }

    /// <summary>
    /// Gets the <see cref="IClientArtifactsProvider"/>.
    /// </summary>
    public static IClientArtifactsProvider ClientArtifactsProvider { get; }

    /// <summary>
    /// Gets the <see cref="IEventTypes"/>.
    /// </summary>
    public static IEventTypes EventTypes { get; }
}
