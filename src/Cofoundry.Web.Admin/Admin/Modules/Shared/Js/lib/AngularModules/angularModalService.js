/** 
 * Modified from (http://www.dwmkerr.com/the-only-angularjs-modal-service-youll-ever-need/)
 * Service for showing modal dialogs.
 */
(function () {

    'use strict';

    var module = angular.module('angularModalService', []);

    module.factory('ModalService', [
        '$document',
        '$compile',
        '$controller',
        '$http',
        '$rootScope',
        '$q',
        '$timeout',
        '$templateCache',
        '_',
    function (
        $document,
        $compile,
        $controller,
        $http,
        $rootScope,
        $q,
        $timeout,
        $templateCache,
        _) {

        var body = $document.find('body');
        return new ModalService();

        /**
         * Publicly exposed service for managing modal popups
         */
        function ModalService() {

            var me = this,
                stack = new ModalPopupStack();

            me.showModal = function (options) {
                var modal;

                options.stack = stack;
                modal = new ModalPopup(options);

                return modal.init();
            }
        }

        /**
         * Maintains a stack of popup elements on a page and handles
         * the openening and closing of stacked modals so that only 
         * one is shown at a time
         */
        function ModalPopupStack() {
            var me = this,
                stack = [];

            me.count = 0;

            me.push = function (modal) {
                setTopElVisibility(false);
                stack.push(modal);
                setTopElVisibility(true);
                me.count = stack.length;
            }

            me.pop = function () {
                if (stack.length) {
                    setTopElVisibility(false);
                    stack.pop();
                    setTopElVisibility(true);
                    me.count = stack.length;
                }
            }

            function setTopElVisibility(isVisible) {
                if (stack.length) stack[stack.length - 1].setVisibility(isVisible);
            }
        }

        /**
         * Represents a single modal popup component.
         */
        function ModalPopup(options) {
            var me = this;

            /* Public */

            me.init = function () {
                var deferred = $q.defer();

                //  Validate the input parameters.
                if (!options.controller) {
                    deferred.reject("No controller has been specified.");
                    return deferred.promise;
                }

                getTemplate(options.template, options.templateUrl)
                    .then(loadModal)
                    .then(deferred.resolve)
                    .catch(function (error) {
                        deferred.reject(error);
                });;

                return deferred.promise;
            }

            /* Private Helpers*/

            function loadModal(template) {

                var MODAL_PAUSE_CLS = 'modal--pause',
                    MODAL_SHOW_CLS = 'modal--show',
                    modalScope = $rootScope.$new(),
                    closeDeferred = $q.defer(),
                    inputs = getInputs(modalScope),
                    parentEl = getModalParentElement();

                modalScope.isRootModal = options.stack.count === 0;

                var modalElement = createModalElement(template, modalScope);
                inputs.$element = modalElement;

                var modalController = $controller(options.controller, inputs);

                //  Finally, append the modal to the dom.
                parentEl.append(modalElement);

                //  Pass back an object that represents this modal
                var modal = {
                    controller: modalController,
                    scope: modalScope,
                    element: modalElement,
                    close: closeDeferred.promise,
                    setVisibility: setVisibility
                };

                modal.close.then(onClosed);

                // Add to the stack
                options.stack.push(modal);

                return modal;
                                
                /**
                 * Create the inputs object to the controller - this will include
                 * the scope, as well as all inputs provided.
                 * We will also create a deferred that is resolved with a provided
                 * close function. The controller can then call 'close(result)'.
                 * The controller can also provide a delay for closing - this is 
                 * helpful if there are closing animations which must finish first.
                 */
                function getInputs(scope) {

                    return _.extend({}, options.inputs, {
                        $scope: scope,
                        close: close
                    });
                }

                function createModalElement(template, scope) {
                    //  Parse the modal HTML into a DOM element (in template form).
                    var modalElementTemplate = angular.element(template);

                    //  Compile then link the template element, building the actual element.
                    //  Set the $element on the inputs so that it can be injected if required.
                    return $compile(modalElementTemplate)(scope);
                }

                function getModalParentElement() {
                    return options.appendElement || body;
                }

                function close(result, delay) {
                    if (delay === undefined || delay === null) delay = 500;
                    modal.element.children().removeClass(MODAL_SHOW_CLS);
                    $timeout(function () {
                        closeDeferred.resolve(result);
                    }, delay);
                }

                /**
                 * When close is resolved, we'll clean up the scope and element.
                 */
                function onClosed(result) {
                    // remove from the stack (assume this is the top modal)
                    options.stack.pop();
                    //  Clean up the scope
                    modalScope.$destroy();
                    //  Remove the element from the dom.
                    modalElement.remove();
                    // Tell the parent that we have closed the modal
                    if (parent && options.stack.count === 0) {
                        parent.postMessage({
                            type: 'MODAL_CLOSE'

                        }, document.location.origin);
                    }
                }

                /**
                 * Temporarily shows or hides the modal.
                 */
                function setVisibility(isVisible) {
                    var fnName = 'addClass';

                    if (isVisible) {
                        fnName = 'removeClass'
                    }
                    modal.element[fnName](MODAL_PAUSE_CLS);
                }

            }

            /** 
            * Returns a promise which gets the template, either
            * from the template parameter or via a request to the 
            * template url parameter.
            */  
            function getTemplate(template, templateUrl) {
                var deferred = $q.defer();

                if (template) {
                    deferred.resolve(template);
                } else if (templateUrl) {
                    // check to see if the template has already been loaded
                    var cachedTemplate = $templateCache.get(templateUrl);

                    if (cachedTemplate !== undefined) {
                        deferred.resolve(cachedTemplate);
                    }
                    else {
                        // if not, let's grab the template for the first time
                        $http({ method: 'GET', url: templateUrl, cache: true })
                        .then(function (result) {
                            // save template into the cache and return the template
                            $templateCache.put(templateUrl, result.data);
                            deferred.resolve(result.data);
                        }).catch(deferred.reject);
                    }
                } else {
                    deferred.reject("No template or templateUrl has been specified.");
                }
                return deferred.promise;
            };

        }

    }]);

}());