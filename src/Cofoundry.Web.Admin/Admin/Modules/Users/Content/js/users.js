angular
    .module('cms.users', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('users.modulePath', '/Admin/Modules/Users/Js/');
angular.module('cms.users').config([
    '$routeProvider',
    'shared.routingUtilities',
    'users.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'User');
}]);
angular.module('cms.users').factory('users.userService', [
    '$http',
    'shared.serviceBase',
    'users.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        userServiceBase = serviceBase + 'users';

    /* QUERIES */

    service.getAll = function (query) {
        query = addUserArea(query);

        return $http.get(userServiceBase, {
            params: query
        });
    }

    service.getById = function (userId) {

        return $http.get(getIdRoute(userId));
    }

    /* COMMANDS */

    service.add = function (command) {
        command = addUserArea(command);
        command.generatePassword = true;

        return $http.post(userServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.userId), command);
    }

    service.resetPassword = function (userId) {

        return $http.put(getIdRoute(userId) + '/reset-password');
    }

    service.updateVerificationStatus = function (userId, isVerified) {

        return $http.put(getIdRoute(userId) + '/verification-status', {
            userId: userId,
            isVerified: isVerified
        });
    }

    service.remove = function (id) {

        return $http.delete(getIdRoute(id));
    }

    /* PRIVATES */

    function getIdRoute(userId) {
        return userServiceBase + '/' + userId;
    }

    function addUserArea(o) {
        o = o || {};
        o.userAreaCode = options.userAreaCode;

        return o;
    }

    return service;
}]);
angular.module('cms.users').controller('AddUserController', [
    '$location',
    '_',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.roleService',
    'users.userService',
    'users.options',
function (
    $location,
    _,
    stringUtilities,
    LoadState,
    roleService,
    userService,
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
        vm.isCofoundryAdmin = options.userAreaCode === 'COF';

        vm.save = save;
        vm.cancel = cancel;
    }

    /* EVENTS */

    function save() {
        vm.globalLoadState.on();
        
        userService.add(vm.command)
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
            .getSelectionList(options.userAreaCode)
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
angular.module('cms.users').controller('UserDetailsController', [
    '$routeParams',
    '$location',
    '$q',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'shared.roleService',
    'shared.currentUser',
    'users.userService',
    'users.options',
function (
    $routeParams,
    $location,
    $q,
    LoadState,
    modalDialogService,
    permissionValidationService,
    roleService,
    currentUser,
    userService,
    options
    ) {

    var vm = this,
        isCurrentUser = currentUser.userId == $routeParams.id;

    init();
    
    /* INIT */

    function init() {

        // UI actions
        vm.edit = edit;
        vm.save = save;
        vm.cancel = reset;
        vm.resetPassword = resetPassword;
        vm.deleteUser = deleteUser;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);
        vm.userArea = options;

        vm.isCofoundryAdmin = options.userAreaCode === 'COF';
        vm.isCurrentUser = isCurrentUser;
        
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

    function resetPassword() {
        var options = {
            title: 'Reset Password',
            message: 'Resetting a password will sign the user out of all sessions and email them a new temporary password that needs to be changed at first sign in.<br><br>Do you want to continue?',
            okButtonTitle: 'Yes, reset it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();

            return userService
                .resetPassword(vm.user.userId)
                .then(onSuccess.bind(null, 'Password reset, notification sent.'))
                .finally(setLoadingOff);
        }
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
            .getSelectionList(options.userAreaCode)
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

            setPermissions(user);
        }
    }

    function setPermissions(user){ 

        var isEditEnabled = true,
            entityDefinitionCode = options.userAreaCode === 'COF' ? 'COFUSR' : 'COFUSN';

        if (!user) return;

        if (user.accountStatus === 'Deleted') {
            isEditEnabled = false;
            vm.mainForm.formStatus.error('This user cannot be edited because it has been deleted.')
        } else if (user.role.isSuperAdminRole && !currentUser.isSuperAdmin) {
            isEditEnabled = false;
            vm.mainForm.formStatus.error('You need to be in the super admin role to update this user.')
        }

        vm.canUpdate = isEditEnabled && permissionValidationService.canUpdate(entityDefinitionCode);
        vm.canDelete = isEditEnabled && !isCurrentUser && permissionValidationService.canDelete(entityDefinitionCode);
        vm.canResetPassword = isEditEnabled 
            && options.allowPasswordSignIn 
            && options.useEmailAsUsername
            && !isCurrentUser
            && permissionValidationService.hasPermission(entityDefinitionCode + 'RSTPWD');
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
            'email',
            'requirePasswordChange',
            'isEmailConfirmed',
            'displayName'
            );

        if (vm.user.accountVerifiedDate) {
            command.isAccountVerified = true;
        }

        if (vm.user.accountStatus === 'Active') {
            command.isActive = true;
        }
        
        if (vm.user.role) {
            command.roleId = vm.user.role.roleId;
        }
        
        command.username = vm.userArea.useEmailAsUsername ? null : vm.user.username;

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
angular.module('cms.users').controller('UserListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'shared.permissionValidationService',
    'users.userService',
    'users.options',
function (
    _,
    LoadState,
    SearchQuery,
    urlLibrary,
    permissionValidationService,
    userService,
    options) {

    var vm = this;

    init();

    function init() {
        
        vm.userArea = options;
        vm.urlLibrary = urlLibrary;
        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged,
            defaultParams: {
                accountStatus: 'Active'
            }
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;
        vm.filterOptions = {
            accountStatus: [{ 
                name: 'Any', 
                value: 'Any'
            },{ 
                name: 'Active', 
                value: 'Active' 
            }, {
                name: 'Deactivated',
                value: 'Deactivated'
            }]
        }

        var entityDefinitionCode = options.userAreaCode === 'COF' ? 'COFUSR' : 'COFUSN';
        vm.canRead = permissionValidationService.canRead(entityDefinitionCode);
        vm.canUpdate = permissionValidationService.canUpdate(entityDefinitionCode);
        vm.canCreate = permissionValidationService.canCreate(entityDefinitionCode);

        toggleFilter(false);

        loadGrid();
    }

    /* ACTIONS */

    function toggleFilter(show) {
        vm.isFilterVisible = _.isUndefined(show) ? !vm.isFilterVisible : show;
    }

    /* EVENTS */

    function onQueryChanged() {
        toggleFilter(false);
        loadGrid();
    }

    /* PRIVATE FUNCS */
    
    function loadGrid() {
        vm.gridLoadState.on();

        return userService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);