angular.module('cms.shared').factory('shared.pageService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        pagesServiceBase = serviceBase + 'pages';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(pagesServiceBase, {
            params: query
        });
    }

    service.getByIdRange = function (pageIds) {
        return $http.get(pagesServiceBase, {
            params: {
                'pageIds': pageIds
            }
        });
    }
    
    service.getById = function (pageId) {

        return $http.get(service.getIdRoute(pageId));
    }

    service.getVersionsByPageId = function (pageId) {

        return $http.get(service.getPageVerionsRoute(pageId));
    }

    service.getPageTypes = function () {
        return [{
            name: 'Generic',
            value: 'Generic'
        },
        {
            name: 'Custom Entity Details',
            value: 'CustomEntityDetails'
        }];
    }

    /* COMMANDS */

    service.add = function (command) {

        return $http.post(pagesServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(service.getIdRoute(command.pageId), command);
    }

    service.updateUrl = function (command) {

        return $http.put(service.getIdRoute(command.pageId) + '/url', command);
    }

    service.updateDraft = function (command) {

        return $http.patch(service.getPageVerionsRoute(command.pageId) + '/draft', command);
    }

    service.removeDraft = function (id) {

        return $http.delete(service.getPageVerionsRoute(id) + '/draft');
    }

    service.remove = function (id) {

        return $http.delete(service.getIdRoute(id));
    }

    service.duplicate = function (command) {
        return $http.post(service.getIdRoute(command.pageToDuplicateId) + '/duplicate', command);
    }

    /* PRIVATES */

    /* HELPERS */

    service.getIdRoute = function (pageId) {
        return pagesServiceBase + '/' + pageId;
    }

    service.getPageVerionsRoute = function (pageId) {
        return service.getIdRoute(pageId) + '/versions';
    }

    return service;
}]);