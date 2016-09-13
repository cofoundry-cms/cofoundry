angular.module('cms.shared').directive('cmsProgressBar', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { loadState: '=' },
        templateUrl: modulePath + 'UIComponents/Loader/ProgressBar.html'
    };
}]);