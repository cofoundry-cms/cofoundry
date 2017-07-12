angular.module('cms.images').controller('ImageDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'images.imageService',
    'images.modulePath',
function (
    $routeParams,
    $q,
    $location,
    _,
    LoadState,
    modalDialogService,
    permissionValidationService,
    imageService,
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
        vm.remove = remove;
        
        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.permissions = permissionValidationService;

        // Init
        initData(vm.formLoadState);
    }
    
    /* UI ACTIONS */

    function edit() {
        vm.editMode = true;
        vm.mainForm.formStatus.clear();
    }

    function save() {
        setLoadingOn(vm.saveLoadState);

        imageService
            .update(vm.command)
            .then(onSuccess.bind(null, 'Changes were saved successfully', vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.previewImage = _.clone(vm.image);
        vm.command = mapCommand(vm.image);
        vm.mainForm.formStatus.clear();
    }
    
    function remove() {
        var options = {
            title: 'Delete Image',
            message: 'Are you sure you want to delete this image?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return imageService
                .remove(vm.image.imageAssetId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }
    
    /* PRIVATE FUNCS */

    function onSuccess(message, loadStateToTurnOff) {
        return initData(loadStateToTurnOff)
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData(loadStateToTurnOff) {

        return getImage()
            .then(setLoadingOff.bind(null, loadStateToTurnOff));
           
        /* helpers */

        function getImage() {
            return imageService.getById($routeParams.id).then(function (image) {
                vm.image = image;
                vm.previewImage = image;
                vm.command = mapCommand(image);
                vm.editMode = false;
            });
        }
    }

    function mapCommand(image) {

        return _.pick(image,
                'imageAssetId',
                'title',
                'tags',
                'defaultAnchorLocation');
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