angular.module('cms.shared').directive('cmsFormFieldDirectorySelector', [
    '_',
    'shared.directiveUtilities',
    'shared.internalModulePath',
    'shared.directoryService',
function (
    _,
    directiveUtilities,
    modulePath,
    directoryService
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Directories/FormFieldDirectorySelector.html',
        scope: {
            model: '=cmsModel',
            title: '@cmsTitle',
            onLoaded: '&cmsOnLoaded',
            readonly: '=cmsReadonly'
        },
        link: {
            pre: preLink
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    function preLink(scope, el, attrs) {
        var vm = scope.vm;

        if (angular.isDefined(attrs.required)) {
            vm.isRequired = true;
        } else {
            vm.isRequired = false;
            vm.defaultItemText = attrs.cmsDefaultItemText || 'None';
        }
        vm.title = attrs.cmsTitle || 'Directory';
        vm.description = attrs.cmsDescription;
        directiveUtilities.setModelName(vm, attrs);
    }

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        directoryService.getAll().then(function (pageDirectories) {
            vm.pageDirectories = pageDirectories;

            if (vm.onLoaded) vm.onLoaded();
        });
    }
}]);