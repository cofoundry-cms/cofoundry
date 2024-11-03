angular.module('cms.shared').factory('shared.youTubeService', [
    '$http',
    '$q',
    'shared.pluginServiceBase',
function (
    $http,
    $q,
    serviceBase
    ) {

    var service = {},
        serviceUrl = 'https://www.googleapis.com/youtube/v3/videos?id=',
        apiKey;

    /* INIT */

    $http.get(serviceBase + 'youtube-settings').then(loadSettings);

    function loadSettings(data) {
        apiKey = data.apiKey;
    }

    /* QUERIES */

    service.getVideoInfo = function (id) {

        if (apiKey) {

            return wrapGetResponse(serviceUrl + id + '&part=snippet&key=' + apiKey)
                .then(function (response) {
                    if (response && response.data && response.data.items.length) {
                        var data = response.data.items[0],
                            snippet = data.snippet,
                            thumbail = snippet.thumbnails.high;

                        var result = {
                            id: id,
                            title: snippet.title,
                            description: snippet.description,
                            publishDate: snippet.publishedAt
                        };

                        if (thumbail) {
                            result.thumbnailUrl = thumbail.url;
                            result.thumbnailWidth = thumbail.width;
                            result.thumbnailHeight = thumbail.height;
                        }

                        return result;
                    }

                    return;
                });
        } else {
            // No API key provided, so just return the id part of the data
            var def = $q.defer();
            def.resolve({ id: id });
            return def.promise;
        }
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