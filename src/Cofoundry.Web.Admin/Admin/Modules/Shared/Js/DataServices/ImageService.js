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

    /* PRIVATES */

    function uploadFile(path, command, method) {
        var data = _.omit(command, 'file'),
            file;

        // the isCurrentFile flag tells us this is a mock version of a file
        // used as placeholder to enable a preview. We shouldn't try and upload it.
        if (command.file && !command.file.isCurrentFile) {
            file = command.file;
        }

        return $upload.upload({
            url: path,
            data: data,
            file: file,
            method: method
        });
    }

    return service;
}]);