angular.module('cms.directories').directive('cmsDirectoryGrid', [
    'shared.permissionValidationService',
    'directories.modulePath',
function (
    permissionValidationService,
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DirectoryGrid.html',
        scope: {
            pageDirectories: '=cmsDirectories',
            redirect: '=cmsRedirect'
        },
        replace: false,
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        vm.canUpdate = permissionValidationService.canUpdate('COFDIR');
    }

}]);