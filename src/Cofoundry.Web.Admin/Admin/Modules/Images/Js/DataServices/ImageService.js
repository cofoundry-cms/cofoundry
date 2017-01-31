angular.module('cms.images').factory('images.imageService', [
        '$http',
        '$upload',
        'shared.imageService',
    function (
        $http,
        $upload,
        sharedImageService) {

    var service = _.extend({}, sharedImageService);

    /* COMMANDS */

    service.remove = function (id) {

        return $http.delete(service.getIdRoute(id));
    }

    return service;
}]);