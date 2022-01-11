// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.Connections
{
    /// <summary>
    /// The concept representing a unique identifier for a client connection to the Kernel.
    /// </summary>
    /// <param name="Value">The inner value.</param>
    public record ConnectionId(string Value) : ConceptAs<string>(Value)
    {
        /// <summary>
        /// Implicitly convert from a <see cref="string"/> to <see cref="ConnectionId"/>.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        public static implicit operator ConnectionId(string value) => new(value);
    }
}
