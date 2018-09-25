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