angular.module('cms.shared').factory('shared.publishableEntityMapper', [
'$http',
'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {};

    /* MAPPERS */

    service.map = function (entity) {
        entity.isPublished = isPublished.bind(null, entity);
        entity.getPublishStatusLabel = getPublishStatusLabel.bind(null, entity);
    }

    /* PRIVATE */

    function isPublished(entity) {

        return entity.publishStatus == 'Published'
            && entity.hasPublishedVersion
            && new Date(entity.publishDate) < Date.now();
    }

    function getPublishStatusLabel(entity) {
        if (entity.publishStatus == 'Published' && entity.publishDate < Date.now()) {
            return 'Pending Publish';
        }

        return entity.publishStatus;
    }

    return service;
}]);