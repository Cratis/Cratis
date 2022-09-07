// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using Aksio.Cratis.Changes;
using Aksio.Cratis.Events;
using Aksio.Cratis.Reflection;
using AutoMapper;

namespace Aksio.Cratis.Integration;

/// <summary>
/// Extension methods for building on the <see cref="IImportBuilderFor{TModel, TExternalModel}"/>.
/// </summary>
public static class ImportBuilderExtensions
{
    static class ModelToEventMapperFor<TModel, TEvent>
    {
        public static IMapper Mapper;

        static ModelToEventMapperFor()
        {
            var eventProperties = typeof(TEvent).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var modelProperties = typeof(TModel).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var configuration = new MapperConfiguration(cfg =>
            {
                var map = cfg.CreateMap<TModel, TEvent>();
                foreach (var eventProperty in eventProperties)
                {
                    if (!modelProperties.Any(_ => _.Name == eventProperty.Name))
                    {
                        throw new MissingExpectedEventPropertyOnModel(typeof(TEvent), typeof(TModel), eventProperty.Name);
                    }

                    map.ForMember(eventProperty.Name, _ => _.MapFrom(eventProperty.Name));
                    map.ForCtorParam(eventProperty.Name, _ => _.MapFrom(eventProperty.Name));
                }
            });

            Mapper = configuration.CreateMapper();
        }
    }

    /// <summary>
    /// Filter down to when a model already exists.
    /// </summary>
    /// <param name="builder"><see cref="IImportBuilderFor{TModel, TExternalModel}"/> to build the filter for.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <returns>Observable for chaining.</returns>
    public static IObservable<ImportContext<TModel, TExternalModel>> WhenModelExists<TModel, TExternalModel>(this IImportBuilderFor<TModel, TExternalModel> builder)
    {
        return builder.Where(_ => _.InitialProjectionResult.ProjectedEventsCount > 0 && _.InitialProjectionResult.AffectedProperties.Any());
    }

    /// <summary>
    /// Filter down to when a model does not exist.
    /// </summary>
    /// <param name="builder"><see cref="IImportBuilderFor{TModel, TExternalModel}"/> to build the filter for.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <returns><see cref="IImportActionBuilderFor{TModel, TExternalModel}"/> for chaining.</returns>
    public static IImportActionBuilderFor<TModel, TExternalModel> WhenModelDoesNotExist<TModel, TExternalModel>(this IImportBuilderFor<TModel, TExternalModel> builder)
    {
        return new ImportActionBuilderFor<TModel, TExternalModel>(
            builder.Where(_ => _.InitialProjectionResult.ProjectedEventsCount == 0 && !_.InitialProjectionResult.AffectedProperties.Any()),
            builder);
    }

    /// <summary>
    /// Filter down to when specific properties on a model are set.
    /// </summary>
    /// <param name="builder"><see cref="IImportBuilderFor{TModel, TExternalModel}"/> to build the filter for.</param>
    /// <param name="properties">Properties as expressions to look for if was set on model.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <returns><see cref="IImportActionBuilderFor{TModel, TExternalModel}"/> for chaining.</returns>
    public static IImportActionBuilderFor<TModel, TExternalModel> WhenModelPropertiesAreSet<TModel, TExternalModel>(this IImportBuilderFor<TModel, TExternalModel> builder, params Expression<Func<TModel, object>>[] properties)
    {
        var propertyPaths = properties.Select(_ => _.GetPropertyPath()).ToArray();
        return new ImportActionBuilderFor<TModel, TExternalModel>(
            builder.Where(_ => _.InitialProjectionResult.AffectedProperties.Any(_ => propertyPaths.Contains(_))),
            builder);
    }

    /// <summary>
    /// Filter down to when specific properties on a model are set.
    /// </summary>
    /// <param name="builder"><see cref="IImportBuilderFor{TModel, TExternalModel}"/> to build the filter for.</param>
    /// <param name="properties">Properties as expressions to look for if was set on model.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <returns><see cref="IImportActionBuilderFor{TModel, TExternalModel}"/> for chaining.</returns>
    public static IImportActionBuilderFor<TModel, TExternalModel> WhenModelPropertiesAreNotSet<TModel, TExternalModel>(this IImportBuilderFor<TModel, TExternalModel> builder, params Expression<Func<TModel, object>>[] properties)
    {
        var propertyPaths = properties.Select(_ => _.GetPropertyPath()).ToArray();
        return new ImportActionBuilderFor<TModel, TExternalModel>(
            builder.Where(_ => !_.InitialProjectionResult.AffectedProperties.Any(_ => propertyPaths.Contains(_))),
            builder);
    }

    /// <summary>
    /// Filter down to when one of the properties defined changes.
    /// </summary>
    /// <param name="builder">Observable of the <see cref="ImportContext{TModel, TExternalModel}"/>.</param>
    /// <param name="properties">Properties as expressions to look for changes on.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <returns><see cref="IImportActionBuilderFor{TModel, TExternalModel}"/> for chaining.</returns>
    public static IImportActionBuilderFor<TModel, TExternalModel> WithProperties<TModel, TExternalModel>(this IImportActionBuilderFor<TModel, TExternalModel> builder, params Expression<Func<TModel, object>>[] properties)
    {
        var propertyPaths = properties.Select(_ => _.GetPropertyPath()).ToArray();

        return new ImportActionBuilderFor<TModel, TExternalModel>(builder.Where(_ =>
        {
            var changes = _.Changeset.Changes.Where(_ => _ is PropertiesChanged<TModel>).Select(_ => _ as PropertiesChanged<TModel>);
            return changes.Any(_ => _!.Differences.Any(_ => propertyPaths.Any(p => _.PropertyPath.Path.StartsWith(p.Path))));
        }), builder);
    }

    /// <summary>
    /// Filter down to when one of the properties defined changes from a value to a null value.
    /// </summary>
    /// <param name="builder">Observable of the <see cref="ImportContext{TModel, TExternalModel}"/>.</param>
    /// <param name="properties">Properties as expressions to look for changes on.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <returns><see cref="IImportActionBuilderFor{TModel, TExternalModel}"/> for chaining.</returns>
    public static IImportActionBuilderFor<TModel, TExternalModel> WithPropertiesBecomingNull<TModel, TExternalModel>(this IImportActionBuilderFor<TModel, TExternalModel> builder, params Expression<Func<TModel, object>>[] properties)
    {
        var propertyPaths = properties.Select(_ => _.GetPropertyPath()).ToArray();

        return new ImportActionBuilderFor<TModel, TExternalModel>(builder.Where(_ =>
        {
            var changes = _.Changeset.Changes.Where(_ => _ is PropertiesChanged<TModel>).Select(_ => _ as PropertiesChanged<TModel>);
            return changes.Any(_ => _!.Differences.Any(_ => _.Original is not null && _.Changed is null && propertyPaths.Any(p => _.PropertyPath.Path.StartsWith(p.Path))));
        }), builder);
    }

    /// <summary>
    /// Append an event by automatically mapping property names matching from the model onto the event.
    /// </summary>
    /// <param name="builder">Observable of the <see cref="ImportContext{TModel, TExternalModel}"/>.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <typeparam name="TEvent">Type of event to append.</typeparam>
    /// <returns><see cref="IImportActionBuilderFor{TModel, TExternalModel}"/> for chaining.</returns>
    public static IImportActionBuilderFor<TModel, TExternalModel> AppendEvent<TModel, TExternalModel, TEvent>(this IImportActionBuilderFor<TModel, TExternalModel> builder)
    {
        builder.AddProducingEventType(typeof(TEvent).GetEventType());
        builder.Subscribe(_ =>
        {
            foreach (var change in _.Changeset.Changes.Where(_ => _ is PropertiesChanged<TModel>).Select(_ => _ as PropertiesChanged<TModel>))
            {
                try
                {
                    var mapper = ModelToEventMapperFor<TModel, TEvent>.Mapper;
                    _.Events.Add(mapper.Map<TEvent>(((TModel)change!.State)!)!);
                }
                catch (TypeInitializationException ex)
                {
                    throw ex.InnerException!;
                }
            }
        });

        return builder;
    }

    /// <summary>
    /// Append an event through calling a callback that will be responsible for creating an instance of the event.
    /// </summary>
    /// <param name="builder">Observable of the <see cref="ImportContext{TModel, TExternalModel}"/>.</param>
    /// <param name="creationCallback">Callback for creating the instance.</param>
    /// <typeparam name="TModel">Type of model.</typeparam>
    /// <typeparam name="TExternalModel">Type of external model.</typeparam>
    /// <typeparam name="TEvent">Type of event to append.</typeparam>
    /// <returns><see cref="IImportActionBuilderFor{TModel, TExternalModel}"/> for chaining.</returns>
    public static IImportActionBuilderFor<TModel, TExternalModel> AppendEvent<TModel, TExternalModel, TEvent>(this IImportActionBuilderFor<TModel, TExternalModel> builder, Func<ImportContext<TModel, TExternalModel>, TEvent> creationCallback)
    {
        builder.AddProducingEventType(typeof(TEvent).GetEventType());
        builder.Subscribe(_ => _.Events.Add(creationCallback(_)!));
        return builder;
    }
}
