// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events.Projections.Definitions;
using Aksio.Cratis.Events.Projections.Expressions;
using Aksio.Cratis.Events.Store;
using Aksio.Cratis.Properties;
using NJsonSchema;

namespace Aksio.Cratis.Events.Projections
{
    /// <summary>
    /// Represents an implementation of <see cref="IProjectionFactory"/>.
    /// </summary>
    public class ProjectionFactory : IProjectionFactory
    {
        readonly IPropertyMapperExpressionResolvers _propertyMapperExpressionResolvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionFactory"/> class.
        /// </summary>
        /// <param name="propertyMapperExpressionResolvers"><see cref="IPropertyMapperExpressionResolvers"/> for resolving expressions for properties.</param>
        public ProjectionFactory(IPropertyMapperExpressionResolvers propertyMapperExpressionResolvers)
        {
            _propertyMapperExpressionResolvers = propertyMapperExpressionResolvers;
        }

        /// <inheritdoc/>
        public Task<IProjection> CreateFrom(ProjectionDefinition definition) =>
            CreateProjectionFrom(
                definition.Name,
                definition,
                PropertyPath.Root,
                PropertyPath.Root,
                PropertyPath.Root,
                PropertyPath.Root,
                ProjectionPath.GetRootFor(definition.Identifier),
                new Dictionary<PropertyPath, ChildrenDefinition>(),
                _ => { });

        async Task<IProjection> CreateProjectionFrom(
            ProjectionName name,
            ProjectionDefinition projectionDefinition,
            PropertyPath parentProperty,
            PropertyPath childrenAccessorProperty,
            PropertyPath parentIdentifiedByProperty,
            PropertyPath identifiedByProperty,
            ProjectionPath path,
            IDictionary<PropertyPath, ChildrenDefinition> childrenDefinitions,
            Action<IEnumerable<EventTypeWithKeyResolver>> addChildEventTypes)
        {
            var eventsForProjection = projectionDefinition.From.Select(kvp => new EventTypeWithKeyResolver(
                kvp.Key,
                string.IsNullOrEmpty(kvp.Value.ParentKey) ? EventValueProviders.FromEventSourceId : EventValueProviders.FromEventContent(kvp.Value.ParentKey!))).ToList();

            if (projectionDefinition.RemovedWith != default)
            {
                eventsForProjection.Add(new EventTypeWithKeyResolver(projectionDefinition.RemovedWith.Event, EventValueProviders.FromEventSourceId));
            }

            var childProjectionTasks = projectionDefinition.Children.Select(async kvp => await CreateProjectionFrom(
                    name,
                    kvp.Value,
                    childrenAccessorProperty,
                    childrenAccessorProperty + kvp.Key,
                    identifiedByProperty,
                    kvp.Value.IdentifiedBy,
                    $"{path} -> ChildrenAt({kvp.Key.Path})",
                    projectionDefinition.Children,
                    _ =>
                    {
                        eventsForProjection.AddRange(_);
                        addChildEventTypes(_);
                    }));
            var childProjections = await Task.WhenAll(childProjectionTasks.ToArray());

            var modelSchema = await JsonSchema.FromJsonAsync(projectionDefinition.Model.Schema);
            var model = new Model(projectionDefinition.Model.Name, modelSchema);
            addChildEventTypes(eventsForProjection);

            var projection = new Projection(
                projectionDefinition.Identifier,
                name,
                path,
                model,
                projectionDefinition.IsPassive,
                projectionDefinition.IsRewindable,
                eventsForProjection.DistinctBy(_ => _.EventType),
                childProjections);

            foreach (var (childrenProperty, childrenDefinition) in childrenDefinitions)
            {
                foreach (var (eventType, fromDefinition) in childrenDefinition.From)
                {
                    var fullyQualifiedChildrenProperty = parentProperty + childrenProperty;

                    var propertyMappers = fromDefinition.Properties.Select(kvp => _propertyMapperExpressionResolvers.Resolve(kvp.Key, kvp.Value));
                    var (actualIdentifiedByProperty, actualKeyResolver) = ResolveIdentifiedPropertyWithKeyResolver(childrenDefinition.IdentifiedBy, fromDefinition.Key);
                    var (actualParentIdentifiedByProperty, actualParentKeyResolver) = ResolveIdentifiedPropertyWithKeyResolver(parentIdentifiedByProperty, fromDefinition.ParentKey);
                    projection.Event.From(eventType).Child(
                        fullyQualifiedChildrenProperty,
                        actualParentIdentifiedByProperty,
                        actualParentKeyResolver,
                        actualIdentifiedByProperty,
                        actualKeyResolver,
                        propertyMappers);
                }
            }

            foreach (var (eventType, fromDefinition) in projectionDefinition.From)
            {
                var propertyMappers = fromDefinition.Properties.Select(kvp => _propertyMapperExpressionResolvers.Resolve(kvp.Key, kvp.Value));
                var (actualIdentifiedByProperty, actualKeyResolver) = ResolveIdentifiedPropertyWithKeyResolver(identifiedByProperty, fromDefinition.Key);
                projection.Event.From(eventType).Project(childrenAccessorProperty, parentIdentifiedByProperty, actualIdentifiedByProperty, actualKeyResolver, propertyMappers);
            }

            if (projectionDefinition.RemovedWith != default)
            {
                projection.Event.RemovedWith(projectionDefinition.RemovedWith.Event);
            }

            return projection;
        }

        (PropertyPath Property, ValueProvider<AppendedEvent> KeyResolver) ResolveIdentifiedPropertyWithKeyResolver(PropertyPath identifiedByProperty, string? key = default)
        {
            if (identifiedByProperty.IsRoot)
            {
                return (Property: "_id", KeyResolver: EventValueProviders.UniqueIdentifier());
            }

            return (Property: identifiedByProperty, KeyResolver: !string.IsNullOrEmpty(key) ? EventValueProviders.FromEventContent(key) : EventValueProviders.FromEventSourceId);
        }
    }
}
