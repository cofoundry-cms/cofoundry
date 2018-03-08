angular.module('cms.shared').controller('EditNestedDataModelDialogController', [
    '$scope',
    'shared.focusService',
    'shared.stringUtilities',
    'options',
    'close',
function (
    $scope,
    focusService,
    stringUtilities,
    options,
    close) {
    
    var vm = $scope;
    init();
    
    /* INIT */
    function init() {
        angular.extend($scope, options);

        vm.save = onSave;
        vm.onCancel = onCancel;
        vm.close = onCancel;

        vm.formDataSource = {
            model: options.model || {},
            modelMetaData: options.modelMetaData
        }
    }

    /* EVENTS */

    function onSave() {

        if (options.onSave) options.onSave(vm.formDataSource.model);
        close();
    }

    function onCancel() {
        close();
    }

    /* PUBLIC HELPERS */

    function initData() {
        vm.command = {};
    }

    function cancel() {
        close();
    }
}]);
