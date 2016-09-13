angular.module('cms.shared').directive('cmsButtonLink', [
    'shared.internalModulePath',
    function (modulePath) {

    return {
        restrict: 'E',
        replace: true,
        templateUrl: modulePath + 'UIComponents/Buttons/buttonlink.html',
        scope: {
            text: '@cmsText',
            href: '@cmsHref'
        }
    };
}]);