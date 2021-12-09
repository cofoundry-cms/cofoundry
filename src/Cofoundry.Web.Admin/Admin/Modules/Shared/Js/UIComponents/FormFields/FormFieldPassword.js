angular.module('cms.shared').directive('cmsFormFieldPassword', [
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    modulePath,
    baseFormFieldFactory) {

    var config = {
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldPassword.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            passwordPolicy: '=cmsPasswordPolicy'
        }),
        passThroughAttributes: [
            'required',
            'minlength',
            'maxlength',
            'pattern',
            'passwordrules',
            'disabled',
            'cmsMatch'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    
    function link(scope, element, attrs, controllers) {
        var vm = scope.vm;

        // call base
        baseFormFieldFactory.defaultConfig.link.apply(this, arguments);
        
        init();

        /* Init */

        function init() {
            scope.$watch('vm.passwordPolicy', onPasswordPolicyChange);
        }

        function onPasswordPolicyChange(policy) {
                
            vm.policyAttributes = {};

            processAttribute('minlength', true);
            processAttribute('maxlength', true);
            processAttribute('placeholder');
            processAttribute('pattern', true);
            processAttribute('passwordrules');
            processAttribute('title', false, policy ? policy.description : null);

            function processAttribute(attribute, addValidator, defaultValue) {
                var value;

                if (attrs[attribute]) {
                    value = attrs[attribute]
                } 
                else if (policy && policy.attributes[attribute]) {
                    value = policy.attributes[attribute];
                }

                setAttributeValue(attribute, value, addValidator, defaultValue);
            }

            function setAttributeValue(attribute, value, addValidator, defaultValue) {

                vm.policyAttributes[attribute] = value || defaultValue || '';
                if (!addValidator) return;

                var validator = _.findWhere(vm.validators, { name: attribute });

                if (validator && (!value || !policy)) {
                    vm.validators = _.without(vm.validators, validator);
                }
                else if (policy && value && validator) {
                    validator.message = policy.description;
                }
                else if (policy && value && !validator)
                {
                    vm.validators.push({
                        name: attribute,
                        message: policy.description
                    });
                }
            }
        }
    }
}]);