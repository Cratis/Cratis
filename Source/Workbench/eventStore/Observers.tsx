// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { useState, useEffect, useMemo } from 'react';
import { AllObservers } from 'API/events/store/observers/AllObservers';
import { AllTenants } from 'API/configuration/tenants/AllTenants';
import { TenantInfo } from 'API/configuration/tenants/TenantInfo';
import { AllObserversArguments } from 'API/events/store/observers/AllObservers';
import { ObserverInformation } from 'API/events/store/observers/ObserverInformation';
import { Replay } from 'API/events/store/observers/Replay';
import { DataGrid, GridCallbackDetails, GridColDef, GridRowSelectionModel, GridValueGetterParams } from '@mui/x-data-grid';
import { Box, Button, Divider, FormControl, InputLabel, MenuItem, Select, Stack, Toolbar, Typography } from '@mui/material';
import { useRouteParams } from './RouteParams';
import * as icons from '@mui/icons-material';

const observerRunningStates: { [key: number]: string; } = {
    0: 'Unknown',
    1: 'Subscribing',
    2: 'Replaying',
    3: 'Catching up',
    4: 'Active',
    5: 'Paused',
    6: 'Stopped',
    7: 'Suspended',
    8: 'Failed',
    9: 'Tail of replay',
    10: 'Disconnected'
};

const observerTypes: { [key: number]: string; } = {
    0: 'Unknown',
    1: 'Client',
    2: 'Projection',
    3: 'Inbox',
    4: 'Reducer'
};

const columns: GridColDef[] = [
    {
        headerName: 'Id',
        field: 'observerId',
        width: 250
    },
    {
        headerName: 'Name',
        field: 'name',
        width: 300,
    },
    {
        headerName: 'Type',
        field: 'type',
        width: 200,
        valueGetter: (params: GridValueGetterParams<ObserverInformation>) => {
            return observerTypes[params.row.type as number];
        }
    },
    {
        headerName: 'State',
        field: 'runningState',
        width: 200,
        valueGetter: (params: GridValueGetterParams<ObserverInformation>) => {
            return observerRunningStates[params.row.runningState as number];
        }
    },
    {
        headerName: 'Next Event',
        field: 'nextEventSequenceNumber',
        width: 200,
    },
];

export const Observers = () => {
    const { microserviceId } = useRouteParams();

    const [tenants] = AllTenants.use();
    const [selectedTenant, setSelectedTenant] = useState<TenantInfo>();

    const [selectedObserver, setSelectedObserver] = useState<ObserverInformation>();

    const getAllObserversArguments = () => {
        return {
            microserviceId: microserviceId,
            tenantId: selectedTenant?.id || undefined
        } as AllObserversArguments;
    };

    const [observers] = AllObservers.use(getAllObserversArguments());

    useEffect(() => {
        if (tenants.data.length > 0) {
            setSelectedTenant(tenants.data[0]);
        }
    }, [tenants.data]);

    const [replayCommand, setReplayCommandVales] = Replay.use();

    const observerSelected = (selectionModel: GridRowSelectionModel, details: GridCallbackDetails) => {
        const selectedItems = selectionModel.map(_ => observers.data.find(__ => __.observerId == _)) as ObserverInformation[];
        if (selectedItems.length > 0) {
            setSelectedObserver(selectedItems[0]);
        }
    };

    return (
        <Stack direction="column" style={{ height: '100%' }}>
            <Typography variant='h4'>Observers</Typography>
            <Divider sx={{ mt: 1, mb: 3 }} />

            <Toolbar>
                <FormControl size="small" sx={{ m: 1, minWidth: 120 }}>
                    <InputLabel>Tenant</InputLabel>
                    <Select
                        label="Tenant"
                        autoWidth
                        value={selectedTenant?.id || ''}
                        onChange={e => setSelectedTenant(tenants.data.find(_ => _.id == e.target.value))}>

                        {tenants.data.map(tenant => {
                            return (
                                <MenuItem key={tenant.id} value={tenant.id}>{tenant.name}</MenuItem>
                            );
                        })}
                    </Select>
                </FormControl>

                {selectedObserver &&
                    <Button
                        startIcon={<icons.Replay />}
                        onClick={() => {
                            setReplayCommandVales({
                                observerId: selectedObserver?.observerId,
                                microserviceId: microserviceId,
                                tenantId: selectedTenant?.id
                            });
                            replayCommand.execute();
                        }}>Replay</Button>
                }

            </Toolbar>

            <Box sx={{ height: '100%', flex: 1 }}>
                <DataGrid
                    columns={columns}
                    filterMode="client"
                    sortingMode="client"
                    getRowId={row => row.observerId}
                    rows={observers.data}
                    onRowSelectionModelChange={observerSelected}
                />
            </Box>
        </Stack>
    );
};
