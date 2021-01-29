angular.module('cms.shared').directive('cmsFormFieldPageSelector', [
    '_',
    'shared.internalModulePath',
    'shared.pageService',
    'shared.directiveUtilities',
    'shared.modalDialogService',
function (
    _,
    modulePath,
    pageService,
    directiveUtilities,
    modalDialogService
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Pages/FormFieldPageSelector.html',
        scope: {
            model: '=cmsModel',
            title: '@cmsTitle',
            localeId: '=cmsLocaleId',
            readonly: '=cmsReadonly'
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
            return pageService.getAll(query);
        };
        
        vm.initialItemFunction = function (id) {
            return pageService.getByIdRange([id]).then(function (results) {
                return results[0];
            });
        };
    }

    /* CONTROLLER */

    function Controller() {
    }
}]);