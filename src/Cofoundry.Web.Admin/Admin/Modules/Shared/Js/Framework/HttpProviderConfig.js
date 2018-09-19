/**
* Configured CSRF tokens for $http requests
*/
angular.module('cms.shared').config([
    '$httpProvider',
    'csrfToken',
    'csrfHeaderName',
function (
    $httpProvider,
    csrfToken,
    csrfHeaderName
) {

    var headers = $httpProvider.defaults.headers,
        csrfableVerbs = [
            'put',
            'post',
            'patch',
            'delete'
        ];
    
    $httpProvider.interceptors.push('httpInterceptor');

    csrfableVerbs.forEach(function (verb) {
        headers[verb] = headers[verb] || {};
        headers[verb][csrfHeaderName] = csrfToken;
    });

    headers.common['X-Requested-With'] = 'XMLHttpRequest';

}]);