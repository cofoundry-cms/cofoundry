angular.module('cms.pages').controller('PageAccessRuleListController', [
    '$scope',
    '$q',
    'shared.LoadState',
    'shared.pageService',
    'shared.userAreaService',
    'shared.roleService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'pages.modulePath',
    'options',
    'close',
function (
    $scope,
    $q,
    LoadState,
    pageService,
    userAreaService,
    roleService,
    modalDialogService,
    arrayUtilities,
    modulePath,
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

        // permissions
        vm.canManage = true; // TODO
        vm.editMode = vm.canManage;

        // Init
        initData(vm.formLoadState);
    }

    /* UI ACTIONS */

    function save() {
        vm.command.accessRules = _.map(vm.accessInfo.accessRules, function(rule) {
            return {
                pageAccessRuleId: rule.pageAccessRuleId,
                userAreaCode: rule.userArea.userAreaCode,
                roleId: rule.role ? rule.role.roleId : null
            }
        });

        if (!vm.command.redirectoToLogin) {
            vm.command.userAreaCodeForLoginRedirect = null;
        }

        setLoadingOn(vm.saveLoadState);

        pageService.updateAccessRules(vm.command)
            .then(onSuccess.bind(null, 'Access rules updated successfully'))
            .then(close)
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function add() {

        modalDialogService.show({
            templateUrl: modulePath + 'Routes/Modals/AddPageAccessRule.html',
            controller: 'AddPageAccessRuleController',
            options: {
                onSave: onAddRule
            }
        });
    }
    
    function deleteRule(rule, $index) {

        arrayUtilities.removeObject(vm.accessInfo.accessRules, rule);
        setUserAreasInRules();
    }

    /* EVENTS */

    function onAddRule(command) {
        
        var rule = {};

        var duplicateRule = _.find(vm.accessInfo.accessRules, function(accessRule) {
            var roleId = accessRule.role ? accessRule.role.roleId : null;
            return accessRule.userArea.userAreaCode === command.userAreaCode && roleId == command.roleId;
        });

        if (duplicateRule) {
            return;
        }

        setLoadingOn();

        $q.all([getRole(), getUserArea()])
            .then(function() {
                vm.accessInfo.accessRules.push(rule);
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
        var page = options.page;
        vm.page = page;

        vm.violationActions = [{
            id: 'Error',
            name: 'Error (403: Forbidden)'
        }, {
            id: 'NotFound',
            name: 'Not Found (404: Not Found)'
        }];

        return pageService
            .getAccessRulesByPageId(vm.page.pageId)
            .then(function (accessInfo) {
                vm.accessInfo = accessInfo;
                vm.command = mapUpdateCommand(accessInfo);
                setUserAreasInRules();
            })
            .then(setLoadingOff.bind(null, loadStateToTurnOff));
    }

    function mapUpdateCommand(accessInfo) {

        var command = _.pick(accessInfo,
            'pageId',
            'userAreaCodeForLoginRedirect',
            'violationAction'
        );
        
        command.redirectoToLogin = !!accessInfo.userAreaCodeForLoginRedirect;

        return command;
    }

    function setUserAreasInRules() {
        vm.userAreasInRules = _(vm.accessInfo.accessRules)
            .chain()
            .map(function(rule) { return rule.userArea; })
            .uniq(function(userArea) { return userArea.userAreaCode; })
            .sortBy('userAreaCode')
            .value();

        if (!vm.userAreasInRules.length) {
            // all user areas have been removed from the list
            vm.command.redirectoToLogin = false;
            vm.command.userAreaCodeForLoginRedirect = null;
        } else if (!_.find(vm.userAreasInRules, function(userArea) { return userArea.userAreaCode === vm.command.userAreaCodeForLoginRedirect; })) {
            // the selected user area has been removed from the list
            vm.command.redirectoToLogin = false;
            vm.command.userAreaCodeForLoginRedirect = null;
        }
        
        if (!vm.command.userAreaCodeForLoginRedirect) {
            // set a default selection in-case the list is hidden
            vm.command.userAreaCodeForLoginRedirect = vm.userAreasInRules[0].userAreaCode;
        } 
    }

    function sortRules() {
        vm.accessInfo.accessRules = _(vm.accessInfo.accessRules)
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