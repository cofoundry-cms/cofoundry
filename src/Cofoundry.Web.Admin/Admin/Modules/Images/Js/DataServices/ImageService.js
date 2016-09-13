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

    service.add = function (command) {
        return uploadFile(service.getBaseRoute(), command, 'POST');
    }

    service.update = function (command) {

        return uploadFile(service.getIdRoute(command.imageAssetId), command, 'PATCH');
    }

    service.remove = function (id) {

        return $http.delete(service.getIdRoute(id));
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