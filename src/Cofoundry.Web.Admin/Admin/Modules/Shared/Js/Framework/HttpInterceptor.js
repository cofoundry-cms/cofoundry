angular.module('cms.shared').factory('httpInterceptor', [
    '$q',
    '$rootScope',
    '_',
    'shared.validationErrorService',
    'authenticationService',
function (
    $q,
    $rootScope,
    _,
    validationErrorService,
    authenticationService) {

    var service = {};

    /* PUBLIC */

    service.response = function (response) {
        if (!_.isUndefined(response.data.data)) {
            response = response.data.data;
        }

        return response;
    };

    service.responseError = function (response) {
        switch (response.status) {
            case 400:
                /* Bad Request */
                if (response.data.isValid === false) {
                    validationErrorService.raise(response.data.errors);
                }
                break;
            case 401:
                /* Unauthorized */
                authenticationService.redirectToLogin();
                break;
            case 403:
                /* Forbidden (authenticated but not permitted to view resource */
                var msg = 'This action is not authorized';
               
                /* Can't use the modal because it would create a circular reference */
                alert(msg);
                break;
            default:
                /* Allow get/404 responses through */
                if (response.status != 404 || response.config.method !== 'GET') {
                    throw new Error('Unexpected response: ' + response.status + ' (' + response.statusText + ')');
                }
                break;
        }

        return $q.reject(response);
    };

    return service;

}]);