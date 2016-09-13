angular.module('cms.directories').factory('directories.directoryService', [
    '$http',
    '_',
    'shared.serviceBase',
    'directories.DirectoryTree',
function (
    $http,
    _,
    serviceBase,
    DirectoryTree) {

    var service = {},
        directoryServiceBase = serviceBase + 'webdirectories';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(directoryServiceBase);
    }

    service.getTree = function () {
        return $http.get(directoryServiceBase + '/tree').then(function (tree) {
            return tree ? new DirectoryTree(tree) : tree;
        });
    }

    service.getById = function (webDirectoryId) {

        return $http.get(getIdRoute(webDirectoryId));
    }

    /* COMMANDS */

    service.add = function (command) {

        return $http.post(directoryServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.webDirectoryId), command);
    }

    service.remove = function (webDirectoryId) {

        return $http.delete(getIdRoute(webDirectoryId));
    }

    /* PRIVATES */

    function getIdRoute(webDirectoryId) {
        return directoryServiceBase + '/' + webDirectoryId;
    }

    return service;
}]);