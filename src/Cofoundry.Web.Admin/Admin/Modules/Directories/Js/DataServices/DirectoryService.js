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
        directoryServiceBase = serviceBase + 'page-directories';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(directoryServiceBase);
    }

    service.getTree = function () {
        return $http.get(directoryServiceBase + '/tree').then(function (tree) {
            return tree ? new DirectoryTree(tree) : tree;
        });
    }

    service.getById = function (pageDirectoryId) {

        return $http.get(getIdRoute(pageDirectoryId));
    }

    /* COMMANDS */

    service.add = function (command) {

        return $http.post(directoryServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.pageDirectoryId), command);
    }

    service.remove = function (pageDirectoryId) {

        return $http.delete(getIdRoute(pageDirectoryId));
    }

    /* PRIVATES */

    function getIdRoute(pageDirectoryId) {
        return directoryServiceBase + '/' + pageDirectoryId;
    }

    return service;
}]);