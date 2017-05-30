angular.module('cms.shared').directive('cmsUserLink', [
    'shared.internalModulePath',
    'shared.urlLibrary',
function (
    modulePath,
    urlLibrary
    ) {

    return {
        restrict: 'E',
        scope: { user: '=cmsUser' },
        templateUrl: modulePath + 'UIComponents/User/UserLink.html',
        controller: controller,
        controllerAs: 'vm',
        bindToController: true
    };

    function controller() {
        var vm = this;

        vm.urlLibrary = urlLibrary;
    }
}]);