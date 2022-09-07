// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;

namespace Aksio.Cratis.Integration;

/// <summary>
/// Defines a builder for building actions to perform responding to input changes.
/// </summary>
/// <typeparam name="TModel">Model to build for.</typeparam>
/// <typeparam name="TExternalModel">The type of the external model.</typeparam>
public interface IImportActionBuilderFor<TModel, TExternalModel> : IObservable<ImportContext<TModel, TExternalModel>>
{
    /// <summary>
    /// Gets the parent <see cref="IImportActionBuilderFor{TModel, TExternalModel}"/>, if any.
    /// </summary>
    /// <value></value>
    IImportActionBuilderFor<TModel, TExternalModel>? Parent { get; }

    /// <summary>
    /// Add event type that will be produced
    /// </summary>
    /// <param name="eventType"><see cref="EventType"/> being produced.</param>
    void AddProducingEventType(EventType eventType);
}
