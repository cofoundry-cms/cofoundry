angular.module('cms.dashboard').factory('dashboard.dashboardService', [
    '$http', 
    '_',
    'shared.serviceBase'
, function (
    $http,
    _,
    serviceBase
    ) {

    var service = {};

    /* QUERIES */
    
    service.getContent = function () {
        return $http.get(serviceBase + 'dashboard');
    }

    return service;
}]);