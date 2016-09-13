angular.module('cms.pages').factory('pages.customEntityService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {};

    /* QUERIES */

    service.getAllRoutingRules = function () {
        return $http.get(serviceBase + 'custom-entity-routing-rules/');
    }
    
    return service;
}]);