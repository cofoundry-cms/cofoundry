angular.module('cms.shared').directive('cmsModalDialogHeader', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogHeader.html',
        transclude: true,
    };
}]);