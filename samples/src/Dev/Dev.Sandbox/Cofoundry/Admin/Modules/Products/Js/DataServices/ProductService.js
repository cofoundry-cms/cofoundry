angular.module('cms.products').factory('products.productService', [
    '$http',
    '$q',
    'shared.serviceBase',
function (
    $http,
    $q,
    serviceBase) {

    var service = {};
     
    /* COMMANDS */

    service.getProductDetails = function () {
        var deferred = $q.defer();

        deferred.resolve({
            title: 'Test Product',
            ref: 'TEST01'
        });

        return deferred.promise;
    }

    return service;
}]);