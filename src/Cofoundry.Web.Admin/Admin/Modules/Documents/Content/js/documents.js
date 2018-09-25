angular
    .module('cms.documents', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('documents.modulePath', '/Admin/Modules/Documents/Js/');
angular.module('cms.documents').config([
    '$routeProvider',
    'shared.routingUtilities',
    'documents.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Document');

}]);
angular.module('cms.documents').factory('documents.documentService', [
        '$http',
        'shared.documentService',
    function (
        $http,
        sharedDocumentService) {

    var service = _.extend({}, sharedDocumentService);
        
    /* COMMANDS */

    service.update = function (command) {
        return service.uploadFile(service.getIdRoute(command.documentAssetId), command, 'PUT');
    }

    service.remove = function (id) {
        
        return $http.delete(service.getIdRoute(id));
    }

    return service;
}]);
angular.module('cms.documents').controller('AddDocumentController', [
            '$location',
            '_',
            'shared.focusService',
            'shared.stringUtilities',
            'shared.LoadState',
            'documents.documentService',
        function (
            $location,
            _,
            focusService,
            stringUtilities,
            LoadState,
            documentService
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

        documentService
            .add(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(redirectToList)
            .finally(vm.saveLoadState.off);
    }

    function onFileChanged() {
        var command = vm.command;

        if (command.file && command.file.name) {
            command.title = stringUtilities.capitaliseFirstLetter(stringUtilities.getFileNameWithoutExtension(command.file.name));
            command.fileName = stringUtilities.slugify(command.title);
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
angular.module('cms.documents').controller('DocumentDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.LoadState',
    'shared.modalDialogService',
    'shared.permissionValidationService',
    'shared.urlLibrary',
    'documents.documentService',
    'documents.modulePath',
function (
    $routeParams,
    $q, $location,
    _,
    LoadState,
    modalDialogService,
    permissionValidationService,
    urlLibrary,
    documentService,
    modulePath) {

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

        vm.canUpdate = permissionValidationService.canUpdate('COFDOC');
        vm.canDelete = permissionValidationService.canDelete('COFDOC');

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

        documentService
            .update(vm.command)
            .progress(vm.saveLoadState.setProgress)
            .then(onSuccess.bind(null, 'Changes were saved successfully', vm.saveLoadState))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.previewDocument = _.clone(vm.document);
        vm.command = mapCommand(vm.document);
        vm.previewUrl = urlLibrary.getDocumentUrl(vm.previewDocument);
        vm.mainForm.formStatus.clear();
    }
    
    function remove() {
        var options = {
            title: 'Delete Document',
            message: 'Are you sure you want to delete this document?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return documentService
                .remove(vm.document.documentAssetId)
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

        return getDocument()
            .then(setLoadingOff.bind(null, loadStateToTurnOff));
           
        /* helpers */

        function getDocument() {
            return documentService.getById($routeParams.id).then(function (document) {
                vm.document = document;
                vm.previewDocument = document;
                vm.command = mapCommand(document);
                vm.previewUrl = urlLibrary.getDocumentUrl(document);
                vm.editMode = false;
            });
        }
    }

    function mapCommand(document) {

        return _.pick(document,
                'documentAssetId',
                'title',
                'fileName',
                'description',
                'tags');
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
angular.module('cms.documents').controller('DocumentListController', [
    '_',
    'shared.LoadState',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'shared.permissionValidationService',
    'documents.documentService',
function (
    _,
    LoadState,
    SearchQuery,
    urlLibrary,
    permissionValidationService,
    documentService) {

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
        vm.getDocumentUrl = urlLibrary.getDocumentUrl;

        vm.canCreate = permissionValidationService.canCreate('COFDOC');
        vm.canUpdate = permissionValidationService.canUpdate('COFDOC');

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

        return documentService.getAll(vm.query.getParameters()).then(function (result) {
            vm.result = result;
            vm.gridLoadState.off();
        });
    }
}]);