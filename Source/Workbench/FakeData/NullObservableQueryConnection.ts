// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { DataReceived, IObservableQueryConnection } from '@aksio/applications/queries';

export class NullObservableQueryConnection<TDataType> implements IObservableQueryConnection<TDataType> {
    connect(dataReceived: DataReceived<TDataType>) {

    }
    disconnect() {

    }
}
