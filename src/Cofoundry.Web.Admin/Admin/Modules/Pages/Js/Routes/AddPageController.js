angular.module('cms.pages').controller('AddPageController', [
    '_',
    '$q',
    '$location',
    '$window',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.urlLibrary',
    'shared.pageService',
    'pages.pageTemplateService',
    'pages.customEntityService',
function (
    _,
    $q,
    $location,
    $window,
    LoadState,
    stringUtilities,
    urlLibrary,
    pageService,
    pageTemplateService,
    customEntityService
    ) {

    var vm = this,
        loadLocalesDeferred = $q.defer(),
        loadPageDirectoryDeferred = $q.defer();

    init();

    /* INIT */

    function init() {

        vm.save = save.bind(null, false, redirectToDetails);
        vm.saveAndPublish = save.bind(null, true, redirectToDetails);
        vm.saveAndEdit = save.bind(null, false, redirectToVisualEditor);
        vm.cancel = cancel;
        vm.onNameChanged = onNameChanged;
        vm.onPageTypeChanged = onPageTypeChanged;

        vm.globalLoadState = new LoadState();
        vm.saveLoadState = new LoadState();
        vm.saveAndPublishLoadState = new LoadState();
        vm.formLoadState = new LoadState(true);

        vm.onLocalesLoaded = loadLocalesDeferred.resolve;
        vm.onPageDirectoriesLoaded = loadPageDirectoryDeferred.resolve;

        initData();
    }

    /* EVENTS */

    function save(publish, redirectToCommand) {
        var loadState;

        if (publish) {
            vm.command.publish = true;
            loadState = vm.saveAndPublishLoadState;
        } else {
            loadState = vm.saveLoadState;
        }

        setLoadingOn(loadState);

        pageService
            .add(vm.command)
            .then(redirectToCommand)
            .finally(setLoadingOff.bind(null, loadState));

    }

    function redirectToDetails(id) {
        $location.path('/' + id);
    }

    function redirectToVisualEditor(id) {

        return pageService
            .getById(id)
            .then(redirect);

        function redirect(page) {
            $window.location.href = urlLibrary.visualEditorForPage(page.pageRoute, true);
        }
    }

    function onNameChanged() {
        vm.command.urlPath = stringUtilities.slugify(vm.command.title);
    }

    function onPageTypeChanged() {
        var selectedPageType = vm.command.pageType,
            filterBy = selectedPageType  == 'CustomEntityDetails' ? selectedPageType : 'Generic'; 
       
        vm.pageTemplates = _.where(vm.allPageTemplates, { pageType: filterBy });
    }

    /* PRIVATE FUNCS */

    function cancel() {
        $location.path('/');
    }

    function initData() {
        vm.pageTypes = pageService.getPageTypes();

        vm.command = {
            showInSiteMap: true,
            pageType: vm.pageTypes[0].value
        };

        var loadTemplatesDeferred = pageTemplateService
            .getAll()
            .then(function (pageTemplates) {
                vm.allPageTemplates = pageTemplates;
            });

        var loadRoutingRulesDeferred = customEntityService
            .getAllRoutingRules()
            .then(function (routingRules) {
                vm.routingRules = routingRules;
            });

        vm.formLoadState
            .offWhen(
                loadLocalesDeferred,
                loadPageDirectoryDeferred,
                loadTemplatesDeferred,
                loadRoutingRulesDeferred
                )
            .then(onPageTypeChanged);
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