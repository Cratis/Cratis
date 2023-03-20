// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import {
    CommandBar,
    ICommandBarItemProps,
    Dropdown,
    IDropdownOption,
    IDropdownStyles,
    Label,
    Stack,
    Selection,
    SelectionMode
} from '@fluentui/react';
import { AllMicroservices } from 'API/configuration/microservices/AllMicroservices';
import { useEffect, useState, useMemo } from 'react';
import { Microservice } from 'API/configuration/microservices/Microservice';
import { ScrollableDetailsList } from '@aksio/cratis-fluentui';
import { IColumn, Panel, TextField } from '@fluentui/react';
import { AllFailedPartitions } from 'API/events/store/failed-partitions/AllFailedPartitions';
import { AllTenants } from 'API/configuration/tenants/AllTenants';
import { TenantInfo } from 'API/configuration/tenants/TenantInfo';
import { RecoverFailedPartitionState } from 'API/events/store/failed-partitions/RecoverFailedPartitionState';
import { useBoolean } from '@fluentui/react-hooks';
import { AllEventSequences } from 'API/events/store/sequences/AllEventSequences';
import { EventSequenceInformation } from '../../API/events/store/sequences/EventSequenceInformation';
import { QueryResultWithState } from '@aksio/cratis-applications-frontend/queries';


const commandBarDropdownStyles: Partial<IDropdownStyles> = { dropdown: { width: 200, marginLeft: 8, marginTop: 8 } };

let eventSequences: QueryResultWithState<EventSequenceInformation[]>;

const columns: IColumn[] = [
    {
        key: 'observerId',
        name: 'Observer Id',
        fieldName: 'observerId',
        minWidth: 250,
        maxWidth: 250,
    },
    {
        key: 'observerName',
        name: 'Observer Name',
        fieldName: 'observerName',
        minWidth: 250,
        maxWidth: 250,
    },
    {
        key: 'attempts',
        name: 'Attempts',
        fieldName: 'numberOfAttemptsOnSinceInitialized',
        minWidth: 100,
        maxWidth: 100,
    },
    {
        key: 'partition',
        name: 'Partition',
        fieldName: 'partition',
        minWidth: 250,
        maxWidth: 250,
    },
    {
        key: 'sequenceNumber',
        name: 'Sequence Number',
        fieldName: 'currentError',
        minWidth: 120,
        maxWidth: 120,
    },
    {
        key: 'sequence',
        name: 'Event Sequence',
        fieldName: 'eventSequenceId',
        minWidth: 120,
        maxWidth: 120,
        onRender: (item: RecoverFailedPartitionState) => {
            return (
                <>{eventSequences.data.find(_ => _.id == item.eventSequenceId)?.name ?? item.id}</>
            );
        }
    },
    {
        key: 'occurred',
        name: 'Occurred',
        fieldName: 'initialPartitionFailedOn',
        minWidth: 250,
        maxWidth: 250,
        onRender: (item: RecoverFailedPartitionState) => {
            return (
                <>{item.initialPartitionFailedOn ? new Date(item.initialPartitionFailedOn).toLocaleString() : ''}</>
            );
        }
    }
];

export const FailedPartitions = () => {
    const [es] = AllEventSequences.use();
    eventSequences = es;

    const [microservices] = AllMicroservices.use();
    const [tenants] = AllTenants.use();
    const [selectedMicroservice, setSelectedMicroservice] = useState<Microservice>();
    const [selectedTenant, setSelectedTenant] = useState<TenantInfo>();
    const [isDetailsPanelOpen, { setTrue: openPanel, setFalse: dismissPanel }] = useBoolean(false);
    const [selectedItem, setSelectedItem] = useState<RecoverFailedPartitionState>();

    const [failedPartitions] = AllFailedPartitions.use({
        microserviceId: selectedMicroservice?.id ?? undefined!,
        tenantId: selectedTenant?.id ?? undefined!
    });

    const microserviceOptions = microservices.data.map(_ => {
        return {
            key: _.id,
            text: _.name
        } as IDropdownOption;
    });

    const tenantOptions = tenants.data.map(_ => {
        return {
            key: _.id,
            text: _.name
        } as IDropdownOption;
    });

    useEffect(() => {
        if (microservices.data.length > 0) {
            setSelectedMicroservice(microservices.data[0]);
        }
    }, [microservices.data]);

    useEffect(() => {
        if (tenants.data.length > 0) {
            setSelectedTenant(tenants.data[0]);
        }
    }, [tenants.data]);

    const commandBarItems: ICommandBarItemProps[] = [
        {
            key: 'microservice',
            text: 'Microservice',
            onRender: () => {
                return (
                    <Dropdown
                        styles={commandBarDropdownStyles}
                        options={microserviceOptions}
                        selectedKey={selectedMicroservice?.id}
                        onChange={(e, option) => {
                            setSelectedMicroservice(microservices.data.find(_ => _.id == option!.key));
                        }} />
                );
            }
        },
        {
            key: 'tenant',
            text: 'Tenant',
            onRender: () => {
                return (
                    <Dropdown
                        styles={commandBarDropdownStyles}
                        options={tenantOptions}
                        selectedKey={selectedTenant?.id}
                        onChange={(e, option) => {
                            setSelectedTenant(tenants.data.find(_ => _.id == option!.key));
                        }} />
                );
            }
        }
    ];

    const closePanel = () => {
        setSelectedItem(undefined);
        dismissPanel();
    };

    const selection = useMemo(
        () => new Selection({
            selectionMode: SelectionMode.single,
            onSelectionChanged: () => {
                const selected = selection.getSelection();
                if (selected.length === 1) {
                    setSelectedItem(selected[0] as RecoverFailedPartitionState);
                    openPanel();
                }
            },
            items: failedPartitions.data as any[]
        }), [failedPartitions.data]);


    return (
        <>
            <Stack style={{ height: '100%' }}>
                <CommandBar items={commandBarItems} />
                <ScrollableDetailsList
                    columns={columns}
                    items={failedPartitions.data}
                    selection={selection}

                />
            </Stack>
            <Panel
                isLightDismiss
                isOpen={isDetailsPanelOpen}
                onDismiss={closePanel}
                headerText={selectedItem?.observerName}>
                <TextField label="Occurred" disabled defaultValue={selectedItem?.initialPartitionFailedOn.toLocaleDateString() ?? new Date().toLocaleString()} />
                <Label>Messages</Label>
                {
                    (selectedItem?.messages) && selectedItem.messages.map((value, index) => <TextField key={index} disabled defaultValue={value.toString()} title={value.toString()} />)
                }
                <TextField label="Stack Trace" disabled defaultValue={selectedItem?.stackTrace} multiline title={selectedItem?.stackTrace.toString()} />
                {/* {

                    (selectedItem) && Object.keys(selectedItem).map(_ => <TextField key={_} label={_} disabled defaultValue={selectedItem![_]} />)
                } */}
            </Panel>
        </>

    );
};