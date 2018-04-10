angular.module('cms.shared').factory('shared.customEntityService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase
    ) {

    var service = {},
        customEntityServiceBase = serviceBase + 'custom-entities',
        schemaServiceBase = serviceBase + 'custom-entity-data-model-schemas';

    /* QUERIES */

    service.getAll = function (query, customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) + '/custom-entities', {
            params: query
        });
    }

    service.getDefinition = function (customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode));
    }

    service.getDataModelSchema = function (customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) + '/data-model-schema');
    }

    service.getDataModelSchemasByCodeRange = function (codes) {
        return $http.get(schemaServiceBase, {
            params: {
                customEntityDefinitionCodes: codes
            }
        });
    }

    service.getPageRoutes = function (customEntityDefinitionCode) {
        return $http.get(getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) + '/routes');
    }

    service.getDefinitionsByIdRange = function (customEntityDefinitionCodes) {
        return $http.get(getCustomEntityDefinitionServiceBase()).then(filterByIdRange);

        function filterByIdRange(results) {
            return _.filter(results, function (result) {
                return _.contains(customEntityDefinitionCodes, result.customEntityDefinitionCode);
            });
        }
    }

    service.getByIdRange = function (ids) {

        return $http.get(customEntityServiceBase + '/', {
            params: {
                'customEntityIds': ids
            }
        });
    }

    service.getById = function (customEntityId) {

        return $http.get(getIdRoute(customEntityId));
    }

    service.getVersionsByCustomEntityId = function (customEntityId) {

        return $http.get(getVerionsRoute(customEntityId));
    }


    /* COMMANDS */

    service.add = function (command, customEntityDefinitionCode) {
        command.customEntityDefinitionCode = customEntityDefinitionCode;
        return $http.post(customEntityServiceBase, command);
    }

    service.updateUrl = function (command) {

        return $http.put(getIdRoute(command.customEntityId) + '/url', command);
    }

    service.updateOrdering = function (command) {

        return $http.put(customEntityServiceBase + '/ordering', command);
    }

    service.updateDraft = function (command, customEntityDefinitionCode) {
        command.customEntityDefinitionCode = customEntityDefinitionCode;

        return $http.put(getVerionsRoute(command.customEntityId) + '/draft', command);
    }

    service.remove = function (customEntityId) {

        return $http.delete(getIdRoute(customEntityId));
    }

    service.removeDraft = function (id) {

        return $http.delete(getVerionsRoute(id) + '/draft');
    }


    /* PRIVATES */

    function getIdRoute(customEntityId) {
        return customEntityServiceBase + '/' + customEntityId;
    }

    function getVerionsRoute(customEntityId) {
        return getIdRoute(customEntityId) + '/versions';
    }

    function getCustomEntityDefinitionServiceBase(customEntityDefinitionCode) {
        var customEntityDefinitionServiceBase = serviceBase + 'custom-entity-definitions/';
        if (!customEntityDefinitionCode) {
            return customEntityDefinitionServiceBase;
        }
        return customEntityDefinitionServiceBase + customEntityDefinitionCode;
    }


    return service;
}]);