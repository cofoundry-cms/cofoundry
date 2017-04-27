import { Injectable }    from '@angular/core';
import { Headers, Http } from '@angular/http';
import { SERVICE_BASE } from '../shared/constants/path.constants';

@Injectable()
export class DashboardService {
    constructor(private http : Http) {}
    
    pagingParams = {
    	pageSize: 5
    };

    /* QUERIES */

    getPages() {
        return this.http.get(SERVICE_BASE + 'pages', this.getParams({}));
    }

    getDraftPages() {
        return this.http.get(SERVICE_BASE + 'pages', this.getParams({
            workFlowStatus: 'Draft'
        }));
    }

    getUsers() {
        return this.http.get(SERVICE_BASE + 'users', this.getParams({
            userAreaCode: 'CMS'
        }));
    }

    getPageTemplates() {
        return this.http.get(SERVICE_BASE + 'page-templates', this.getParams({}));
    }

    getParams(additionalParams) {
        var newParams = this.pagingParams;

        if (additionalParams) {
            newParams = Object.assign(this.pagingParams, additionalParams);
        };

        return {
            params: newParams
        };
    }
};