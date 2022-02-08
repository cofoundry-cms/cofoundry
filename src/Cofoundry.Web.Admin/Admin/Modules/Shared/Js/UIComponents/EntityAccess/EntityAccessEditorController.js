angular.module('cms.shared').controller('EntityAccessEditorController', [
    '$scope',
    '$q',
    'shared.LoadState',
    'shared.userAreaService',
    'shared.roleService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'shared.internalModulePath',
    'shared.permissionValidationService',
    'shared.urlLibrary',
    'options',
    'close',
function (
    $scope,
    $q,
    LoadState,
    userAreaService,
    roleService,
    modalDialogService,
    arrayUtilities,
    modulePath,
    permissionValidationService,
    urlLibrary,
    options,
    close) {

    var vm = $scope;

    init();
    
    /* INIT */

    function init() {
 
        // UI actions
        vm.save = save;
        vm.close = close;
        vm.add = add;
        vm.deleteRule = deleteRule;

        // Properties
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);
        vm.urlLibrary = urlLibrary;

        // permissions
        vm.canManage = permissionValidationService.hasPermission(options.entityDefinitionCode + 'ACCRUL');
        vm.editMode = vm.canManage;

        // Init
        initData(vm.formLoadState);
    }

    /* UI ACTIONS */

    function save() {
        vm.command.accessRules = _.map(vm.accessRuleSet.accessRules, function(rule) {
            var idProp = options.entityIdPrefix + 'AccessRuleId';
            var command = {
                userAreaCode: rule.userArea.userAreaCode,
                roleId: rule.role ? rule.role.roleId : null
            };

            command[idProp] = rule[idProp];

            return command;
        });

        if (!vm.command.redirectoToSignIn) {
            vm.command.userAreaCodeForSignInRedirect = null;
        }

        setLoadingOn(vm.saveLoadState);

        options.saveAccess(vm.command)
            .then(onSuccess.bind(null, 'Access rules updated successfully'))
            .then(close)
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function add() {

        modalDialogService.show({
            templateUrl: modulePath + 'UIComponents/EntityAccess/AddEntityAccessRule.html',
            controller: 'AddEntityAccessRuleController',
            options: {
                onSave: onAddRule
            }
        });
    }
    
    function deleteRule(rule, $index) {

        arrayUtilities.removeObject(vm.accessRuleSet.accessRules, rule);
        setUserAreasInRules();
    }

    /* EVENTS */

    function onAddRule(command) {
        
        var rule = {};

        var duplicateRule = _.find(vm.accessRuleSet.accessRules, function(accessRule) {
            var roleId = accessRule.role ? accessRule.role.roleId : null;
            return accessRule.userArea.userAreaCode === command.userAreaCode && roleId == command.roleId;
        });

        if (duplicateRule) {
            return;
        }

        setLoadingOn();

        $q.all([getRole(), getUserArea()])
            .then(function() {
                vm.accessRuleSet.accessRules.push(rule);
                sortRules();
                setUserAreasInRules();
            })
            .finally(setLoadingOff);

        function getUserArea() {
            return userAreaService
                .getByCode(command.userAreaCode)
                .then(function(userArea) {
                    rule.userArea = userArea;
                });
        }

        function getRole() {
            if (!command.roleId) {
                return $q(function(resolve) { resolve() });
            }

            return roleService
                .getById(command.roleId)
                .then(function(role) {
                    rule.role = role;
                });
        }
    }

    function onSuccess(message, loadStateToTurnOff) {

        return initData(loadStateToTurnOff)
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    /* PRIVATE FUNCS */

    function initData(loadStateToTurnOff) {

        vm.entityDefinitionName = options.entityDefinitionName;
        vm.entityDefinitionNameLower = options.entityDefinitionName.toLowerCase();
        vm.entityDescription = options.entityDescription;
        vm.violationActions = [{
            id: 'Error',
            name: 'Error',
            description: 'Error (403: Forbidden)'
        }, {
            id: 'NotFound',
            name: 'Not Found',
            description: 'Not Found (404: Not Found)'
        }];

        return options.entityAccessLoader()
            .then(function (accessRuleSet) {
                vm.accessRuleSet = accessRuleSet;
                vm.command = mapUpdateCommand(accessRuleSet);
                vm.inheritedRules = [];
                
                _.each(vm.accessRuleSet.inheritedAccessRules, function (inheritedAccessRuleSet) {
                    inheritedAccessRuleSet.violationAction = _.findWhere(vm.violationActions, { id: inheritedAccessRuleSet.violationAction });
                    if (inheritedAccessRuleSet.userAreaForSignInRedirect) {
                        inheritedAccessRuleSet.signInRedirect = 'Yes';
                        inheritedAccessRuleSet.signInRedirectDescription = 'If the user is not signed in, then they will be redirected to the sign in page associated with the ' + inheritedAccessRuleSet.userAreaForSignInRedirect.name + ' user area.';
                    } else {
                        inheritedAccessRuleSet.signInRedirect = 'No';
                        inheritedAccessRuleSet.signInRedirectDescription = 'No sign in redirection, the default action will trigger instead.';
                    }

                    _.each(inheritedAccessRuleSet.accessRules, function(rule) {
                        rule.accessRuleSet = inheritedAccessRuleSet;
                        vm.inheritedRules.push(rule);
                    });
                });

                setUserAreasInRules();
            })
            .then(setLoadingOff.bind(null, loadStateToTurnOff));
    }

    function mapUpdateCommand(accessRuleSet) {

        var command = _.pick(accessRuleSet,
            options.entityIdPrefix + 'Id',
            'userAreaCodeForSignInRedirect',
            'violationAction'
        );
        
        if (accessRuleSet.userAreaForSignInRedirect) {
            command.userAreaCodeForSignInRedirect = accessRuleSet.userAreaForSignInRedirect.userAreaCode;
            command.redirectoToSignIn = true;
        }

        return command;
    }

    function setUserAreasInRules() {
        vm.userAreasInRules = _(vm.accessRuleSet.accessRules)
            .chain()
            .map(function(rule) { return rule.userArea; })
            .uniq(function(userArea) { return userArea.userAreaCode; })
            .sortBy('userAreaCode')
            .value();

        if (!vm.userAreasInRules.length) {
            // all user areas have been removed from the list
            vm.command.redirectoToSignIn = false;
            vm.command.userAreaCodeForSignInRedirect = null;
        } else if (!_.find(vm.userAreasInRules, function(userArea) { return userArea.userAreaCode === vm.command.userAreaCodeForSignInRedirect; })) {
            // the selected user area has been removed from the list
            vm.command.redirectoToSignIn = false;
            vm.command.userAreaCodeForSignInRedirect = null;
        }
        
        if (!vm.command.userAreaCodeForSignInRedirect && vm.userAreasInRules.length) {
            // set a default selection in-case the list is hidden
            vm.command.userAreaCodeForSignInRedirect = vm.userAreasInRules[0].userAreaCode;
        } 
    }

    function sortRules() {
        vm.accessRuleSet.accessRules = _(vm.accessRuleSet.accessRules)
            .chain()
            .sortBy(function (rule) {
                return rule.role ? rule.role.roleId : -1;
            })
            .sortBy(function (rule) {
                return rule.userArea.userAreaCode;
            })
            .value();
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