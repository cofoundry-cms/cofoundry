angular.module('cms.shared').factory('shared.nestedDataModelSchemaService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase
    ) {

    var service = {};

    /* QUERIES */
    
    service.getByName = function (name) {
        return $http.get(serviceBase + 'nested-data-model-schemas/' + name);
    }
    
    return service;
}]);