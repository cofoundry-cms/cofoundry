/**
 * Some helper functions that can be used to determine if a user has permission to access
 * to various entities and actions.
 */
angular.module('cms.shared').factory('shared.permissionValidationService', [
    '_',
    'shared.currentUser',
function (
    _,
    currentUser
    ) {

    var service = {},
        COMMON_PERMISSION_CODE_CMSMODULE = 'COMMOD',
        COMMON_PERMISSION_CODE_READ = 'COMRED',
        COMMON_PERMISSION_CODE_CREATE = 'COMCRT',
        COMMON_PERMISSION_CODE_UPDATE = 'COMUPD',
        COMMON_PERMISSION_CODE_DELETE = 'COMDEL';

    /**
     * Determines if the current user has access to a specific permission. Permission codes
     * are formatted {EntityDefinitionCode}{PermissionTypeCode}, e.g. 'CMSPAGCOMRED' or 'ERRLOG' if there 
     * is no EntityDefinitionCode
     */
    service.hasPermission = function (permissionCode) {
        return _.contains(currentUser.permissionCodes, permissionCode);
    }

    /**
     * Determines if the user is permitted to access any data relating to the entity. Read permissions
     * are required for any other entity permission, so you don't need to check (for example) both Create 
     * and Read permissions, just Create. Pass only the EntityDefinitionCode
     */
    service.canRead = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_READ);
    }

    /**
     * Determines if the user can view the CMS module associated with the entity. Pass only
     * the EntityDefinitionCode
     */
    service.canViewModule = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_CMSMODULE);
    }

    /**
     * Determines if the user can add new entities of this type. Pass only
     * the EntityDefinitionCode
     */
    service.canCreate = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_CREATE);
    }

    /**
     * Determines if the user can update entities of this type. Pass only
     * the EntityDefinitionCode
     */
    service.canUpdate = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_UPDATE);
    }

    /**
     * Determines if the user can delete entities of this type. Pass only
     * the EntityDefinitionCode
     */
    service.canDelete = function (entityDefinitionCode) {
        return service.hasPermission(entityDefinitionCode + COMMON_PERMISSION_CODE_DELETE);
    }

    return service;
}]);