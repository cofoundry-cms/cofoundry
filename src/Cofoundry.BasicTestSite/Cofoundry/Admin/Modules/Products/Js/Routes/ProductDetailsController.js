angular.module('cms.products').controller('ProductDetailsController', [
    'shared.LoadState',
    'shared.permissionValidationService',
    'products.productService',
    'products.modulePath',
    function (
        LoadState,
        permissionValidationService,
        productService,
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

            // Properties
            vm.editMode = false;
            vm.globalLoadState = new LoadState();
            vm.saveLoadState = new LoadState();
            vm.formLoadState = new LoadState(true);

            vm.canUpdate = true;//permissionValidationService.canUpdate('COFCUR');

            // Init
            initData()
                .then(setLoadingOff.bind(null, vm.formLoadState));
        }

        /* UI ACTIONS */

        function edit() {
            vm.editMode = true;
            vm.mainForm.formStatus.clear();
        }

        function save() {
            setLoadingOn(vm.saveLoadState);

            productService.update(vm.command)
                .then(onSuccess.bind(null, 'Changes were saved successfully'))
                .finally(setLoadingOff.bind(null, vm.saveLoadState));
        }

        function reset() {
            vm.editMode = false;
            vm.command = mapUpdateCommand(vm.product);
            vm.mainForm.formStatus.clear();
        }

        /* PRIVATE FUNCS */

        function onSuccess(message) {
            return initData()
                .then(vm.mainForm.formStatus.success.bind(null, message));
        }

        function initData() {

            return productService
                .getProductDetails()
                .then(load);

            function load(product) {

                vm.product = product;
                vm.command = mapUpdateCommand(product);
                vm.editMode = false;
            }
        }

        function mapUpdateCommand(product) {

            return _.pick(product,
                'title',
                'ref',
                'test'
            );
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