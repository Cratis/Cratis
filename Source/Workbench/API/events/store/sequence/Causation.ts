/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { field } from '@aksio/fundamentals';


export class Causation {

    @field(Date)
    occurred!: Date;

    @field(String)
    type!: string;

    @field(Object)
    properties!: [key: string, value: string];
}
