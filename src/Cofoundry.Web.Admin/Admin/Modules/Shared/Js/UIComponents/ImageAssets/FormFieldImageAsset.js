/**
 * A form field control for an image asset that uses a search and pick dialog
 * to allow the user to change the selected file.
 */
angular.module('cms.shared').directive('cmsFormFieldImageAsset', [
            '_',
            'shared.internalModulePath',
            'shared.internalContentPath',
            'shared.modalDialogService',
            'shared.stringUtilities',
            'shared.imageService',
            'shared.urlLibrary',
            'baseFormFieldFactory',
        function (
            _,
            modulePath,
            contentPath,
            modalDialogService,
            stringUtilities,
            imageService,
            urlLibrary,
            baseFormFieldFactory) {

            /* VARS */

            var assetReplacementPath = contentPath + 'img/AssetReplacement/',
                noImagePath = assetReplacementPath + 'image-replacement.png',
                baseConfig = baseFormFieldFactory.defaultConfig;

            /* CONFIG */

            var config = {
                templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageAsset.html',
                scope: _.extend(baseConfig.scope, {
                    asset: '=cmsAsset', // if we already have the full asset data we can set it here to save an api call
                    loadState: '=cmsLoadState',
                    updateAsset: '@cmsUpdateAsset' // update the asset property if it changes
                }),
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
                    isAssetInitialized;

                init();
                return baseConfig.link(scope, el, attributes, controllers);

                /* INIT */

                function init() {

                    vm.urlLibrary = urlLibrary;
                    vm.showPicker = showPicker;
                    vm.remove = remove;
                    vm.isRemovable = _.isObject(vm.model) && !isRequired;

                    vm.filter = parseFilters(attributes);
                    vm.previewWidth = attributes['cmsPreviewWidth'] || 220;
                    vm.previewHeight = attributes['cmsPreviewHeight'];
                    
                    scope.$watch("vm.asset", setAsset);
                    scope.$watch("vm.model", setAssetById);
                }

                /* EVENTS */

                function remove() {
                    setAsset(null);
                }

                function showPicker() {

                    modalDialogService.show({
                        templateUrl: modulePath + 'UIComponents/ImageAssets/ImageAssetPickerDialog.html',
                        controller: 'ImageAssetPickerDialogController',
                        options: {
                            currentAsset: vm.previewAsset,
                            filter: vm.filter,
                            onSelected: onSelected
                        }
                    });

                    function onSelected(newAsset) {

                        if (!newAsset && vm.previewAsset) {
                            setAsset(null);

                            vm.onChange();
                        } else if (!vm.previewAsset || (newAsset && vm.previewAsset.imageAssetId !== newAsset.imageAssetId)) {
                            setAsset(newAsset);

                            vm.onChange();
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

                    if (assetId && (!vm.previewAsset || vm.previewAsset.imageAssetId != assetId)) {
                        imageService.getById(assetId).then(function (asset) {
                            if (asset) {
                                setAsset(asset);
                            }
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
                        vm.model = asset.imageAssetId;

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

                function setButtonText() {
                    vm.buttonText = vm.model ? 'Change' : 'Select';
                }
            }

        }]);