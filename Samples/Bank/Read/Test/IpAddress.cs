// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Concepts.Infrastructure
{
    public record IpAddress(string Value) : ConceptAs<string>(Value)
    {
        public static implicit operator IpAddress(string address) => new(address);
    }
}
