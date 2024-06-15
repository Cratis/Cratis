// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;

namespace Cratis.Chronicle.Changes.for_ObjectComparer;

public class when_comparing_expando_objects_and_left_is_null_and_right_has_properties : given.an_object_comparer
{
    dynamic left;
    dynamic right;

    bool result;

    IEnumerable<PropertyDifference> differences;

    void Establish()
    {
        left = null;
        right = new ExpandoObject();
        right.StringValue = "FortyTwo";
        right.IntValue = 44;
    }

    void Because() => result = comparer.Equals(left, right, out differences);

    [Fact] void should_not_be_considered_equal() => result.ShouldBeFalse();
    [Fact] void should_have_first_difference_be_the_string_value() => differences.ToArray()[0].PropertyPath.Path.ShouldEqual("stringValue");
    [Fact] void should_have_second_difference_be_the_int_value() => differences.ToArray()[1].PropertyPath.Path.ShouldEqual("intValue");
    [Fact] void should_hold_changed_value_for_the_string_value() => differences.ToArray()[0].Changed.ShouldEqual((string)right.StringValue);
    [Fact] void should_hold_changed_value_for_the_int_value() => differences.ToArray()[1].Changed.ShouldEqual((int)right.IntValue);
}
