angular.module('cms.roles').controller('RoleDetailsController', [
    '$routeParams',
    '$location',
    '$q',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'roles.roleService',
    'roles.permissionService',
    'roles.modulePath',
function (
    $routeParams,
    $location,
    $q,
    LoadState,
    modalDialogService,
    permissionValidationService,
    roleService,
    permissionService,
    modulePath
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save;
        vm.cancel = reset;
        vm.deleteRole = deleteRole;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.canUpdate = permissionValidationService.canUpdate('COFROL');
        vm.canDelete = permissionValidationService.canDelete('COFROL');

        // Init
        initData().then(setLoadingOff.bind(null, vm.formLoadState));
    }

    /* UI ACTIONS */

    function edit() {
        vm.editMode = true;
        vm.mainForm.formStatus.clear();
    }

    function save() {
        setLoadingOn(vm.saveLoadState);

        roleService.update(vm.command)
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.command = mapUpdateCommand(vm.role);
        vm.mainForm.formStatus.clear();
    }

    function deleteRole() {
        var options = {
            title: 'Delete Role',
            message: 'Are you sure you want to delete this role?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();

            return roleService
                .remove(vm.role.roleId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }

    /* PRIVATE FUNCS */

    function onSuccess(message) {
        return initData()
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData() {
        var roleId = $routeParams.id;

        return roleService.getById(roleId)
            .then(load);

        function load(role) {

            vm.role = role;
            vm.command = mapUpdateCommand(role);
            vm.editMode = false;
        }
    }

    function mapUpdateCommand(role) {

        var command = _.pick(role,
            'roleId',
            'title'
            );

        command.permissions = _.map(role.permissions, function (permission) {
            var data = {
                permissionCode: permission.permissionType.code
            };

            if (permission.entityDefinition) {
                data.entityDefinitionCode = permission.entityDefinition.entityDefinitionCode;
            }

            return data;
        });

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