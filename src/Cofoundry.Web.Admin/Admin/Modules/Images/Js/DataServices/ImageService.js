angular.module('cms.images').factory('images.imageService', [
        '$http',
        'shared.imageService',
    function (
        $http,
        sharedImageService) {

    var service = _.extend({}, sharedImageService);

    /* COMMANDS */

    service.remove = function (id) {

        return $http.delete(service.getIdRoute(id));
    }

    return service;
}]);