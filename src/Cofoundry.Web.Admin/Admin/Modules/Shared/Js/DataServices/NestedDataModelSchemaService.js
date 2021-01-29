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
    };

    service.getByNames = function (names) {
        return $http.get(serviceBase + 'nested-data-model-schemas/', {
            params: {
                names: names
            }
        });
    };

    service.validate = function (typeName, model) {
        return $http.post(serviceBase + 'nested-data-model-schemas/validate', {
            typeName: typeName,
            model: model
        });
    };
    
    return service;
}]);