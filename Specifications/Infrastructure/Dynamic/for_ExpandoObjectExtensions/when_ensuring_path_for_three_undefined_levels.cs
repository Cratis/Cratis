// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Cratis.Properties;

namespace Cratis.Dynamic.for_ExpandoObjectExtensions;

public class when_ensuring_path_for_three_undefined_levels : Specification
{
    ExpandoObject result;

    void Because() => result = new ExpandoObject().EnsurePath("first_level.second_level.third_level", ArrayIndexers.NoIndexers);

    [Fact] void should_return_object() => result.ShouldNotBeNull();
}
