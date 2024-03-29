// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Json.for_TypeWithObjectPropertiesJsonConverter;

public class Converter : TypeWithObjectPropertiesJsonConverter<TypeWithObjectProperties>
{
    protected override IEnumerable<string> ObjectProperties => new[] {
        nameof(TypeWithObjectProperties.FirstObjectProperty),
        nameof(TypeWithObjectProperties.SecondObjectProperty)
    };
}
