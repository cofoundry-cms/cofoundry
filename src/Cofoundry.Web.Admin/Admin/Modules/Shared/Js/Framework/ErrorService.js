angular.module('cms.shared').factory('errorService', function () {
    var service = {};

    /* PUBLIC */

    service.raise = function (errors) {
        //alert('An unexpected error occured');
        //TODO: Log
    }

    return service;
});