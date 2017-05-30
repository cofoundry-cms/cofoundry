angular
    .module('cms.images', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('images.modulePath', '/Admin/Modules/Images/Js/');
angular.module('cms.images').config([
    '$routeProvider',
    'shared.routingUtilities',
    'images.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Image');

}]);
angular.module('cms.images').factory('images.imageService', [
        '$http',
        '$upload',
        'shared.imageService',
    function (
        $http,
        $upload,
        sharedImageService) {

    var service = _.extend({}, sharedImageService);

    /* COMMANDS */

    service.remove = function (id) {

        return $http.delete(service.getIdRoute(id));
    }

    return service;
}]);
angular.module('cms.images').controller('AddImageController', [
            '$location',
            '_',
            'shared.focusService',
            'shared.stringUtilities',
            'shared.LoadState',
            'images.imageService',
        function (
            $location,
            _,
            focusService,
            stringUtilities,
            LoadState,
            imageService
        ) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        initData();

        vm.save = save;
        vm.cancel = cancel;
        vm.onFileChanged = onFileChanged;

        vm.editMode = false;
        vm.saveLoadState = new LoadState();
    }

    /* EVENTS */

    function save() {
        vm.saveLoadState.on();

        imageService
            .add(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(redirectToList);
    }

    function onFileChanged() {
        var command = vm.command;

        if (command.file && command.file.name) {
            command.title = stringUtilities.capitaliseFirstLetter(stringUtilities.getFileNameWithoutExtension(command.file.name));
            focusService.focusById('title');
        }
    }

    /* PRIVATE FUNCS */

    function initData() {
        vm.command = {};
    }

    function cancel() {
        redirectToList();
    }

    function redirectToList() {
        $location.path('');
    }
}]);
angular.module('cms.images').controller('ImageDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.LoadState',
    'shared.modalDialogService',
    'images.imageService',
    'images.modulePath',
function (
    $routeParams,
    $q,
    $location,
    _,
    LoadState,
    modalDialogService,
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
angular.module('cms.images').controller('ImageListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'images.imageService',
function (
    _,
    LoadState,
    SearchQuery,
    imageService) {

    /* START */

    var vm = this;
    init();
    
    /* INIT */
    function init() {

        vm.gridLoadState = new LoadState();
        vm.query = new SearchQuery({
            onChanged: onQueryChanged
        });
        vm.filter = vm.query.getFilters();
        vm.toggleFilter = toggleFilter;

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

    /* HELPERS */

    function loadGrid() {
        vm.gridLoadState.on();

        return imageService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
}]);