/**
 * A form field control for an image asset that uses a search and pick dialog
 * to allow the user to change the selected file.
 */
angular.module('cms.shared').directive('cmsFormFieldDocumentAsset', [
    '_',
    'shared.internalModulePath',
    'shared.internalContentPath',
    'shared.modalDialogService',
    'shared.stringUtilities',
    'shared.documentService',
    'shared.urlLibrary',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    contentPath,
    modalDialogService,
    stringUtilities,
    documentService,
    urlLibrary,
    baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/DocumentAssets/FormFieldDocumentAsset.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState',
            updateAsset: '@cmsUpdateAsset' // update the asset property if it changes
        }),
        passThroughAttributes: ['required'],
        link: link
    };

    return baseFormFieldFactory.create(config);

    /* LINK */

    function link(scope, el, attributes, controllers) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required'),
            isAssetInitialized;

        init();
        return baseFormFieldFactory.defaultConfig.link(scope, el, attributes, controllers);

        /* INIT */

        function init() {

            vm.urlLibrary = urlLibrary;
            vm.showPicker = showPicker;
            vm.remove = remove;
            vm.isRemovable = _.isObject(vm.model) && !isRequired;

            vm.filter = parseFilters(attributes);

            scope.$watch("vm.asset", setAsset);
            scope.$watch("vm.model", setAssetById);
        }

        /* EVENTS */

        function remove() {
            setAsset(null);
        }

        function showPicker() {

            modalDialogService.show({
                templateUrl: modulePath + 'UIComponents/DocumentAssets/DocumentAssetPickerDialog.html',
                controller: 'DocumentAssetPickerDialogController',
                options: {
                    currentAsset: vm.previewAsset,
                    filter: vm.filter,
                    onSelected: onSelected
                }
            });

            function onSelected(newAsset) {
                if (!newAsset && vm.asset) {
                    setAsset(null);
                } else if (!vm.asset || (newAsset && vm.asset.documentAssetId !== newAsset.documentAssetId)) {
                    setAsset(newAsset);
                }
            }
        }

        /** 
         * When the model is set without a preview asset, we need to go get the full 
         * asset details. This query can be bypassed by setting the cms-asset attribute
         */
        function setAssetById(assetId) {

            // Remove the id if it is 0 or invalid to make sure required validation works
            if (!assetId) {
                vm.model = assetId = undefined;
            }

            if (assetId && (!vm.previewAsset || vm.previewAsset.documentAssetId != assetId)) {
                documentService.getById(assetId).then(function (asset) {
                    setAsset(asset);
                });
            }
        }

        /**
         * Initialise the state when the asset is changed
         */
        function setAsset(asset) {

            if (asset) {
                vm.previewAsset = asset;
                vm.isRemovable = !isRequired;
                vm.model = asset.documentAssetId;

                if (vm.updateAsset) {
                    vm.asset = asset;
                }

            } else if (isAssetInitialized) {
                // Ignore if we are running this first time to avoid overwriting the model with a null vlaue
                vm.previewAsset = null;
                vm.isRemovable = false;

                if (vm.model) {
                    vm.model = null;
                }
                if (vm.updateAsset) {
                    vm.asset = null;
                }
            }

            setButtonText();

            isAssetInitialized = true;
        }

        /* Helpers */

        function parseFilters(attributes) {
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

        function setButtonText() {
            vm.buttonText = vm.model ? 'Change' : 'Select';
        }
    }

}]);