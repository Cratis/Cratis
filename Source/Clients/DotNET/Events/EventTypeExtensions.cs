// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Aksio.Cratis.Reflection;

namespace Aksio.Cratis.Events;

/// <summary>
/// Extension methods for working with event types.
/// </summary>
public static class EventTypeExtensions
{
    /// <summary>
    /// Get the <see cref="EventType"/> from a CLR <see cref="Type"/>.
    /// </summary>
    /// <param name="type"><see cref="Type"/> to get from.</param>
    /// <returns>The <see cref="EventType"/>.</returns>
    /// <exception cref="MissingEventTypeAttribute">Thrown if CLR type is missing the <see cref="EventTypeAttribute"/>.></exception>
    public static EventType GetEventType(this Type type)
    {
        if (!type.HasAttribute<EventTypeAttribute>())
        {
            throw new MissingEventTypeAttribute(type);
        }

        return type.GetCustomAttribute<EventTypeAttribute>()!.Type!;
    }
}
