angular.module('cms.shared').directive('cmsTableCellImage', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { image: '=cmsImage' },
        templateUrl: modulePath + 'UIComponents/Table/TableCellImage.html'
    };
}]);