angular.module('cms.shared').factory('shared.vimeoService', [
    '$http',
    '$q',
function (
    $http,
    $q
    ) {

    var service = {},
        serviceUrl = 'https://vimeo.com/api/oembed.json?url=https%3A%2F%2Fvimeo.com%2F';

    /* QUERIES */

    service.getVideoInfo = function (id) {

        return wrapGetResponse(serviceUrl + id)
            .then(function (response) {
                if (response && response.data) {
                    return response.data;
                }

                return;
            });
    }

    function wrapGetResponse() {
        var def = $q.defer();

        $http.get.apply(this, arguments)
            .then(def.resolve)
            .catch(function (response) {
                if (response.status == 404) {
                    def.resolve();
                }
                def.reject(response);
            });

        return def.promise;
    }

    return service;
}]);