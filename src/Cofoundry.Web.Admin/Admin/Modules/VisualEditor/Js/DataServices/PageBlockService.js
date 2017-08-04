/**
 * Service for managing page block, which can either be attached to a page or a custom entity.
 * Pass in the isCustomEntityRoute to switch between either route endpoint.
 */
angular.module('cms.visualEditor').factory('visualEditor.pageBlockService', [
    '$http',
    'shared.serviceBase',
    'visualEditor.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        pageBlocksServiceBase = serviceBase + 'page-version-region-blocks',
        customEntityBlocksServiceBase = serviceBase + 'custom-entity-version-page-blocks';

    /* QUERIES */

    service.getAllBlockTypes = function () {
        return $http.get(serviceBase + 'page-block-types/');
    }

    service.getPageVersionBlockById = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.get(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '?datatype=updatecommand');
    }   

    service.getRegion = function (pageRegionId) {
        return $http.get(serviceBase + 'page-templates/0/regions/' + pageRegionId);
    }

    service.getBlockTypeSchema = function (pageBlockTypeId) {
        return $http.get(serviceBase + 'page-block-types/' + pageBlockTypeId);
    }

    /* COMMANDS */

    service.add = function (isCustomEntityRoute, command) {
        var entityName = isCustomEntityRoute ? 'customEntity' : 'page';
        command[entityName + 'VersionId'] = options.versionId;

        return $http.post(getServiceBase(isCustomEntityRoute), command);
    }

    service.update = function (isCustomEntityRoute, pageVersionBlockId, command) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId), command);
    }

    service.remove = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.delete(getIdRoute(isCustomEntityRoute, pageVersionBlockId));
    }

    service.moveUp = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '/move-up');
    }

    service.moveDown = function (isCustomEntityRoute, pageVersionBlockId) {
        return $http.put(getIdRoute(isCustomEntityRoute, pageVersionBlockId) + '/move-down');
    }

    /* HELPERS */

    function getIdRoute(isCustomEntityRoute, pageVersionBlockId) {
        return getServiceBase(isCustomEntityRoute) + '/' + pageVersionBlockId;
    }

    function getServiceBase(isCustomEntityRoute) {
        return isCustomEntityRoute ? customEntityBlocksServiceBase : pageBlocksServiceBase;
    }

    return service;
}]);