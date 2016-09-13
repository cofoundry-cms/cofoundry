angular.module('cms.shared').factory('shared.vimeoService', [
    '$http',
    '$q',
function (
    $http,
    $q
    ) {

    var service = {},
        serviceUrl = '//vimeo.com/api/v2/video/';

    /* QUERIES */

    service.getVideoInfo = function (id) {

        return wrapGetResponse(serviceUrl + id + '.json')
            .then(function (response) {

                if (response && response.data) {
                    return response.data[0];
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