angular.module('cms.shared').factory('shared.vimeoService', [
    '$http',
    '$q',
    'shared.errorService',
function (
    $http,
    $q,
    errorService
    ) {

    var service = {},
        serviceUrl = 'https://vimeo.com/api/oembed.json?url=https%3A%2F%2Fvimeo.com%2F';

    /* QUERIES */

    service.getVideoInfo = function (id) {

        return wrapGetResponse(serviceUrl + id)
            .then(function (response) {
                return JSON.parse(response.responseText);
            });
    }

    function wrapGetResponse(url) {
        var def = $q.defer();

        var xhr = new XMLHttpRequest();
        xhr.addEventListener("load", onComplete);
        xhr.open("GET", url);
        xhr.send();

        function onComplete() {
            var response = this;
            var isUnexpectedError = false;
            var errorMsg = "";

            switch (response.status) {
                case 200:
                    break;
                case 404:
                    errorMsg = "You aren't able to access the video because of privacy or permissions issues, or because the video is still transcoding.";
                    break;
                case 403:
                    errorMsg = "Embed permissions are disabled for this video, so you can't embed it.";
                    break;
                default:
                    isUnexpectedError = true;
                    errorMsg = "Something unexpected happened whilst connecting to the Vimeo API.";
            }

            if (!errorMsg.length) {
                def.resolve(response);
            } else {
                var error = {
                    title: 'Vimeo API Error',
                    message: errorMsg,
                    response: response
                }

                if (isUnexpectedError) {
                    errorService.raise(error);
                }

                def.reject(error);
            }
        }

        return def.promise;
    }

    return service;
}]);