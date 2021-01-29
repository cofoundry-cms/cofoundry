/**
 * Base class for form fields that uses default conventions and includes integration with 
 * server validation.
 */
angular.module('cms.shared').factory('baseFormFieldFactory', [
    '$timeout',
    'shared.stringUtilities',
    'shared.directiveUtilities',
    'shared.validationErrorService',
function (
    $timeout,
    stringUtilities,
    directiveUtilities,
    validationErrorService
    ) {

    var service = {},
        /* Here we can validation messages that can apply to all FormField controls */
        globalDefaultValidationMessages = [
            {
                attr: 'required',
                msg: 'This field is required'
            },
            {
                attr: 'maxlength',
                msg: 'This field cannot be longer than {0} characters'
            },
            {
                attr: 'minlength',
                msg: 'This must be at least {0} characters long'
            }
        ];

    /* PUBLIC */

    service.create = function (config) {
        return angular.extend({}, service.defaultConfig, config);
    }

    /* CONFIG */

    /**
     * Configuration defaults
     */
    service.defaultConfig = {
        restrict: 'E',
        replace: true,
        require: ['^^cmsForm'],
        scope: {
            title: '@cmsTitle',
            description: '@cmsDescription',
            change: '&cmsChange',
            model: '=cmsModel',
            disabled: '=cmsDisabled',
            readonly: '=cmsReadonly'
        },
        compile: compile,
        link: link,
        controller: function () { },
        controllerAs: 'vm',
        bindToController: true,

        /* Custom Properties */

        /** 
         * Should return the main input element that is displayed in edit mode.
         * By default this returns the first child input element.
         */
        getInputEl: getInputEl,

        /**
         * a list of attributes that when defined on the directive are passed through to the element
         * returned from getInputEl.
         */
        passThroughAttributes: [],

        /**
         * Default validation messages to use when none are provided on the element. Saves specifying common messages
         * like 'This field is required' on every field. Each array element should be an object in the form of:
         * { attr: 'name', msg: 'string message or formatter function' }
         */
        defaultValidationMessages: []
    };

    /* COMPILE */

    function compile(el, attrs) {

        initPassThroughAttributes.call(this, el, attrs);

        return this.link.bind(this);
    }

    function link(scope, el, attrs, controllers) {
        var vm = scope.vm,
            formController = controllers[0];

        // Model Properties
        vm.formScope = formController.getFormScope();
        vm.form = vm.formScope.getForm();
        
        directiveUtilities.setModelName(vm, attrs);
        parseErrorMessages(vm, attrs);

        // Model Funcs
        vm.onChange = onChange.bind(vm);
        vm.resetCustomErrors = resetCustomErrors.bind(vm);
        vm.addOrUpdateValidator = addOrUpdateValidator.bind(vm);

        // Init Errors
        vm.resetCustomErrors();

        // Bind Validation Events
        bindValidationEvents(scope, el);

        // watches
        scope.$watch('vm.model', function () {
            vm.resetCustomErrors();
        });
    }

    /* PUBLIC */

    function onChange() {
        var vm = this;

        vm.resetCustomErrors();
        if (vm.change) {
            // run after digest cycle completes so the parent ngModel is updated
            $timeout(vm.change, 0);
        }
    }

    function resetCustomErrors() {
        var model = this.form[this.modelName];

        if (model) {
            model.$setValidity('server', true);
        }
        this.customErrors = [];
    }

    function addOrUpdateValidator(validator) {
        var vm = this;
        var validators = _.filter(vm.validators, function (v) {
            return v.name !== validator.name;
        });
        validators.push(validator);
        vm.validators = validators;
    }

    /* HELPERS */

    /**
     * Loop through attributes specified in config.passThroughAttributes and copy
     * them onto the input control requrned by config.getInputEl
     */
    function initPassThroughAttributes(rootEl, attrs) {
        var config = this,
            el = config.getInputEl(rootEl);

        (config.passThroughAttributes || []).forEach(function (passThroughAttribute) {
            var name = passThroughAttribute.name || passThroughAttribute;
            if (angular.isDefined(attrs[name])) {
                el[0].setAttribute(attrs.$attr[name], attrs[name]);
            }
            else if (passThroughAttribute.default) {
                el[0].setAttribute(name, passThroughAttribute.default);
            }
        });
    }

    function getInputEl(rootEl) {
        return rootEl.find('input');
    }

    function bindValidationEvents(scope, el) {
        var fn = _.partial(addErrors, scope.vm, el);

        validationErrorService.addHandler(scope.vm.modelName, fn);

        scope.$on('$destroy', function () {
            validationErrorService.removeHandler(fn);
        });
    }
    
    function parseErrorMessages(vm, attrs) {
        var config = this,
            postfix = 'ValMsg',
            attrPostfix = '-val-msg';

        vm.validators = [];

        _.each(attrs.$attr, function (value, key) {
            var attributeToValidate,
                msg;

            if (stringUtilities.endsWith(key, postfix)) {
                // if the property is postfix '-val-msg' then pull in the message from the attribute
                attributeToValidate = value.substring(0, value.length - attrPostfix.length);
                msg = attrs[key];
            } else {

                attributeToValidate = value;

                // check to see if we have a default message for the property
                msg = getDefaultErrorMessage(config.defaultValidationMessages, key) || getDefaultErrorMessage(globalDefaultValidationMessages, key);
            }

            if (msg) {
                vm.validators.push({
                    name: attrs.$normalize(attributeToValidate),
                    message: stringUtilities.format(msg, attrs[key])
                });
            }
        });

        function getDefaultErrorMessage(defaultsToCheck, attr) {
            var validator = _.find(defaultsToCheck, function (v) {
                return v.attr === attr;
            });

            if (validator) {
                if (_.isFunction(validator.msg)) {
                    return validator.msg(vm.modelName, attrs);
                }
                return validator.msg;
            }
        }
    }

    function addErrors(vm, el, errors) {
        var form = vm.formScope.getForm();
        var model = form[vm.modelName];
        vm.resetCustomErrors();

        model.$setValidity('server', false);

        // make dirty to ensure css classes are applied
        getInputEl(el).removeClass('ng-pristine').addClass('ng-dirty');

        errors.forEach(function (error) {
            vm.customErrors.push(error);
        });
    }

    /* DEFINITION */

    return service;
}]);