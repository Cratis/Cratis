// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { ConnectedClientsForMicroservice } from 'API/clients/ConnectedClientsForMicroservice';
import { useRouteParams } from '../eventStore/RouteParams';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Box } from '@mui/material';
import { ConnectedClient } from 'API/clients/ConnectedClient';

const columns: GridColDef[] = [
    {
        headerName: 'Id',
        field: 'connectionId',
        width: 250,
    },
    {
        headerName: 'Client Uri',
        field: 'clientUri',
        width: 300,
    },
    {
        headerName: 'Version',
        field: 'version',
        width: 300,
    },
    {
        headerName: 'Last Seen',
        field: 'lastSeen',
        width: 300,
    },
    {
        headerName: 'Debugger Attached',
        field: 'isRunningWithDebugger',
        width: 300,
    }
];

export const ConnectedClients = () => {
    const { microserviceId } = useRouteParams();
    const [connectedClients] = ConnectedClientsForMicroservice.use({ microserviceId });

    return (
        <Box sx={{ height: 400 }}>
            <DataGrid
                columns={columns}
                filterMode="client"
                sortingMode="client"
                getRowId={(row: ConnectedClient) => row.connectionId}
                rows={connectedClients.data}
            />
        </Box>
    );
};
