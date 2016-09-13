angular.module('cms.customEntities').factory('customEntities.customEntityService', [
    '$http',
    'shared.serviceBase',
    'customEntities.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        customEntityDefinitionServiceBase = serviceBase + 'custom-entity-definitions/' + options.customEntityDefinitionCode,
        customEntityServiceBase = serviceBase + 'custom-entities';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(customEntityDefinitionServiceBase + '/custom-entities', {
            params: query
        });
    }

    service.getById = function (customEntityId) {

        return $http.get(getIdRoute(customEntityId));
    }

    service.getDataModelSchema = function () {
        return $http.get(customEntityDefinitionServiceBase + '/data-model-schema');
    }

    service.getVersionsByCustomEntityId = function (customEntityId) {

        return $http.get(getVerionsRoute(customEntityId));
    }

    service.getByIdRange = function (ids) {

        return $http.get(customEntityServiceBase + '/', {
            params: {
                'ids': ids
            }
        });
    }

    /* COMMANDS */

    service.add = function (command) {
        command.customEntityDefinitionCode = options.customEntityDefinitionCode;
        return $http.post(customEntityServiceBase, command);
    }

    service.updateUrl = function (command) {

        return $http.put(getIdRoute(command.customEntityId) + '/url', command);
    }

    service.updateOrdering = function (command) {

        return $http.put(customEntityServiceBase + '/ordering', command);
    }
    
    service.updateDraft = function (command) {
        command.customEntityDefinitionCode = options.customEntityDefinitionCode;

        return $http.put(getVerionsRoute(command.customEntityId) + '/draft', command);
    }

    service.remove = function (customEntityId) {

        return $http.delete(getIdRoute(customEntityId));
    }

    service.removeDraft = function (id) {

        return $http.delete(getVerionsRoute(id) + '/draft');
    }


    /* PRIVATES */

    function getIdRoute (customEntityId) {
        return customEntityServiceBase + '/' + customEntityId;
    }

    function getVerionsRoute (customEntityId) {
        return getIdRoute(customEntityId) + '/versions';
    }

    return service;
}]);