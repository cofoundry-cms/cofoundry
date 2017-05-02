import { Injectable }    from '@angular/core';
import * as _ from 'lodash';

@Injectable()
export default class ValidationErrorService {
    private handlers = [];

    /**
    * Raises validation errors with any registered handlers
    */
    raise(errors) {
        let unhandledErrors = [];

        errors.forEach((error) => {
            let errorHandlers = _.filter(this.handlers, (handler) => {
                return _.find(error.properties, (prop: string) => {
                    if (!prop || !handler.prop) {
                        return false;
                    }
                    return handler.prop.toLowerCase() === prop.toLowerCase();
                });
            });

            if (errorHandlers.length) {
                errorHandlers.forEach((errorHandler) => {
                    errorHandler.fn([error]);
                });
            } else {
                unhandledErrors.push(error);
            }
        });

        if (unhandledErrors.length) {
            let genericHandlers = _.filter(this.handlers, (handler) => {
                return !handler.prop;
            });

            if (genericHandlers.length) {
                this.executeHandlers(genericHandlers, unhandledErrors);
            } else {
                this.unhandledValidationErrorHandler(errors);
            }
        }
    }

    /**
    * Registers a handler for a validation error with a specific property
    */
    addHandler(prop, fn) {
        let handler = {
            prop: prop,
            fn: fn
        };

        this.handlers.push(handler);
        return handler;
    }

    /**
    * Unregisters a handler for a validation error, you can either pass
    * the handler function to remove that specific instance or the property
    * name to remove all handlers for that property.
    */
    removeHandler(fnOrProp) {
        let items; 

        if (_.isFunction(fnOrProp)) {
            items = _.filter(this.handlers, { fn: fnOrProp });
        } else {
            items = _.filter(this.handlers, { prop: fnOrProp });
        }

        this.handlers = _.difference(this.handlers, items);
    }

    /* PRIVATE */

    unhandledValidationErrorHandler(errors) {
        // TODO: Display a friendly validation error popup
        throw new Error('An unhandled validation exception has occured');
    }

    executeHandlers(handlers, errors) {
        handlers.forEach((handler) => {
            handler.fn(errors);
        });
    }
}
