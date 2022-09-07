// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;

namespace Aksio.Cratis.Integration;

/// <summary>
/// Represents an implementation of <see cref="IImportActionBuilderFor{TModel, TExternalModel}"/>.
/// </summary>
/// <typeparam name="TModel">Model to build for.</typeparam>
/// <typeparam name="TExternalModel">The type of the external model.</typeparam>
public class ImportActionBuilderFor<TModel, TExternalModel> : IImportActionBuilderFor<TModel, TExternalModel>
{
    readonly IObservable<ImportContext<TModel, TExternalModel>> _observable;

    readonly List<EventType> _eventTypes = new();

    public ImportActionBuilderFor(IObservable<ImportContext<TModel, TExternalModel>> observable, IImportActionBuilderFor<TModel, TExternalModel>? parent = default)
    {
        Parent = parent;
        _observable = observable;
    }

    /// <inheritdoc/>
    public IImportActionBuilderFor<TModel, TExternalModel>? Parent { get; }

    /// <inheritdoc/>
    public void AddProducingEventType(EventType eventType)
    {
        if (Parent is not null)
        {
            Parent.AddProducingEventType(eventType);
        }
        else
        {
            _eventTypes.Add(eventType);
        }
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<ImportContext<TModel, TExternalModel>> observer) => _observable.Subscribe(observer);
}
