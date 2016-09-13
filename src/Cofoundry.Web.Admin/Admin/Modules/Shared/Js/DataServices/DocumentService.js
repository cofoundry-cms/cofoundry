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

    service.getAllDocumentFileTypes = function () {

        return $http.get(serviceBase + 'document-file-types');
    }

    /* HELPERS */

    service.getIdRoute = function (documentId) {
        return documentsServiceBase + '/' + documentId;
    }

    service.getBaseRoute = function () {
        return documentsServiceBase;
    }

    return service;
}]);