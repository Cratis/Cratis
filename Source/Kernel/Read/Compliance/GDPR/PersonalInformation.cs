// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Kernel.Concepts.Compliance.PersonalInformation;

namespace Aksio.Cratis.Kernel.Read.Compliance.GDPR;

/// <summary>
/// Represents personal information for a person.
/// </summary>
/// <param name="Identifier">The unique <see cref="PersonalInformationId"/>.</param>
/// <param name="Type">Type of value.</param>
/// <param name="Value">The actual value.</param>
public record PersonalInformation(PersonalInformationId Identifier, PersonalInformationType Type, PersonalInformationValue Value);
