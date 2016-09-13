angular.module('cms.shared').directive('cmsFormSection', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormSection.html',
        scope: {
            title: '@cmsTitle'
        },
        replace: true,
        transclude: true
    };
}]);