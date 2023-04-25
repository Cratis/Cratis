/*---------------------------------------------------------------------------------------------
 *  **DO NOT EDIT** - This file is an automatically generated file.
 *--------------------------------------------------------------------------------------------*/

import { QueryFor, QueryResultWithState, useQuery, PerformQuery } from '@aksio/cratis-applications-frontend/queries';
import { JsonResult } from './JsonResult';
import Handlebars from 'handlebars';

const routeTemplate = Handlebars.compile('/api/events/store/pivotviewer//api/events/store/images/imagelist.json');

export class ImageList extends QueryFor<JsonResult> {
    readonly route: string = '/api/events/store/pivotviewer//api/events/store/images/imagelist.json';
    readonly routeTemplate: Handlebars.TemplateDelegate = routeTemplate;
    readonly defaultValue: JsonResult = {} as any;

    constructor() {
        super(JsonResult, false);
    }

    get requestArguments(): string[] {
        return [
        ];
    }

    static use(): [QueryResultWithState<JsonResult>, PerformQuery] {
        return useQuery<JsonResult, ImageList>(ImageList);
    }
}
