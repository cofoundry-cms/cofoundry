angular.module('cms.shared').factory('shared.documentService', [
    '$http',
    '$upload',
    'shared.serviceBase',
function (
    $http,
    $upload,
    serviceBase) {

    var service = {},
        documentsServiceBase = serviceBase + 'documents';

    /* QUERIES */

    service.getAll = function (query) {

        return $http.get(documentsServiceBase, {
            params: query
        });
    }


    service.getById = function (documentId) {

        return $http.get(service.getIdRoute(documentId));
    }

    service.getByIdRange = function (ids) {

        return $http.get(documentsServiceBase + '/', {
            params: {
                'documentAssetIds': ids
            }
        });
    }

    service.getAllDocumentFileTypes = function () {

        return $http.get(serviceBase + 'document-file-types');
    }

    /* COMMANDS */

    service.add = function (command) {

        return service.uploadFile(service.getBaseRoute(), command, 'POST');
    }

    /* HELPERS */

    service.getIdRoute = function (documentId) {
        return documentsServiceBase + '/' + documentId;
    }

    service.getBaseRoute = function () {
        return documentsServiceBase;
    }

    service.uploadFile = function (path, command, method) {
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