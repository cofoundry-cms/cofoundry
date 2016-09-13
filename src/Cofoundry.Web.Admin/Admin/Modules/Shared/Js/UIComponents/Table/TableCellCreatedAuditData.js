angular.module('cms.shared').directive('cmsTableCellCreatedAuditData', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { auditData: '=cmsAuditData' },
        templateUrl: modulePath + 'UIComponents/Table/TableCellCreatedAuditData.html'
    };
}]);