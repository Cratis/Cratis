// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Chronicle.Specifications;

namespace Aksio.Cratis.Specifications.given;

public class no_events : Specification
{
    protected List<AppendedEventForSpecifications> events;

    void Establish() => events = new();
}
