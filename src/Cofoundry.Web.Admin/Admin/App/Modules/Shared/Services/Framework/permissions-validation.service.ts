import { Injectable } from '@angular/core';
import * as _ from 'lodash';

/**
 * Some helper functions that can be used to determine if a user has permission to access
 * to various entities and actions.
 */

@Injectable()
export default class PermissionValidationService {
    /*
    TODO: Current user should come from a service api
    */
    currentUser = {
        permissionCodes: []
    };

    COMMON_PERMISSION_CODE_CMSMODULE = 'COMMOD';
    COMMON_PERMISSION_CODE_READ = 'COMRED';
    COMMON_PERMISSION_CODE_CREATE = 'COMCRT';
    COMMON_PERMISSION_CODE_UPDATE = 'COMUPD';
    COMMON_PERMISSION_CODE_DELETE = 'COMDEL';

    /**
     * Determines if the current user has access to a specific permission. Permission codes
     * are formatted {EntityDefinitionCode}{PermissionTypeCode}, e.g. 'CMSPAGCOMRED' or 'ERRLOG' if there 
     * is no EntityDefinitionCode
     */
    private hasPermission (permissionCode) {
        return _.includes(this.currentUser.permissionCodes, permissionCode);
    }

    /**
     * Determines if the user is permitted to access any data relating to the entity. Read permissions
     * are required for any other entity permission, so you don't need to check (for example) both Create 
     * and Read permissions, just Create. Pass only the EntityDefinitionCode
     */
    canRead(entityDefinitionCode) {
        return this.hasPermission(entityDefinitionCode + this.COMMON_PERMISSION_CODE_READ);
    }

    /**
     * Determines if the user can view the CMS module associated with the entity. Pass only
     * the EntityDefinitionCode
     */
    canViewModule(entityDefinitionCode) {
        return this.hasPermission(entityDefinitionCode + this.COMMON_PERMISSION_CODE_CMSMODULE);
    }

    /**
     * Determines if the user can add new entities of this type. Pass only
     * the EntityDefinitionCode
     */
    canCreate(entityDefinitionCode) {
        return this.hasPermission(entityDefinitionCode + this.COMMON_PERMISSION_CODE_CREATE);
    }

    /**
     * Determines if the user can update entities of this type. Pass only
     * the EntityDefinitionCode
     */
    canUpdate(entityDefinitionCode) {
        return this.hasPermission(entityDefinitionCode + this.COMMON_PERMISSION_CODE_UPDATE);
    }

    /**
     * Determines if the user can delete entities of this type. Pass only
     * the EntityDefinitionCode
     */
    canDelete(entityDefinitionCode) {
        return this.hasPermission(entityDefinitionCode + this.COMMON_PERMISSION_CODE_DELETE);
    }
}