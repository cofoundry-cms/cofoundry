angular.module('cms.shared').directive('cmsButtonIcon', [
    'shared.internalModulePath',
    function (modulePath) {

    return {
        restrict: 'E',
        replace: false,
        templateUrl: modulePath + 'UIComponents/Buttons/ButtonIcon.html',
        scope: {
            title: '@cmsTitle',
            icon: '@cmsIcon',
            href: '@cmsHref',
            external: '@cmsExternal'
        },
        link: function (scope, el) {
            if (scope.icon) {
                scope.iconCls = 'fa-' + scope.icon;
            }
        }
    };
}]);