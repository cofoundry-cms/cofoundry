angular.module('cms.documents').factory('documents.documentService', [
        '$http',
        '$upload',
        'shared.documentService',
    function (
        $http,
        $upload,
        sharedDocumentService) {

    var service = _.extend({}, sharedDocumentService);
        
    /* COMMANDS */

    service.add = function (command) {

        return uploadFile(service.getBaseRoute(), command, 'POST');
    }

    service.update = function (command) {
        return uploadFile(service.getIdRoute(command.documentAssetId), command, 'PATCH');
    }

    service.remove = function (id) {

        return $http.delete(service.getIdRoute(id));
    }

    /* PRIVATES */

    function uploadFile(path, command, method) {
        var data = _.omit(command, 'file');

        return $upload.upload({
            url: path,
            data: data,
            file: command.file,
            method: method
        });
    }

    return service;
}]);