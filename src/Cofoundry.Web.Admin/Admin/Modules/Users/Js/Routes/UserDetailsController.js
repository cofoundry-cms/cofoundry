angular.module('cms.users').controller('UserDetailsController', [
    '$routeParams',
    '$location',
    '$q',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'users.userService',
    'users.roleService',
    'users.modulePath',
    'users.options',
function (
    $routeParams,
    $location,
    $q,
    LoadState,
    modalDialogService,
    permissionValidationService,
    userService,
    roleService,
    modulePath,
    options
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save;
        vm.cancel = reset;
        vm.deleteUser = deleteUser;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);
        vm.userArea = options;

        var entityDefinitionCode = options.userAreaCode === 'COF' ? 'COFUSR' : 'COFUSN';
        vm.canUpdate = permissionValidationService.canUpdate(entityDefinitionCode);
        vm.canDelete = permissionValidationService.canDelete(entityDefinitionCode);

        // Init
        $q.all([loadRoles(), loadUser()])
            .then(initForm)
            .then(setLoadingOff.bind(null, vm.formLoadState));
    }

    /* UI ACTIONS */

    function edit() {
        vm.editMode = true;
        vm.mainForm.formStatus.clear();
    }

    function save() {
        setLoadingOn(vm.saveLoadState);

        userService.update(vm.command)
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.command = mapUpdateCommand();
        vm.mainForm.formStatus.clear();
    }

    function deleteUser() {
        var options = {
            title: 'Delete User',
            message: 'Are you sure you want to delete this user?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();

            return userService
                .remove(vm.user.userId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }

    /* PRIVATE FUNCS */
    function onSuccess(message) {
        return loadUser()
            .then(initForm)
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function loadRoles() {

        return roleService
            .getSelectionList()
            .then(load);

        function load(result) {

            if (result) {
                vm.roles = result.items;
            }
        }

    }

    function loadUser() {
        var userId = $routeParams.id;

        return userService
            .getById(userId)
            .then(load);

        function load(user) {

            vm.user = user;
        }
    }

    function initForm() {
        vm.command = mapUpdateCommand();
        vm.editMode = false;
    }

    function mapUpdateCommand() {

        var command = _.pick(vm.user,
            'userId',
            'firstName',
            'lastName',
            'username',
            'email'
            );

        if (vm.user.role) {
            command.roleId = vm.user.role.roleId;
        }

        return command;
    }

    function redirectToList() {
        $location.path('');
    }

    function setLoadingOn(loadState) {
        vm.globalLoadState.on();
        if (loadState && _.isFunction(loadState.on)) loadState.on();
    }

    function setLoadingOff(loadState) {
        vm.globalLoadState.off();
        if (loadState && _.isFunction(loadState.off)) loadState.off();
    }
}]);