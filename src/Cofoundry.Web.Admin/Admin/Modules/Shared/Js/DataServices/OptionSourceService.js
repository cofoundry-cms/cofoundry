angular.module('cms.shared').factory('shared.optionSourceService', ['$http', 'shared.serviceBase', function ($http, serviceBase) {
    var service = {};

    /* QUERIES */

    service.getFromApi = function (api) {
        return $http.get(api).then(loadData);

        function loadData(response) {
            var data = response,
                depth = 0,
                maxDepth = 2;

            while (data.data && depth <= maxDepth) {
                data = data.data;
                depth++;
            }

            return data;
        }
    }

    return service;
}]);