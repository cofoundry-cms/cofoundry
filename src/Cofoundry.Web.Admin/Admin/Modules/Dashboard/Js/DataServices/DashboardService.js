angular.module('cms.dashboard').factory('dashboard.dashboardService', ['$http', '_', 'shared.serviceBase', function ($http, _, serviceBase) {
    var service = {},
        pagingParams = {
            pageSize: 5
        };

    /* QUERIES */

    service.getPages = function () {
        return $http.get(serviceBase + 'pages', getParams());
    }

    service.getDraftPages = function () {
        return $http.get(serviceBase + 'pages', getParams({
            publishStatus: 'Draft'
        }));
    }

    service.getUsers = function () {
        return $http.get(serviceBase + 'users', getParams({
            userAreaCode: 'COF'
        }));
    }

    service.getPageTemplates = function () {
        return $http.get(serviceBase + 'page-templates', getParams());
    }

    function getParams(additionalParams) {
        var newParams = pagingParams;

        if (additionalParams) {
            newParams = _.extend({}, pagingParams, additionalParams)
        };

        return {
            params: newParams
        };
    }

    return service;
}]);