angular.module('cms.shared').controller('AddEntityAccessRuleController', [
    '$scope',
    '$q',
    'shared.LoadState',
    'shared.roleService',
    'shared.userAreaService',
    'options',
    'close',
function (
    $scope,
    $q,
    LoadState,
    roleService,
    userAreaService,
    options,
    close) {

    var vm = $scope;

    init();
    
    /* INIT */

    function init() {

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState();
        setLoadingOn(vm.formLoadState);

        vm.onAdd = onAdd;
        vm.onCancel = onCancel;
        vm.onUserAreaChanged = onUserAreaChanged;
        vm.searchRoles = searchRoles;

        initData();
    }
    
    /* EVENTS */

    function onUserAreaChanged() {
        vm.command.roleId = null;
    }

    function searchRoles(query) {
        if (!vm.command.userAreaCode) {
            var def = $q.defer();
            def.resolve();
            return def.promise;
        } 

        query.userAreaCode = vm.command.userAreaCode;
        query.excludeAnonymous = true;

        return roleService.search(query);
    }

    function onAdd() {
        options.onSave(vm.command);

        close();
    }

    function onCancel() {
        close();
    }

    /* PRIVATE FUNCS */

    function initData() {
        
        vm.command = {}; 
        onUserAreaChanged();

        userAreaService
            .getAll()
            .then(loadUserAreas)
            .finally(setLoadingOff.bind(null, vm.formLoadState));

        function loadUserAreas(userAreas) {
            vm.userAreas = _.filter(userAreas, function(userArea) { return userArea.userAreaCode !== 'COF' });

            if (vm.userAreas.length == 1) {
                vm.command.userAreaCode = vm.userAreas[0].userAreaCode;
            }
        }
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