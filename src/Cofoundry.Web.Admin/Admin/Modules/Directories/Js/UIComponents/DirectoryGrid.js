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
            startDepth: '=cmsStartDepth',
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

        vm.getPathDepthIndicator = getPathDepthIndicator;
        vm.canUpdate = permissionValidationService.canUpdate('COFDIR');

        /* View Helpers */

        function getPathDepthIndicator(depth) {
            var depthIndicator = '',
                startDepth = (vm.startDepth || 0) + 1;

            for (var i = startDepth; i < depth; i++) {
                depthIndicator += '— ';
            }

            return depthIndicator;
        }
    }

}]);