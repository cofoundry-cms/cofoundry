angular.module('cms.shared').directive('cmsFormFieldImageAssetCollection', [
    '_',
    'shared.internalModulePath',
    'shared.LoadState',
    'shared.imageService',
    'shared.modalDialogService',
    'shared.arrayUtilities',
    'shared.stringUtilities',
    'shared.urlLibrary',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    LoadState,
    imageService,
    modalDialogService,
    arrayUtilities,
    stringUtilities,
    urlLibrary,
    baseFormFieldFactory) {

    /* VARS */

    var IMAGE_ASSET_ID_PROP = 'imageAssetId',
        baseConfig = baseFormFieldFactory.defaultConfig;

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageAssetCollection.html',
        passThroughAttributes: [
            'required',
            'ngRequired'
        ],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm;

        init();
        return baseConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.gridLoadState = new LoadState();
            vm.urlLibrary = urlLibrary;

            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.onDrop = onDrop;

            scope.$watch("vm.model", setGridItems);
        }

        /* EVENTS */

        function remove(image) {

            removeItemFromArray(vm.gridData, image);
            removeItemFromArray(vm.model, image.imageAssetId);

            function removeItemFromArray(arr, item) {
                var index = arr.indexOf(item);

                if (index >= 0) {
                    return arr.splice(index, 1);
                }
            }
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/ImageAssets/ImageAssetPickerDialog.html',
                controller: 'ImageAssetPickerDialogController',
                options: {
                    selectedIds: vm.model || [],
                    filter: getFilter(),
                    onSelected: onSelected
                }
            });

            function onSelected(newImageArr) {
                vm.model = newImageArr;
                setGridItems(newImageArr);
            }
        }

        function onDrop($index, droppedEntity) {

            arrayUtilities.moveObject(vm.gridData, droppedEntity, $index, IMAGE_ASSET_ID_PROP);

            // Update model with new orering
            setModelFromGridData();
        }

        function setModelFromGridData() {
            vm.model = _.pluck(vm.gridData, IMAGE_ASSET_ID_PROP);
        }

        /* HELPERS */

        function getFilter() {
            var filter = {},
                attributePrefix = 'cms';

            setAttribute('Tags');
            setAttribute('Width', true);
            setAttribute('Height', true);
            setAttribute('MinWidth', true);
            setAttribute('MinHeight', true);

            return filter;

            function setAttribute(filterName, isInt) {
                var value = attributes[attributePrefix + filterName];

                if (value) {
                    filterName = stringUtilities.lowerCaseFirstWord(filterName);
                    filter[filterName] = isInt ? parseInt(value) : value;
                }
            }
        }

        /** 
         * Load the grid data if it is inconsistent with the Ids collection.
         */
        function setGridItems(ids) {

            if (!ids || !ids.length) {
                vm.gridData = [];
            }
            else if (!vm.gridData || _.pluck(vm.gridData, IMAGE_ASSET_ID_PROP).join() != ids.join()) {

                vm.gridLoadState.on();
                imageService.getByIdRange(ids).then(function (items) {
                    vm.gridData = items;
                    vm.gridLoadState.off();
                });
            }
        }
    }
}]);