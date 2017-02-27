angular.module('cms.shared').directive('cmsFormSectionActions', ['shared.internalModulePath', '$timeout', function (modulePath, $timeout) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormSectionActions.html',
        scope: {
        },
        replace: true,
        transclude: true,
        link: link
    };

    function link(scope, elem, attrs) {
    }
}]);