angular
    .module('cms.roles', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('roles.modulePath', '/Admin/Modules/Roles/Js/');
angular.module('cms.roles').config([
    '$routeProvider',
    'shared.routingUtilities',
    'roles.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Role');
}]);
angular.module('cms.roles').factory('roles.permissionService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        permissionServiceBase = serviceBase + 'permissions';

    /* QUERIES */

    service.getAll = function (query) {
        return $http.get(permissionServiceBase);
    }

    return service;
}]);
angular.module('cms.roles').factory('roles.roleService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {},
        COFOUNDRY_USER_AREA_CODE = 'COF',
        roleServiceBase = serviceBase + 'roles';

    /* QUERIES */

    service.getAll = function (query) {

        return $http.get(roleServiceBase, {
            params: query
        });
    }

    service.getById = function (roleId) {

        return $http.get(getIdRoute(roleId));
    }

    /* COMMANDS */

    service.add = function (command) {
        return $http.post(roleServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.roleId), command);
    }

    service.remove = function (roleId) {

        return $http.delete(getIdRoute(roleId));
    }

    /* PRIVATES */

    function getIdRoute(roleId) {
        return roleServiceBase + '/' + roleId;
    }


    /* PRIVATES */

    return service;
}]);
angular.module('cms.roles').factory('roles.userAreaService', [
    '$http',
    '_',
    'shared.serviceBase',
function (
    $http,
    _,
    serviceBase) {

    var service = {};

    /* QUERIES */

    service.getAll = function () {
        return $http.get(serviceBase + 'user-areas');
    }

    return service;
}]);
angular.module('cms.shared').directive('cmsFormFieldPermissionsCollection', [
    '_',
    'shared.LoadState',
    'roles.modulePath',
    'roles.permissionService',
function (
    _,
    LoadState,
    modulePath,
    permissionService
    ) {
    
    var PERMISSIONCODE_READ = 'COMRED';

    return {
        restrict: 'E',
        scope: {
            model: '=cmsModel',
            globalLoadState: '=cmsGlobalLoadState'
        },
        templateUrl: modulePath + 'UIComponents/FormFieldPermissionsCollection.html',
        require: ['^^cmsForm'],
        link: link
    };

    /* LINK */

    function link(scope, el, attrs, controllers) {
        var vm = scope,
            formController = controllers[0];

        scope.$watch('model', function (newValue, oldValue) {
            initPermissions();
        });

        loadPermissionsGrid();

        // Model Properties
        vm.formScope = formController.getFormScope();
        vm.permissionsLoadState = new LoadState(true);

        // UI Events
        vm.toggleGroup = toggleGroup;
        vm.permissionChanged = permissionChanged;

        /* Event Handlers */

        function toggleGroup($event, group) {
            var checkbox = $event.target;

            _.each(group.permissions, function (permission) {
                permission.selected = !!checkbox.checked;
                permissionChanged(permission, group, true);
            });

            setModel();
        }

        function permissionChanged(permission, group, supressRecalculation) {
            if (permission.permissionType.code === PERMISSIONCODE_READ) {
                group.isReadPermitted = permission.selected;

                // prevent 
                if (!permission.selected) {
                    _.each(group.permissions, function (permission) {
                        permission.selected = false;
                    });
                }
            }

            if (!supressRecalculation) {
                setModel();
            }
        }

        /* helpers */

        /**
         * Recalculates the data model from the selected permissions
         */
        function setModel() {
            var permissions = [];

            _.each(vm.permissions, function (permission) {
                var data;

                if (permission.selected) {
                    data = {
                        permissionCode: permission.permissionType.code
                    };

                    if (permission.entityDefinition) {
                        data.entityDefinitionCode = permission.entityDefinition.entityDefinitionCode;
                    }

                    permissions.push(data);
                }
            });

            vm.model = permissions;
        }

        /**
         * When we load the existing permission data model
         * we need to run through all the permissions and set the 
         * selected property
         */
        function initPermissions() {
            if (!vm.permissionGroups || !vm.permissionGroups.length) return;

            var hasModel = !!(vm.model && vm.model.length);
            _.each(vm.permissionGroups, function (group) {

                _.each(group.permissions, function (permission) {

                    permission.selected = hasModel && !!_.find(vm.model, function (permissionCommandData) {
                        return permission.uniqueId === makeUniquePermissionId(permissionCommandData.permissionCode, permissionCommandData.entityDefinitionCode);
                    });

                    permissionChanged(permission, group, true);
                });
            });
        }

        function makeUniquePermissionId(permissionTypeCode, entityDefinitionCode) {

            var uniqueId = 'permission' + permissionTypeCode;
            if (entityDefinitionCode) {
                uniqueId += entityDefinitionCode;
            }

            return uniqueId;
        }

        function loadPermissionsGrid() {

            return permissionService
                .getAll()
                .then(load);

            function load(result) {

                if (result) {
                    vm.permissions = result;
                    vm.permissionGroups = _.chain(vm.permissions)
                        .groupBy(function (permission) {
                            return permission.entityDefinition ? permission.entityDefinition.name : 'Misc';
                        })
                        .map(function (value, key) {
                            var readPermission = getReadPermission(value);

                            return {
                                title: key,
                                isReadPermitted: !readPermission || readPermission.selected,
                                permissions: mapPermissions(value)
                            }
                        })
                        .sortBy('title')
                        .value();

                    initPermissions();
                }
                vm.permissionsLoadState.off();
            }

            function getReadPermission(permissions) {
                var readPermission = _.find(permissions, function (permission) {
                    return permission.permissionType.code === PERMISSIONCODE_READ;
                });

                return readPermission;
            }

            function mapPermissions(permissions) {
                return _.sortBy(permissions, function (permission) {
                    var type = permission.permissionType;

                    permission.uniqueId = makeUniquePermissionId(permission.permissionType.code, permission.entityDefinition ? permission.entityDefinition.entityDefinitionCode : '');

                    switch (type.code) {
                        case 'COMRED':
                            permission.isRead = true;
                            return 'AAAA1';
                        case 'COMMOD':
                            return 'AAAA2';
                        case 'COMCRT':
                            return 'AAAA3';
                        case 'COMUPD':
                            return 'AAAA4';
                        case 'COMDEL':
                            return 'AAAA5';
                        default:
                            return type.name;
                    }
                    return permission.permissionType.code;
                })
            }
        }
    }

}]);
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
angular.module('cms.roles').controller('RoleListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'shared.permissionValidationService',
    'roles.roleService',
function (
    _,
    LoadState,
    SearchQuery,
    urlLibrary,
    permissionValidationService,
    rolesService) {

    var vm = this;

    init();

    function init() {
        
        vm.urlLibrary = urlLibrary;
        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

        vm.canCreate = permissionValidationService.canCreate('COFROL');
        vm.canUpdate = permissionValidationService.canUpdate('COFROL');

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

        return rolesService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }

}]);