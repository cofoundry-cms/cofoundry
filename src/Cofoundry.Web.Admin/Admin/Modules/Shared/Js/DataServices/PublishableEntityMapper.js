angular.module('cms.shared').factory('shared.publishableEntityMapper', [
'$http',
'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {};

    /* MAPPERS */

    service.map = function (entity) {
        entity.isPublished = isPublished.bind(null, isPublished);
    }

    /* PRIVATE */

    function isPublished(entity) {

        return entity.publishStatus == 'Published'
            && entity.hasPublishedVersion
            && new Date(entity.publishDate) < Date.now();
    }

    return service;
}]);