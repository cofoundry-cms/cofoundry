import { Injectable }    from '@angular/core';
import { Http, URLSearchParams } from '@angular/http';
import { SERVICE_BASE } from '../../constants/path.constants';
import * as _ from 'lodash';

@Injectable()
export default class CustomEntityService {
    private http: Http;

    constructor(http: Http) {
        this.http = http;
    }

    customEntityDefinitionServiceBase = SERVICE_BASE + 'custom-entity-definitions/';
    customEntityServiceBase = SERVICE_BASE + 'custom-entities';

    /* QUERIES */

    getAll(customEntityDefinitionCode, query) {
        return this.http
            .get(this.customEntityDefinitionServiceBase + customEntityDefinitionCode + '/custom-entities', {
                search: query
            });
    }

    getDefinition(customEntityDefinitionCode) {
        return this.http
            .get(this.customEntityDefinitionServiceBase + customEntityDefinitionCode);
    }

    getDefinitionsByIdRange(customEntityDefinitionCodes) {
        return this.http
            .get(this.customEntityDefinitionServiceBase)
            .subscribe(filterByIdRange);

        function filterByIdRange(results) {
            return _.filter(results, (result: any) => {
                return _.includes(customEntityDefinitionCodes, result.customEntityDefinitionCode);
            });
        }
    }

    getByIdRange(ids) {
        let searchParams = new URLSearchParams();
        searchParams.set('ids', ids);
        
        return this.http
            .get(this.customEntityServiceBase + '/', {
                search: searchParams
            });
    }
}