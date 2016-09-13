angular.module('cms.pageTemplates').controller('ReScanTemplateSectionsController', [
    '_',
    '$scope',
    '$q',
    'shared.LoadState',
    'pageTemplates.pageTemplateService',
    'options',
    'close',
function (
    _,
    $scope,
    $q,
    LoadState,
    pageTemplateService,
    options,
    close) {

    // These constants used as display values and as a status in code
    var STATUS_ADDED = 'Added',
        STATUS_DELETED = 'Deleted'
        STATUS_UPDATED = 'Updated';

    init();
    
    /* INIT */

    function init() {

        $scope.pageTemplate = options.pageTemplate;

        $scope.formLoadState = new LoadState(true);
        $scope.submitLoadState = new LoadState(false);
        
        $scope.save = save;
        $scope.close = close;
        $scope.onRenameAsChanged = onRenameAsChanged;

        loadGrid();
    }

    /* EVENTS */
    
    function save() {
        var pageTemplate = $scope.pageTemplate,
            deletionCommands = [],
            updateCommands = [],
            additionCommands = [],
            def = $q.defer(),
            executionPromise = def.promise;

        def.resolve();

        // Set loading
        $scope.submitLoadState.on();

        // map commands
        _.each($scope.sections, function (section) {
            if (section.status === STATUS_ADDED && section.renameAs) {
                updateCommands.push(renameSection.bind(null, section));
            } else if (section.status === STATUS_UPDATED) {
                updateCommands.push(updateSection.bind(null, section));
            } else if (section.status === STATUS_ADDED) {
                additionCommands.push(addSection.bind(null, section));
            } else if (section.status === STATUS_DELETED) {
                deletionCommands.push(removeSection.bind(null, section));
            }
        });

        if ($scope.newCustomEntityModelType) {
            executionPromise = pageTemplateService.update({
                pageTemplateId: pageTemplate.pageTemplateId,
                customEntityModelType: $scope.newCustomEntityModelType
            });
        }
        // Execute them in this order to ensure we don't have naming conflicts
        _.each(deletionCommands, chainExecution);
        _.each(updateCommands, chainExecution);
        _.each(additionCommands, chainExecution);

        // close
        chainExecution(close);

        /* Helpers */

        function addSection(section) {
            return pageTemplateService.addSection({
                pageTemplateId: pageTemplate.pageTemplateId,
                name: section.name,
                isCustomEntitySection: section.isCustomEntitySection,
                permitAllModuleTypes: true
            });
        }

        function renameSection(section) {

            return pageTemplateService.updateSection({
                pageTemplateId: pageTemplate.pageTemplateId,
                pageTemplateSectionId: section.pageTemplateSectionId,
                isCustomEntitySection: section.isCustomEntitySection,
                name: section.renameAs
            });
        }

        function updateSection(section) {

            return pageTemplateService.updateSection({
                pageTemplateId: pageTemplate.pageTemplateId,
                pageTemplateSectionId: section.pageTemplateSectionId,
                isCustomEntitySection: section.isCustomEntitySection
            });
        }

        function removeSection(section) {

            return pageTemplateService.removeSection(pageTemplate.pageTemplateId, section.pageTemplateSectionId);
        }

        function chainExecution(command) {
            executionPromise = executionPromise.then(command);
        }
    }

    /**
     * When we select to rename a deleted section as one of the
     * new selections, we need to remove it from the list so the user
     * cannot select the same module to be renamed twice.
     */
    function onRenameAsChanged() {
        var allOptions = _.filter($scope.deletedSections, function (deletedSection) {
            var renamedSection = _.find($scope.sections, function (section) {
                return section.renameAs && section.renameAs.name === deletedSection.name;
            });

            return !renamedSection;
        });

        // re-add any selected items
        _.each($scope.sections, function (section) {
            if (section.status === STATUS_ADDED) {
                section.renameAsOptions = _.clone(allOptions);
                if (section.renameAs) {
                    section.renameAsOptions.push(section.renameAs);
                }
            }
        });
    }

    /* PRIVATE FUNCS */

    function loadGrid() {
        var pageTemplate = $scope.pageTemplate;

        // clone the sections so we don't modify the original object
        $scope.sections = _.map(pageTemplate.sections, _.clone);

        pageTemplateService
            .parseFile(pageTemplate.fullPath)
            .then(onLoaded)
            .then($scope.formLoadState.off);

        /* helpers */

        function onLoaded(fileInfo) {
            var deletedSections = [],
                fileSections = fileInfo.sections;

            $scope.newCustomEntityModelType = fileInfo.customEntityModelType;
            $scope.hasModelChanged = fileInfo.customEntityModelType != pageTemplate.customEntityModelType;

            // loop through the sections and work out which sections have 
            // been added/removed
            _.each(pageTemplate.sections, function (section) {
                var modifiedSection,
                    matchingFileSection = _.find(fileSections, function (fileSection) {
                    return fileSection.name == section.name;
                });

                if (matchingFileSection && matchingFileSection.isCustomEntitySection === section.isCustomEntitySection) {
                    matchingFileSection.status = 'No Change';
                } else if (matchingFileSection) {
                    $scope.hasChanges = true;
                    $scope.hasUpdates = true;
                    _.extend(matchingFileSection, section, {
                        status: STATUS_UPDATED,
                        statusCls: 'warning',
                        isCustomEntitySection: matchingFileSection.isCustomEntitySection
                    });

                } else {
                    $scope.hasChanges = true;
                    $scope.hasDeletions = true;

                    modifiedSection = _.extend({
                        status: STATUS_DELETED,
                        statusCls: 'error'
                    }, section);

                    fileSections.push(modifiedSection);
                    deletedSections.push(modifiedSection);
                }
            });

            _.each(fileSections, function (section) {
                if (!section.status) {
                    $scope.hasChanges = true;
                    section.status = STATUS_ADDED;
                    section.statusCls = 'success';
                }
            });

            // keep a list of deleted sections which we can use to offer a 'rename' option
            $scope.deletedSections = deletedSections;
            $scope.sections = fileSections;

            // init the renameAsOptions property on each section
            onRenameAsChanged();
        }
    }

}]);