/**
 * A warning status message, dislpayed in a yellow coloured box
 */
angular.module('cms.shared').directive('cmsWarningMessage', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/StatusMessages/WarningMessage.html',
        replace: true,
        transclude: true
    };
}]);
