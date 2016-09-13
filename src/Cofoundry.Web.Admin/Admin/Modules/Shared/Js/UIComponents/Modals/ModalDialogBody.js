angular.module('cms.shared').directive('cmsModalDialogBody', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Modals/ModalDialogBody.html',
        transclude: true,
    };
}]);