/**
* Configured CSRF tokens for $http requests
*/
angular.module('cms.shared').config(['$httpProvider', 'csrfToken', function ($httpProvider, csrfToken) {
    var headers = $httpProvider.defaults.headers,
        csrfHeader = 'X-XSRF-Token',
        csrfableVerbs = [
            'put',
            'post',
            'patch',
            'delete'
        ];
    
    $httpProvider.interceptors.push('httpInterceptor');

    csrfableVerbs.forEach(function (verb) {
        headers[verb] = headers[verb] || {};
        headers[verb][csrfHeader] = csrfToken;
    });

}]);