angular.module('cms.shared').directive('cmsCustomEntityLink', [
    'shared.internalModulePath',
    'shared.urlLibrary',
function (
    modulePath,
    urlLibrary
    ) {

    return {
        restrict: 'E',
        scope: {
            customEntityDefinition: '=cmsCustomEntityDefinition',
            customEntity: '=cmsCustomEntity'
        },
        templateUrl: modulePath + 'UIComponents/CustomEntities/CustomEntityLink.html',
        controller: controller,
        controllerAs: 'vm',
        bindToController: true
    };

    function controller() {
        var vm = this;

        vm.urlLibrary = urlLibrary;
    }
}]);