/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { field } from '@aksio/fundamentals';

import { Causation } from './Causation';
import { CausedBy } from './CausedBy';
import { EventObservationState } from './EventObservationState';

export class EventContext {

    @field(String)
    eventSourceId!: string;

    @field(Number)
    sequenceNumber!: number;

    @field(Date)
    occurred!: Date;

    @field(Date)
    validFrom!: Date;

    @field(String)
    tenantId!: string;

    @field(String)
    correlationId!: string;

    @field(Causation, true)
    causation!: Causation[];

    @field(CausedBy)
    causedBy!: CausedBy;

    @field(Number)
    observationState!: EventObservationState;
}
