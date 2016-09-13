angular.module('cms.account').factory('account.accountService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        accountServiceBase = serviceBase + 'account';

    /* QUERIES */
        
    service.getAccountDetails = function () {

        return $http.get(accountServiceBase);
    }

    /* COMMANDS */

    service.update = function (command) {

        return $http.patch(accountServiceBase, command);
    }

    service.updatePassword = function (command) {

        return $http.put(accountServiceBase + "/password", command);
    }

    return service;
}]);