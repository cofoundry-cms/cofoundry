angular.module('cms.shared').directive('cmsFormSectionAuditData', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormSectionAuditData.html',
        scope: {
            auditData: '=cmsAuditData'
        }
    };
}]);