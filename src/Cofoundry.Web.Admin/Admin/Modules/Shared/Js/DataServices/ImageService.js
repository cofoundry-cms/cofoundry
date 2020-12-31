angular.module('cms.shared').factory('shared.imageService', [
    '$http',
    'Upload',
    'shared.stringUtilities',
    'shared.serviceBase',
function (
    $http,
    Upload,
    stringUtilities,
    serviceBase) {

    var service = {},
        imagesServiceBase = serviceBase + 'images';

    /* QUERIES */

    service.add = function (command) {
        return uploadFile(service.getBaseRoute(), command, 'POST');
    }

    service.update = function (command) {

        return uploadFile(service.getIdRoute(command.imageAssetId), command, 'PUT');
    }

    service.getAll = function (query) {

        return $http.get(imagesServiceBase, {
            params: query
        });
    }

    service.getById = function (imageId) {

        return $http.get(service.getIdRoute(imageId));
    }

    service.getSettings = function () {

        return $http.get(imagesServiceBase + "/settings");
    }

    service.getByIdRange = function (ids) {

        return $http.get(imagesServiceBase + '/', {
            params: {
                'imageAssetIds': ids
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

    /* PRIVATES */

    function uploadFile(path, command, method) {
        var data = _.omit(command, 'file'),
            file;

        // the isCurrentFile flag tells us this is a mock version of a file
        // used as placeholder to enable a preview. We shouldn't try and upload it.
        if (command.file && !command.file.isCurrentFile) {
            data.file = command.file;
        }

        return Upload.upload({
            url: path,
            data: data,
            method: method
        });
    }

    return service;
}]);