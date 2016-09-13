angular.module('cms.shared').factory('shared.entityVersionService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        pageServiceBase = serviceBase + 'pages',
        customEntityServiceBase = serviceBase + 'custom-entities';

    /* QUERIES */


    /* COMMANDS */

    service.publish = function (isCustomEntity, entityId) {

        return $http.patch(getVersionsRoute(isCustomEntity, entityId) + '/draft/publish');
    }

    service.unpublish = function (isCustomEntity, entityId) {

        return $http.patch(getVersionsRoute(isCustomEntity, entityId) + '/published/unpublish');
    }

    service.duplicateDraft = function (isCustomEntity, entityId, entityVersionId) {
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

        return $http.post(getVersionsRoute(isCustomEntity, entityId), command);
    }

    service.removeDraft = function (isCustomEntity, entityId) {

        return $http.delete(getVersionsRoute(isCustomEntity, entityId) + '/draft');
    }
    
    /* HELPERS */

    function getVersionsRoute (isCustomEntity, entityId) {
        return getIdRoute(isCustomEntity, entityId) + '/versions';
    }

    function getIdRoute(isCustomEntity, entityId) {
        return getServiceBase(isCustomEntity) + '/' + entityId;
    }

    function getServiceBase(isCustomEntity) {
        return isCustomEntity ? customEntityServiceBase : pageServiceBase;
    }

    return service;
}]);