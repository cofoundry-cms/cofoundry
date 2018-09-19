angular.module('cms.shared').directive('cmsFormFieldDocumentAssetCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.documentService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'shared.stringUtilities',
    'shared.urlLibrary',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    documentService,
    modalDialogService,
    arrayUtilities,
    stringUtilities,
    urlLibrary,
    baseFormFieldFactory) {

    /* VARS */

    var DOCUMENT_ASSET_ID_PROP = 'documentAssetId',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/DocumentAssets/FormFieldDocumentAssetCollection.html',
        passThroughAttributes: [
            'required'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.urlLibrary = urlLibrary;
            vm.gridLoadState = new LoadState();

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(document) {

            removeItemFromArray(vm.gridData, document);
            removeItemFromArray(vm.model, document[DOCUMENT_ASSET_ID_PROP]);

            function removeItemFromArray(arr, item) {
                var index = arr.indexOf(item);

                if (index >= 0) {
                    return arr.splice(index, 1);
                }
            }
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/DocumentAssets/DocumentAssetPickerDialog.html',
                controller: 'DocumentAssetPickerDialogController',
                options: {
                    selectedIds: vm.model || [],
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function onSelected(newArr) {
                vm.model = newArr;
                setGridItems(newArr);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, DOCUMENT_ASSET_ID_PROP);

            // Update model with new orering
            setModelFromGridData();
        }

        function setModelFromGridData() {
            vm.model = _.pluck(vm.gridData, DOCUMENT_ASSET_ID_PROP);
        }

        /* HELPERS */

        function getFilter() {
            var filter = {},
                attributePrefix = 'cms';

            setAttribute('Tags');
            setAttribute('FileExtension');
            setAttribute('FileExtensions');

            return filter;

            function setAttribute(attributeName) {
                var filterName = stringUtilities.lowerCaseFirstWord(attributeName);
                filter[filterName] = attributes[attributePrefix + attributeName];
            }
        }

        /** 
         * Load the grid data if it is inconsistent with the Ids collection.
         */
        function setGridItems(ids) {

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, DOCUMENT_ASSET_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                documentService.getByIdRange(ids).then(function (items) {
                    vm.gridData = items;
                    vm.gridLoadState.off();
                });
            }
        }
    }
}]);