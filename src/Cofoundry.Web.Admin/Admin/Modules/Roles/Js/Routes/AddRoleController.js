angular.module('cms.roles').controller('AddRoleController', [
    '$location',
    'shared.LoadState',
    'roles.permissionService',
    'roles.roleService',
    'roles.userAreaService',
function (
    $location,
    LoadState,
    permissionService,
    roleService,
    userAreaService) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        vm.globalLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        initForm();
        initData();

        vm.editMode = false;

        vm.save = save;
        vm.cancel = cancel;
    }

    /* EVENTS */

    function save() {
        vm.globalLoadState.on();

        roleService
            .add(vm.command)
            .then(redirectToList)
            .finally(vm.globalLoadState.off);
    }

    function cancel() {
        redirectToList();
    }

    function redirectToList() {
        $location.path('/');
    }

    function initForm() {
        userAreaService
            .getAll()
            .then(loadAreas);

        function loadAreas(results) {
            vm.userAreas = results;
            if (results.length == 1) {
                vm.command.userAreaCode = results[0].userAreaCode;
            }
            vm.formLoadState.off();
        }
    }

    function initData() {
        vm.command = {
            permissions: []
        };
    }
}]);