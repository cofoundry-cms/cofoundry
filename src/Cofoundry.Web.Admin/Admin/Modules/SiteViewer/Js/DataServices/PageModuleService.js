/**
 * Service for managing page modules, which can either be attached to a page or a custom entity. 
 * Pass in the isCustomEntityRoute to switch between either route endpoint.
 */
angular.module('cms.siteViewer').factory('siteViewer.pageModuleService', [
    '$http',
    'shared.serviceBase',
    'siteViewer.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        pageModulesServiceBase = serviceBase + 'page-version-section-modules',
        customEntityModulesServiceBase = serviceBase + 'custom-entity-version-page-modules';

    /* QUERIES */

    service.getPageVersionModuleById = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.get(getIdRoute(isCustomEntityRoute, pageVersionModuleId) + '?datatype=updatecommand');
    }   

    service.getSection = function (pageSectionId) {
        return $http.get(serviceBase + 'page-templates/0/sections/' + pageSectionId);
    }

    service.getModuleTypeSchema = function (pageModuleTypeId) {
        return $http.get(serviceBase + 'page-module-types/' + pageModuleTypeId);
    }

    /* COMMANDS */

    service.add = function (isCustomEntityRoute, command) {
        var entityName = isCustomEntityRoute ? 'customEntity' : 'page';
        command[entityName + 'VersionId'] = options.versionId;

        return $http.post(getServiceBase(isCustomEntityRoute), command);
    }

    service.update = function (isCustomEntityRoute, pageVersionModuleId, command) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionModuleId), command);
    }

    service.remove = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.delete(getIdRoute(isCustomEntityRoute, pageVersionModuleId));
    }

    service.moveUp = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionModuleId) + '/move-up');
    }

    service.moveDown = function (isCustomEntityRoute, pageVersionModuleId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionModuleId) + '/move-down');
    }

    /* HELPERS */

    function getIdRoute(isCustomEntityRoute, pageVersionModuleId) {
        return getServiceBase(isCustomEntityRoute) + '/' + pageVersionModuleId;
    }

    function getServiceBase(isCustomEntityRoute) {
        return isCustomEntityRoute ? customEntityModulesServiceBase : pageModulesServiceBase;
    }

    return service;
}]);