angular.module('cms.shared').directive('cmsTableCellUpdatedAuditData', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { auditData: '=cmsAuditData' },
        templateUrl: modulePath + 'UIComponents/Table/TableCellUpdatedAuditData.html'
    };
}]);