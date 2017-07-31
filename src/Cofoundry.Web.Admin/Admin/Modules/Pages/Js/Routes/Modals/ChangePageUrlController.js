angular.module('cms.pages').controller('ChangePageUrlController', [
    '$scope',
    '$q',
    '$location',
    'shared.LoadState',
    'shared.pageService',
    'pages.customEntityService',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    LoadState,
    pageService,
    customEntityService,
    options,
    close) {

    var loadLocalesDeferred = $q.defer(),
        loadPageDirectoryDeferred = $q.defer();

    init();
    
    /* INIT */

    function init() {

        initData();

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(true);

        $scope.save = save;
        $scope.close = close;
        $scope.localesLoaded = loadLocalesDeferred.resolve;
        $scope.pageDirectoriesLoaded = loadPageDirectoryDeferred.resolve;
        $scope.formLoadState.offWhen(loadLocalesDeferred, loadPageDirectoryDeferred, loadRoutingRules());
    }

    function initData() {
        var page = options.page,
            pageRoute = page.pageRoute;

        $scope.isCustomEntityRoute = pageRoute.pageType === 'CustomEntityDetails';

        $scope.page = page;
        $scope.command = {
            pageId: page.pageId,
            localeId: pageRoute.locale ? pageRoute.locale.localeId : undefined,
            pageDirectoryId: pageRoute.pageDirectory.pageDirectoryId
        };

        if ($scope.isCustomEntityRoute) {
            $scope.command.customEntityRoutingRule = pageRoute.urlPath;
        } else {
            $scope.command.urlPath = pageRoute.urlPath;
        }
    }

    function loadRoutingRules() {

        if ($scope.isCustomEntityRoute) {
            return customEntityService
                .getAllRoutingRules()
                .then(function (routingRules) {
                    $scope.routingRules = routingRules;
                });
        } 
        
        var def = $q.defer();
        def.resolve();
        return def;
    }

    /* EVENTS */

    function save() {
        $scope.submitLoadState.on();

        pageService
            .updateUrl($scope.command)
            .then(options.onSave)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

}]);