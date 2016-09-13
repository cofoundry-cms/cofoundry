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