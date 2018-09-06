angular.module('cms.shared').factory('shared.pageService', [
    '_',
    '$http',
    'shared.serviceBase',
    'shared.publishableEntityMapper',
function (
    _,
    $http,
    serviceBase,
    publishableEntityMapper) {

    var service = {},
        pagesServiceBase = serviceBase + 'pages';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(pagesServiceBase, {
            params: query
        }).then(map);

        function map(pagedResult) {
            _.map(pagedResult.items, publishableEntityMapper.map);

            return pagedResult;
        }
    }

    service.getByIdRange = function (pageIds) {
        return $http.get(pagesServiceBase, {
            params: {
                'pageIds': pageIds
            }
        }).then(map);

        function map(pageSummaries) {
            _.map(pageSummaries, publishableEntityMapper.map);

            return pageSummaries;
        }
    }
    
    service.getById = function (pageId) {

        return $http
            .get(service.getIdRoute(pageId))
            .then(map);

        function map(page) {

            if (page) {
                publishableEntityMapper.map(page.pageRoute);
            }

            return page;
        }
    }

    service.getVersionsByPageId = function (pageId, query) {

        return $http.get(service.getPageVerionsRoute(pageId), {
            params: query
        });
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

    /* HELPERS */

    service.getIdRoute = function (pageId) {
        return pagesServiceBase + '/' + pageId;
    }

    service.getPageVerionsRoute = function (pageId) {
        return service.getIdRoute(pageId) + '/versions';
    }

    function mapPageSummaries(page) {

        return _.map(publishableEntityMapper.map);
    }

    return service;
}]);