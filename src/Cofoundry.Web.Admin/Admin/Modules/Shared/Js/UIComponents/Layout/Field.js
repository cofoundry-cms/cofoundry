angular.module('cms.shared').directive('cmsField', [
    '$timeout',
    'shared.internalModulePath',
    function (
        $timeout,
        modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Layout/Field.html',
        transclude: true,
        replace: true,
        require: '^^cmsForm'
    }
}]);