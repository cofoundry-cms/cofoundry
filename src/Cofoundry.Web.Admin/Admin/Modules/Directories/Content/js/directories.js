angular
    .module('cms.directories', ['ngRoute', 'cms.shared'])
    .constant('_', window._)
    .constant('directories.modulePath', '/Admin/Modules/Directories/Js/');
angular.module('cms.directories').config([
    '$routeProvider',
    'shared.routingUtilities',
    'directories.modulePath',
function (
    $routeProvider,
    routingUtilities,
    modulePath) {

    routingUtilities.registerCrudRoutes($routeProvider, modulePath, 'Directory');
}]);
angular.module('cms.directories').factory('directories.directoryService', [
    '$http',
    '_',
    'shared.serviceBase',
    'directories.DirectoryTree',
function (
    $http,
    _,
    serviceBase,
    DirectoryTree) {

    var service = {},
        directoryServiceBase = serviceBase + 'webdirectories';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(directoryServiceBase);
    }

    service.getTree = function () {
        return $http.get(directoryServiceBase + '/tree').then(function (tree) {
            return tree ? new DirectoryTree(tree) : tree;
        });
    }

    service.getById = function (webDirectoryId) {

        return $http.get(getIdRoute(webDirectoryId));
    }

    /* COMMANDS */

    service.add = function (command) {

        return $http.post(directoryServiceBase, command);
    }

    service.update = function (command) {

        return $http.patch(getIdRoute(command.webDirectoryId), command);
    }

    service.remove = function (webDirectoryId) {

        return $http.delete(getIdRoute(webDirectoryId));
    }

    /* PRIVATES */

    function getIdRoute(webDirectoryId) {
        return directoryServiceBase + '/' + webDirectoryId;
    }

    return service;
}]);
angular.module('cms.directories').factory('directories.DirectoryTree', [
    '_',
function (
    _) {

    return DirectoryTree;

    /**
     * Represents a query for searching entities and returning a paged result, handling
     * the persistance of the query parameters in the query string.
     */
    function DirectoryTree(originalTree) {
        var me = this;

        _.extend(me, originalTree);

        /* Public Funcs */

        /**
         * Flattens the node tree into a single array of nodes, optionally
         * excluding the directory with the specified id.
         */
        me.flatten = function (webDirectoryIdToExclude) {
            var allNodes = [];

            flattenNode(me, allNodes)

            return allNodes;

            function flattenNode(node, allNodes) {
                if (node.webDirectoryId == webDirectoryIdToExclude) return;
                allNodes.push(node);

                _.each(node.childWebDirectories, function (node) {
                    flattenNode(node, allNodes);
                });
            }
        }
        
        /**
         * Finds a directory node, searching through child nodes recursively.
         */
        me.findNodeById = function (webDirectoryIdToFind) {
            return findDirectory([me]);

            function findDirectory(directories) {
                var result;

                if (!directories) return;

                directories.forEach(function (directory) {
                    if (result) return;

                    if (directory.webDirectoryId == webDirectoryIdToFind) {
                        result = directory;
                    } else {
                        result = findDirectory(directory.childWebDirectories);
                    }

                });

                return result;
            }
        }
    }
}]);
angular.module('cms.directories').directive('cmsDirectoryGrid', [
    'directories.modulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DirectoryGrid.html',
        scope: {
            webDirectories: '=cmsDirectories',
            startDepth: '=cmsStartDepth',
            redirect: '=cmsRedirect'
        },
        replace: false,
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        vm.getPathDepthIndicator = getPathDepthIndicator;

        /* View Helpers */

        function getPathDepthIndicator(depth) {
            var depthIndicator = '',
                startDepth = (vm.startDepth || 0) + 1;

            for (var i = startDepth; i < depth; i++) {
                depthIndicator += '— ';
            }

            return depthIndicator;
        }
    }

}]);
angular.module('cms.directories').controller('AddDirectoryController', [
    '$location',
    'shared.stringUtilities',
    'shared.LoadState',
    'directories.directoryService',
function (
    $location,
    stringUtilities,
    LoadState,
    directoryService) {

    var vm = this;

    init();

    /* INIT */

    function init() {

        initData();

        vm.formLoadState = new LoadState(true);
        vm.globalLoadState = new LoadState();
        vm.editMode = false;

        vm.save = save;
        vm.cancel = cancel;
        vm.onNameChanged = onNameChanged;
        vm.onDirectoriesLoaded = onDirectoriesLoaded;
    }

    /* EVENTS */

    function save() {
        vm.globalLoadState.on();

        directoryService
            .add(vm.command)
            .then(redirectToList, vm.globalLoadState.off);
    }

    /* PRIVATE FUNCS */

    function onDirectoriesLoaded() {
        vm.formLoadState.off();
    }

    function onNameChanged() {
        vm.command.urlPath = stringUtilities.slugify(vm.command.name);
    }

    function cancel() {
        redirectToList();
    }

    function redirectToList() {
        $location.path('/');
    }

    function initData() {
        vm.command = {};
    }
}]);
angular.module('cms.directories').controller('DirectoryDetailsController', [
    '$routeParams',
    '$q',
    '$location',
    '_',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.modalDialogService',
    'directories.directoryService',
    'directories.modulePath',
function (
    $routeParams,
    $q,
    $location,
    _,
    stringUtilities,
    LoadState,
    modalDialogService,
    directoryService,
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
        vm.deleteDirectory = deleteDirectory;

        // Events
        vm.onNameChanged = onNameChanged;

        // Properties
        vm.editMode = false;
        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

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

        directoryService.update(vm.command)
            .then(onSuccess.bind(null, 'Changes were saved successfully'))
            .finally(setLoadingOff.bind(null, vm.saveLoadState));
    }

    function reset() {
        vm.editMode = false;
        vm.command = mapUpdateCommand(vm.webDirectory);
        vm.mainForm.formStatus.clear();
    }

    function deleteDirectory() {
        var options = {
            title: 'Delete Directory',
            message: 'Are you sure you want to delete this directory?',
            okButtonTitle: 'Yes, delete it',
            onOk: onOk
        };

        modalDialogService.confirm(options);

        function onOk() {
            setLoadingOn();
            return directoryService
                .remove(vm.webDirectory.webDirectoryId)
                .then(redirectToList)
                .catch(setLoadingOff);
        }
    }

    /* EVENTS */

    function onNameChanged() {
        if (!vm.hasChildContent) {
            vm.command.urlPath = stringUtilities.slugify(vm.command.name);
        }
    }

    /* PRIVATE FUNCS */

    function onSuccess(message) {
        return initData()
            .then(vm.mainForm.formStatus.success.bind(null, message));
    }

    function initData() {
        var webDirectoryId = $routeParams.id;

        return directoryService.getTree()
            .then(loadDirectory);

        function loadDirectory(tree) {
            var webDirectory = tree.findNodeById(webDirectoryId),
                parentDirectories = tree.flatten(webDirectoryId);

            vm.webDirectory = webDirectory;
            vm.parentDirectories = parentDirectories;
            vm.command = mapUpdateCommand(webDirectory);
            vm.editMode = false;
            vm.hasChildContent = webDirectory.numPages || webDirectory.childWebDirectories.length;
        }
    }

    function mapUpdateCommand(webDirectory) {

        return _.pick(webDirectory,
            'webDirectoryId',
            'name',
            'urlPath',
            'parentWebDirectoryId'
            );
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
angular.module('cms.directories').controller('DirectoryListController', [
    '_',
    'shared.modalDialogService',
    'shared.LoadState',
    'shared.SearchQuery',
    'directories.directoryService',
function (
    _,
    modalDialogService,
    LoadState,
    SearchQuery,
    directoryService) {

    var vm = this;

    init();

    function init() {
        
        vm.gridLoadState = new LoadState();

        loadGrid();
    }
    
    /* PRIVATE FUNCS */
    
    function loadGrid() {
        vm.gridLoadState.on();

        return directoryService.getTree().then(function (tree) {
            var result = tree.flatten();

            // remove the root directory
            vm.result = result.slice(1, result.length);
            vm.gridLoadState.off();
        });
    }

}]);