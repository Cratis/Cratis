// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Events;

namespace Aksio.Cratis.Integration.for_ImportBuilderExtensions;

[EventType("2df7fc25-363a-43ba-9506-544aab61ef57", 1)]
public record SomeEventWithMoreProperties(int SomeInteger, string SomeString, int UnmatchedProperty);
