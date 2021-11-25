angular.module('cms.directories').directive('cmsDirectoryPath', [
    'directories.modulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DirectoryPath.html',
        scope: {
            pageDirectory: '=cmsDirectory'
        },
        replace: false,
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* CONTROLLER */

    function Controller() {
    }

}]);