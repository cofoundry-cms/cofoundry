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
angular.module('cms.users').factory('users.roleService', [
    '$http',
    'shared.serviceBase',
    'users.options',
function (
    $http,
    serviceBase,
    options) {

    var service = {},
        roleServiceBase = serviceBase + 'roles';

    /* QUERIES */

    service.getSelectionList = function () {
        return $http.get(roleServiceBase, {
            params: {
                userAreaCode: options.userAreaCode,
                excludeAnonymous: true
            }
        });
    }

    /* PRIVATES */

    return service;
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
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

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