angular.module('cms.customEntities').controller('DuplicateCustomEntityController', [
    '$scope',
    '$q',
    '$location',
    'shared.stringUtilities',
    'shared.LoadState',
    'shared.customEntityService',
    'customEntities.options',
    'options',
    'close',
function (
    $scope,
    $q,
    $location,
    stringUtilities,
    LoadState,
    customEntityService,
    customEntityOptions,
    options,
    close) {

    var loadLocalesDeferred = $q.defer();

    init();
    
    /* INIT */

    function init() {
       
        initData();

        $scope.submitLoadState = new LoadState();
        $scope.formLoadState = new LoadState(customEntityOptions.hasLocale);
        $scope.customEntityDefinition = customEntityOptions;

        $scope.save = save;
        $scope.close = close;
        $scope.onTitleChanged = onTitleChanged;

        $scope.localesLoaded = loadLocalesDeferred.resolve;
        $scope.formLoadState.offWhen(loadLocalesDeferred);
    }

    /* EVENTS */

    function initData() {
        var customEntity = options.customEntity;

        $scope.customEntity = customEntity;
        $scope.command = {
            customEntityToDuplicateId: customEntity.customEntityId,
            localeId: customEntity.locale ? customEntity.locale.localeId : undefined,
            title: 'Copy of ' + customEntity.latestVersion.title
        }

        onTitleChanged();
    }

    function onTitleChanged() {
        $scope.command.urlSlug = stringUtilities.slugify($scope.command.title);
    }

    function save() {
        $scope.submitLoadState.on();

        customEntityService
            .duplicate($scope.command)
            .then(redirectoToEntity)
            .then(close)
            .finally($scope.submitLoadState.off);
    }

    /* PRIVATE FUNCS */

    function redirectoToEntity(customEntityId) {
        $location.path('/' + customEntityId);
    }
}]);