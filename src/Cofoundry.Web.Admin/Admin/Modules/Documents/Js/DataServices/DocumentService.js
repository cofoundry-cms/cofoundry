angular.module('cms.documents').factory('documents.documentService', [
        '$http',
        'shared.documentService',
    function (
        $http,
        sharedDocumentService) {

    var service = _.extend({}, sharedDocumentService);
        
    /* COMMANDS */

    service.update = function (command) {
        return service.uploadFile(service.getIdRoute(command.documentAssetId), command, 'PUT');
    }

    service.remove = function (id) {
        
        return $http.delete(service.getIdRoute(id));
    }

    return service;
}]);