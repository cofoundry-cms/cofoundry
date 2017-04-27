import { Injectable }    from '@angular/core';
import { Headers, Http, RequestOptionsArgs } from '@angular/http';
import { SERVICE_BASE } from '../../constants/path.constants';

@Injectable()
export default class EntityVersionService {
    pageServiceBase = SERVICE_BASE + 'pages';
    customEntityServiceBase = SERVICE_BASE + 'custom-entities';

    constructor(private http: Http) {}

    /* COMMANDS */

    publish(isCustomEntity, entityId) {
        return this.http
            .patch(this.getVersionsRoute(isCustomEntity, entityId) + '/draft/publish', null);
    }

    unpublish(isCustomEntity, entityId) {
        return this.http
            .patch(this.getVersionsRoute(isCustomEntity, entityId) + '/published/unpublish', null);
    }

    duplicateDraft(isCustomEntity, entityId, entityVersionId) {
        var command;

        if (isCustomEntity) {
            command = {
                customEntityId: entityId,
                copyFromCustomEntityVersionId: entityVersionId
            }
        } else {
            command = {
                pageId: entityId,
                copyFromPageVersionId: entityVersionId
            }
        }

        return this.http
            .post(this.getVersionsRoute(isCustomEntity, entityId), command);
    }

    removeDraft(isCustomEntity, entityId) {

        return this.http
            .delete(this.getVersionsRoute(isCustomEntity, entityId) + '/draft');
    }
    
    /* HELPERS */

    getVersionsRoute (isCustomEntity, entityId) {
        return this.getIdRoute(isCustomEntity, entityId) + '/versions';
    }

    getIdRoute(isCustomEntity, entityId) {
        return this.getServiceBase(isCustomEntity) + '/' + entityId;
    }

    getServiceBase(isCustomEntity) {
        return isCustomEntity ? this.customEntityServiceBase : this.pageServiceBase;
    }
}