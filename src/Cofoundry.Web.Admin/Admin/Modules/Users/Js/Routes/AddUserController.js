angular.module('cms.users').controller('AddUserController', [
    '$location',
    '_',
    'shared.stringUtilities',
    'shared.LoadState',
    'users.userService',
    'users.roleService',
    'users.options',
function (
    $location,
    _,
    stringUtilities,
    LoadState,
    userService,
    roleService,
    options) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        initForm();
        initData();

        vm.globalLoadState = new LoadState();
        vm.editMode = false;
        vm.userArea = options;

        vm.save = save;
        vm.cancel = cancel;
    }

    /* EVENTS */

    function save() {
        vm.globalLoadState.on();

        userService
            .add(vm.command)
            .then(redirectToList)
            .finally(vm.globalLoadState.off);
    }

    /* PRIVATE FUNCS */
    
    function cancel() {
        redirectToList();
    }

    function redirectToList() {
        $location.path('/');
    }

    function initForm() {

        return roleService
            .getSelectionList()
            .then(load);

        function load(result) {

            if (result) {
                vm.roles = result.items;
                if (result.items.length === 1) {
                    vm.command.roleId = result.items[0].roleId;
                }
            }
        }

    }

    function initData() {
        vm.command = {};
    }
}]);