angular.module('cms.shared').factory('shared.modalDialogService', [
    '$q',
    '_',
    'ModalService',
    'shared.internalModulePath',
    'shared.LoadState',
function (
    $q,
    _,
    ModalService,
    modulePath,
    LoadState) {

    var service = {};

    /* PUBLIC */

    /**
    * Displays a modal message with a button to dismiss the message.
    */
    service.alert = function (optionsOrMessage) {
        var deferred = $q.defer(),
            options = optionsOrMessage || {};

        if (_.isString(optionsOrMessage)) {
            options = {
                message: optionsOrMessage
            }
        }

        ModalService.showModal({
            templateUrl: modulePath + "UIComponents/Modals/Alert.html",
            controller: "AlertController",
            inputs: {
                options: options
            }
        }).then(function (modal) {

            // Apres-creation stuff
            modal.close.then(deferred.resolve);
        });

        return deferred.promise;
    }

    /**
    * Displays a custom modal popup using a template at the specified url.
    */
    service.show = function (modalOptions) {
        return ModalService.showModal({
            templateUrl: modalOptions.templateUrl,
            controller: modalOptions.controller,
            inputs: {
                options: modalOptions.options
            }
        });
    }

    /**
    * Displays a modal message with a button options to ok/cancel an action.
    */
    service.confirm = function (optionsOrMessage) {
        var returnDeferred = $q.defer(),
            onOkLoadState = new LoadState(),
            options = initOptions(optionsOrMessage);

        ModalService.showModal({
            templateUrl: modulePath + "UIComponents/Modals/ConfirmDialog.html",
            controller: "ConfirmDialogController",
            inputs: {
                options: options
            }
        });

        return returnDeferred.promise;

        /* helpers */

        function initOptions(optionsOrMessage) {
            var options = optionsOrMessage || {},
                defaults = {
                    okButtonTitle: 'OK',
                    cancelButtonTitle: 'Cancel',
                    autoClose: true,
                    // onCancel: fn or promise
                    // onOk: fn or promise
                },
                internalScope = {
                    ok: resolve.bind(null, true),
                    cancel: resolve.bind(null, false),
                    onOkLoadState: onOkLoadState
                };

            if (_.isString(optionsOrMessage)) {
                options = {
                    message: optionsOrMessage
                }
            }

            return _.defaults(internalScope, options, defaults);
        }

        function resolve(isSuccess) {
            var optionToExec = isSuccess ? options.onOk : options.onOk.onCancel,
                deferredAction = isSuccess ? returnDeferred.resolve : returnDeferred.reject,
                optionResult;

            // run the action
            if (_.isFunction(optionToExec)) {
                onOkLoadState.on();
                optionResult = optionToExec();
            }

            // Wait for the result to resolve if its a promise
            // Then resolve/reject promise we returned to the callee
            return $q.when(optionResult)
                     .then(deferredAction);
        }
    }

    return service;
}]);