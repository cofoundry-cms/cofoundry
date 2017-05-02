import { Injectable }    from '@angular/core';
import { Headers, Http, RequestOptionsArgs } from '@angular/http';
import { SERVICE_BASE } from '../../constants/path.constants';

@Injectable()
export default class DocumentService {
    private http: Http;
    private documentsServiceBase = SERVICE_BASE + 'documents';

    constructor(http: Http) {
        this.http = http;
    }

    getAll(query) {
        return this.http
            .get(this.documentsServiceBase, {
                search: query
            });
    }


    getById(documentId) {
        return this.http
            .get(this.getIdRoute(documentId));
    }

    getAllDocumentFileTypes() {
        return this.http
            .get(SERVICE_BASE + 'document-file-types');
    }

    private getIdRoute(documentId) {
        return this.documentsServiceBase + '/' + documentId;
    }

    private getBaseRoute() {
        return this.documentsServiceBase;
    }
}
