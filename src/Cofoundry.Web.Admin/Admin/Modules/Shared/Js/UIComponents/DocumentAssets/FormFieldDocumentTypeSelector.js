angular.module('cms.shared').directive('cmsFormFieldDocumentTypeSelector', [
    '_',
    'shared.internalModulePath',
    'shared.documentService',
function (
    _,
    modulePath,
    documentService) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DocumentAssets/FormFieldDocumentTypeSelector.html',
        scope: {
            model: '=cmsModel',
            disabled: '=cmsDisabled',
            readonly: '=cmsReadonly',
            onLoaded: '&cmsOnLoaded'
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        documentService.getAllDocumentFileTypes().then(function (fileTypes) {

            vm.fileTypes = fileTypes;

            if (vm.onLoaded) vm.onLoaded();
        });
    }
}]);