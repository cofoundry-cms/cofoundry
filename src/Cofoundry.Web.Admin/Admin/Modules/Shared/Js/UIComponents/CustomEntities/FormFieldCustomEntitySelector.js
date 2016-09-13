angular.module('cms.shared').directive('cmsFormFieldCustomEntitySelector', [
    '_',
    'shared.internalModulePath',
    'shared.customEntityService',
    'shared.directiveUtilities',
function (
    _,
    modulePath,
    customEntityService,
    directiveUtilities
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/CustomEntities/FormFieldCustomEntitySelector.html',
        scope: {
            model: '=cmsModel',
            title: '@cmsTitle',
            localeId: '=cmsLocaleId',
            customEntityDefinitionCode: '@cmsCustomEntityDefinitionCode'
        },
        require: ['?^^cmsFormDynamicFieldSet'],
        link: {
            pre: preLink
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    function preLink(scope, el, attrs, controllers) {
        var vm = scope.vm,
            dynamicFormFieldController = controllers[0];

        if (angular.isDefined(attrs.required)) {
            vm.isRequired = true;
        } else {
            vm.isRequired = false;
        }

        directiveUtilities.setModelName(vm, attrs);

        vm.search = function (query) {
            return customEntityService.getAll(vm.customEntityDefinitionCode, query);
        };

        vm.initialItemFunction = function (id) {
            return customEntityService.getByIdRange([id]).then(function (results) {
                return results[0];
            });
        };
    }

    /* CONTROLLER */

    function Controller() {
    }
}]);