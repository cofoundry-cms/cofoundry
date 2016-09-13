angular.module('cms.pages').directive('cmsFormFieldDirectorySelector', [
    '_',
    'shared.directiveUtilities',
    'pages.modulePath',
    'pages.directoryService',
function (
    _,
    directiveUtilities,
    modulePath,
    directoryService
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'uicomponents/directories/FormFieldDirectorySelector.html',
        scope: {
            model: '=cmsModel',
            onLoaded: '&cmsOnLoaded'
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

        directoryService.getAll().then(function (webDirectories) {
            vm.webDirectories = webDirectories;
            if (vm.onLoaded) vm.onLoaded();
        });
    }
}]);