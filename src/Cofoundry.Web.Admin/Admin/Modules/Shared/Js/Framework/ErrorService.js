/**
 * Using modal dialogs in some places can cause circular reference issues, so
 * we proxy error messages through this shared service. This also allows multiple
 * handlers to subscribe to the errors, although in practice this will typically
 * just be the default handler provided in ErrorHandlerInitializer.
 */
angular.module('cms.shared').factory('shared.errorService', function () {
    var service = {},
        handlers = [];

    /* PUBLIC */

    // { title: 'server', 'message': 'Help!', response }
    service.raise = function (error) {
        handlers.forEach(function (handler) {
            handler(error);
        });
    }

    /**
    * Registers a handler for an error
    */
    service.addHandler = function (fn) {

        handlers.push(fn);
        return fn;
    }

    /**
    * Unregisters a handler for an error. 
    */
    service.removeHandler = function (fn) {

        developerExceptionHandlers = _.difference(developerExceptionHandlers, fn);
    }

    return service;
});