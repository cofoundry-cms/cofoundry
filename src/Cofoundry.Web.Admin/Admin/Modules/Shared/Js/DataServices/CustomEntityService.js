angular.module('cms.shared').factory('shared.customEntityService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase,
    options) {

    var service = {},
        customEntityDefinitionServiceBase = serviceBase + 'custom-entity-definitions/',
        customEntityServiceBase = serviceBase + 'custom-entities';

    /* QUERIES */

    service.getAll = function (customEntityDefinitionCode, query) {
        return $http.get(customEntityDefinitionServiceBase + customEntityDefinitionCode + '/custom-entities', {
            params: query
        });
    }

    service.getDefinition = function (customEntityDefinitionCode) {
        return $http.get(customEntityDefinitionServiceBase + customEntityDefinitionCode);
    }

    service.getDefinitionsByIdRange = function (customEntityDefinitionCodes) {
        return $http.get(customEntityDefinitionServiceBase).then(filterByIdRange);

        function filterByIdRange(results) {
            return _.filter(results, function (result) {
                return _.contains(customEntityDefinitionCodes, result.customEntityDefinitionCode);
            });
        }
    }

    service.getByIdRange = function (ids) {

        return $http.get(customEntityServiceBase + '/', {
            params: {
                'ids': ids
            }
        });
    }

    return service;
}]);