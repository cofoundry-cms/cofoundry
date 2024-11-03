angular.module('cms.errors').controller('ErrorDetailsController', [
    '$routeParams',
    'shared.LoadState',
    'errors.errorService',
function (
    $routeParams,
    LoadState,
    errorService
    ) {

    var vm = this;

    init();
    
    /* INIT */

    function init() {

        // Properties
        vm.editMode = false;
        vm.formLoadState = new LoadState(true);

        // Init
        initData()
            .then(vm.formLoadState.off);
    }

    /* UI ACTIONS */

    function reset() {
        vm.command = mapUpdateCommand(vm.error);
        vm.mainForm.formStatus.clear();
    }

    /* PRIVATE FUNCS */

    function initData() {
        var errorId = $routeParams.id;

        return errorService.getById(errorId)
            .then(load);

        function load(error) {

            vm.error = error;
        }
    }
}]);