angular.module('cms.shared').directive('cmsModalDialogActions', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogActions.html',
        transclude: true
    };
}]);