// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Aksio.Cratis.Events.Store.Grains.Observation;

/// <summary>
/// Holds log messages for <see cref="Observer"/>.
/// </summary>
public static partial class ObserverLogMessages
{
    [LoggerMessage(0, LogLevel.Information, "Subscribing observer '{ObserverId}' for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void Subscribing(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(1, LogLevel.Information, "Unsubscribing observer '{ObserverId}' for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void Unsubscribing(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(2, LogLevel.Information, "Rewinding observer '{ObserverId}' for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void Rewinding(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(3, LogLevel.Information, "Catching up observer '{ObserverId}' up for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void CatchingUp(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(4, LogLevel.Information, "Observer '{ObserverId}' is now active up for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void Active(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(5, LogLevel.Information, "Replaying observer '{ObserverId}' for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void Replaying(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(6, LogLevel.Information, "Offset is at tail for observer '{ObserverId}' for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void OffsetIsAtTail(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(7, LogLevel.Debug, "Clearing out failed partitions for observer '{ObserverId}' for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void ClearingFailedPartitions(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(8, LogLevel.Debug, "Clearing out recovering partitions for observer '{ObserverId}' for microservice '{MicroserviceId}' on sequence '{EventSequenceId}' for tenant '{TenantId}'")]
    internal static partial void ClearingRecoveringPartitions(this ILogger logger, Guid observerId, Guid microserviceId, Guid eventSequenceId, Guid tenantId);

    [LoggerMessage(9, LogLevel.Debug, "The {EventType} is being produced by multiple microservices ({microservices}), this needs to be explicitly configured in the configuration file.")]
    internal static partial void MultipleMicroservicesProducesSameEventType(this ILogger logger, Guid eventType, IEnumerable<Guid> microservices);

    [LoggerMessage(10, LogLevel.Debug, "Connecting inbox for observer '{observerId}' microservice '{MicroserviceId}' and tenant '{TenantId}' to outbox of microservice '{SourceMicroserviceId}'")]
    internal static partial void ImplicitlyConnectingInbox(this ILogger logger, Guid observerId, Guid microserviceId, Guid tenantId, Guid sourceMicroserviceId);
}
