angular.module('cms.shared').factory('shared.youTubeService', [
    '$http',
    '$q',
function (
    $http,
    $q
    ) {

    var service = {},
        serviceUrl = 'https://www.googleapis.com/youtube/v3/videos?id=';

    /* QUERIES */

    service.getVideoInfo = function (id) {

        return wrapGetResponse(serviceUrl + id + '&part=contentDetails&key=')
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