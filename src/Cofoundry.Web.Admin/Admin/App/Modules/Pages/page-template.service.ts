import { Injectable }    from '@angular/core';
import { Headers, Http, RequestOptionsArgs } from '@angular/http';
import { SERVICE_BASE } from '../shared/constants/path.constants';

@Injectable()
export class PageTemplateService {
    pageTemplateServiceBase = SERVICE_BASE + 'page-templates';

    constructor(private http: Http) {}

    /* QUERIES */

    getAll() {
        return this.http
            .get(this.pageTemplateServiceBase);
    }
}