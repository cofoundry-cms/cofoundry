angular.module('cms.shared').directive('cmsFormFieldPageCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.pageService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'shared.urlLibrary',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    pageService,
    modalDialogService,
    arrayUtilities,
    urlLibrary,
    baseFormFieldFactory) {

    /* VARS */

    var PAGE_ID_PROP = 'pageId',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/Pages/FormFieldPageCollection.html',
        scope: _.extend(baseConfig.scope, {
            localeId: '=cmsLocaleId',
            orderable: '=cmsOrderable'
        }),
        require: _.union(baseConfig.require, ['?^^cmsFormDynamicFieldSet']),
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            definitionPromise,
            dynamicFormFieldController = _.last(controllers);

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;
            vm.urlLibrary = urlLibrary;

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(page) {

            arrayUtilities.removeObject(vm.gridData, page);
            arrayUtilities.removeObject(vm.model, page[PAGE_ID_PROP]);
        }

        function showPicker() {
            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/Pages/PagePickerDialog.html',
                controller: 'PagePickerDialogController',
                options: {
                    selectedIds: vm.model || [],
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function onSelected(newEntityArr) {
                vm.model = newEntityArr
                setGridItems(newEntityArr);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, PAGE_ID_PROP);

            // Update model with new ordering
            setModelFromGridData();
        }

        function orderGridItemsAndSetModel() {
            if (!vm.orderable) {
                vm.gridData = _.sortBy(vm.gridData, function (page) {
                    return page.auditData.createDate;
                }).reverse();
                setModelFromGridData();
            }
        }

        function setModelFromGridData() {
            vm.model = _.pluck(vm.gridData, PAGE_ID_PROP);
        }

        /* HELPERS */

        function getFilter() {
            var filter = {},
                localeId;

            if (vm.localeId) {
                localeId = vm.localeId;
            } else if (dynamicFormFieldController && dynamicFormFieldController.additionalParameters) {
                localeId = dynamicFormFieldController.additionalParameters.localeId;
            }

            if (localeId) {
                filter.localeId = localeId;
            }

            return filter;
        }

        /** 
         * Load the grid data if it is inconsistent with the Ids collection.
         */
        function setGridItems(ids) {

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, PAGE_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                pageService
                    .getByIdRange(ids)
                    .then(loadPages)
                    .then(vm.gridLoadState.off);
            }

            function loadPages(items) {
                vm.gridData = items;
                orderGridItemsAndSetModel();
            }
        }
    }
}]);