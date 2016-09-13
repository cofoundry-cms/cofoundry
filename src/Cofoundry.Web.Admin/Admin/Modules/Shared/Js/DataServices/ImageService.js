angular.module('cms.shared').factory('shared.imageService', [
    '$http',
    '$upload',
    'shared.stringUtilities',
    'shared.serviceBase',
function (
    $http,
    $upload,
    stringUtilities,
    serviceBase) {

    var service = {},
        imagesServiceBase = serviceBase + 'images';

    /* QUERIES */

    service.getAll = function (query) {

        return $http.get(imagesServiceBase, {
            params: query
        });
    }

    service.getById = function (imageId) {

        return $http.get(service.getIdRoute(imageId));
    }

    service.getByIdRange = function (ids) {

        return $http.get(imagesServiceBase + '/', {
            params: {
                'ids': ids
            }
        });
    }

    /* HELPERS */

    service.getIdRoute = function (imageId) {
        return imagesServiceBase + '/' + imageId;
    }

    service.getBaseRoute = function () {
        return imagesServiceBase;
    }

    return service;
}]);