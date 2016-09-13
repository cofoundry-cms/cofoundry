/**
 * A success status message, dislpayed in a green coloured box
 */
angular.module('cms.shared').directive('cmsSuccessMessage', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/StatusMessages/SuccessMessage.html',
        replace: true,
        transclude: true
    };
}]);
