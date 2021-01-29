angular.module('cms.shared').directive('cmsPageBody', [
    'shared.internalModulePath',
function (
    modulePath
) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Layout/PageBody.html',
        scope: {
            contentType: '@cmsContentType',
            hasActions: '=cmsHasActions'
        },
        replace: true,
        transclude: true,
        controllerAs: 'vm',
        bindToController: true,
        controller: ['$scope', Controller]
    };

    /* CONTROLLER */

    function Controller(scope) {
    };
}]);