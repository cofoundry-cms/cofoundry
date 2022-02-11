angular.module('cms.shared').directive('cmsUserLink', [
    'shared.internalModulePath',
    'shared.urlLibrary',
    'shared.permissionValidationService',
function (
    modulePath,
    urlLibrary,
    permissionValidationService
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
        vm.canRead = permissionValidationService.canRead('COFUSR');
        vm.formatName = formatName;
    }

    function formatName(user) {
        if (user.firstName || user.lastName) {
            return user.firstName + ' ' + user.lastName;
        }

        return user.username;
    }
}]);