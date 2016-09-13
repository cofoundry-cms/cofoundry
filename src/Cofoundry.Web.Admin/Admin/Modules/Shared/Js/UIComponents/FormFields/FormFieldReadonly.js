angular.module('cms.shared').directive('cmsFormFieldReadonly', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/FormFields/FormFieldReadonly.html',
        replace: true,
        require: '^^cmsForm',
        scope: {
            title: '@cmsTitle',
            description: '@cmsDescription',
            model: '=cmsModel'
        },
        controller: function () { },
        controllerAs: 'vm',
        bindToController: true
    };
}]);