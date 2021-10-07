// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;

namespace Cratis.Events.Projections
{
    /// <summary>
    /// Represents properties that has been changed.
    /// </summary>
    /// <param name="State">State after change applied.</param>
    public record PropertiesChanged(ExpandoObject State) : Change(State);
}
