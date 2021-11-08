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

    service.getAccessRulesByPageDirectoryId = function (pageDirectoryId) {

        return $http.get(getAccessRulesRoute(pageDirectoryId));
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

    service.updateAccessRules = function (command) {

        return $http.patch(getAccessRulesRoute(command.pageDirectoryId), command);
    }

    /* PRIVATES */

    function getIdRoute(pageDirectoryId) {
        return directoryServiceBase + '/' + pageDirectoryId;
    }

    function getAccessRulesRoute (pageDirectoryId) {
        return getIdRoute(pageDirectoryId) + '/access-rules';
    }

    return service;
}]);