angular.module('cms.shared').factory('shared.validationErrorService', [
    '_',
function (
    _
    ) {

    var service = {},
        handlers = [];

    /* PUBLIC */

    /**
    * Raises validation errors with any registered handlers
    */
    service.raise = function (errors) {
        var unhandledErrors = [];

        errors.forEach(function (error) {
            var errorHandlers = _.filter(handlers, function (handler) {
                return _.find(error.properties, function (prop) {
                    if (!prop || !handler.prop) return false;
                    return handler.prop.toLowerCase() === prop.toLowerCase();
                });
            });

            if (errorHandlers.length) {
                errorHandlers.forEach(function (errorHandler) {
                    errorHandler.fn([error]);
                });
            } else {
                unhandledErrors.push(error);
            }
        });

        if (unhandledErrors.length) {
            var genericHandlers = _.filter(handlers, function (handler) {
                return !handler.prop;
            });

            if (genericHandlers.length) {
                executeHandlers(genericHandlers, unhandledErrors);
            } else {
                unhandledValidationErrorHandler(errors);
            }
        }
    }

    /**
    * Registers a handler for a validation error with a specific property
    */
    service.addHandler = function (prop, fn) {
        var handler = {
            prop: prop,
            fn: fn
        };

        handlers.push(handler);
        return handler;
    }

    /**
    * Unregisters a handler for a validation error, you can either pass
    * the handler function to remove that specific instance or the property
    * name to remove all handlers for that property.
    */
    service.removeHandler = function (fnOrProp) {
        var items; 

        if (_.isFunction(fnOrProp)) {
            items = _.where(handlers, { fn: fnOrProp });
        } else {
            items = _.where(handlers, { prop: fnOrProp });
        }

        handlers = _.difference(handlers, items);
    }

    /* PRIVATE */

    function unhandledValidationErrorHandler(errors) {
        // TODO: Display a friendly validation error popup
        throw new Error('An unhandled validation exception has occured');
    }

    function executeHandlers(handlers, errors) {
        handlers.forEach(function (handler) {
            handler.fn(errors);
        });
    }

    return service;
}]);