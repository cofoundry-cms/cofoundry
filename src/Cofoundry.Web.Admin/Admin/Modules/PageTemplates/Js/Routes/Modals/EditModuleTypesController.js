angular.module('cms.pageTemplates').controller('EditModuleTypesController', [
    '_',
    '$scope',
    'shared.LoadState',
    'pageTemplates.pageModuleTypeService',
    'pageTemplates.pageTemplateService',
    'options',
    'close',
function (
    _,
    $scope,
    LoadState,
    pageModuleTypeService,
    pageTemplateService,
    options,
    close) {

    init();
    
    /* INIT */

    function init() {
        var section = options.section;

        $scope.section = section;
        $scope.command = _.pick(section,
            'hasAllModuleTypes'
            );

        $scope.formLoadState = new LoadState(true);
        $scope.submitLoadState = new LoadState(false);

        $scope.save = save;
        $scope.close = close;

        loadModuleTypes();
    }

    /* EVENTS */
    
    function save() {
        var section = $scope.section;

        if (section.pageTemplateSectionId) {
            // This is an existing section, so update it on the server
            saveToServer();
        } else {
            // This section hasn't been saved yet so just update the raw section object
            // because it will be saved later in a batch.
            saveLocally();
        }

        /* Helpers */

        function saveToServer() {
            $scope.submitLoadState.on();

            var command = {
                pageTemplateId : section.pageTemplateId,
                pageTemplateSectionId: section.pageTemplateSectionId,
                permitAllModuleTypes: !!$scope.command.hasAllModuleTypes,
                permittedModuleTypeIds: _.map(parseModuleTypes(), function (pageModuleType) {
                    return pageModuleType.pageModuleTypeId;
                })
            };

            pageTemplateService
                .updateSectionModuleTypes(command)
                .finally($scope.submitLoadState.off)
                .then(close);
        }

        function saveLocally() {
            section.hasAllModuleTypes = !!$scope.command.hasAllModuleTypes;
            section.moduleTypes = parseModuleTypes();

            close();
        }

        function parseModuleTypes() {
            if ($scope.command.hasAllModuleTypes) return [];

            return _.filter($scope.moduleTypes, function (pageModuleType) {
                return pageModuleType.selected === true;
            });
        }
    }


    /* PRIVATE FUNCS */

    function loadModuleTypes() {
        pageModuleTypeService
            .getAll()
            .then(onLoaded)
            .then($scope.formLoadState.off);

        function onLoaded(moduleTypes) {
            if (!$scope.command.hasAllModuleTypes) {
                _.each(moduleTypes, function (moduleType) {
                    moduleType.selected = isModuleSelected(moduleType);
                });
            }

            $scope.moduleTypes = moduleTypes;
            $scope.moduleTypeGroups = _.groupBy(moduleTypes, 'isCustom');

            // private funcs

            function isModuleSelected(moduleTypeToFind) {
                return _.find($scope.section.moduleTypes, function(sectionModuleType) {
                    return moduleTypeToFind.pageModuleTypeId == sectionModuleType.pageModuleTypeId;
                }) !== undefined;
            }
        }
    }

}]);