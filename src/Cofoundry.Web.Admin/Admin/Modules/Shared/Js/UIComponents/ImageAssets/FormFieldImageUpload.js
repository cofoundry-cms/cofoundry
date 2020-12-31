/**
 * File upload control for images. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsFormFieldImageUpload', [
    '_',
    '$timeout',
    'shared.internalModulePath',
    'shared.internalContentPath',
    'shared.LoadState',
    'shared.stringUtilities',
    'shared.imageFileUtilities',
    'shared.imageService',
    'shared.urlLibrary',
    'shared.validationErrorService',
    'baseFormFieldFactory',
function (
    _,
    $timeout,
    modulePath,
    contentPath,
    LoadState,
    stringUtilities,
    imageFileUtilities,
    imageService,
    urlLibrary,
    validationErrorService,
    baseFormFieldFactory) {

    /* VARS */

    var baseConfig = baseFormFieldFactory.defaultConfig,
        assetReplacementPath = contentPath + 'img/AssetReplacement/',
        noPreviewImagePath = assetReplacementPath + 'preview-not-supported.png',
        noImagePath = assetReplacementPath + 'image-replacement.png';

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageUpload.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState',
            filter: '=cmsFilter',
            onChange: '&cmsOnChange'
        }),
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

            vm.isRequired = _.has(attributes, 'required');
            vm.remove = remove;
            vm.fileChanged = onFileChanged;
            vm.isRemovable = _.isObject(vm.ngModel) && !vm.isRequired;
            vm.mainLoadState = new LoadState(true);

            scope.$watch("vm.asset", setAsset);

            imageService
                .getSettings()
                .then(mapSettings)
                .then(vm.mainLoadState.off);
        }

        function mapSettings(settings) {
            vm.settings = settings;
        }

        /* EVENTS */

        function remove() {
            onFileChanged();
        }

        /**
         * Initialise the state when the asset is changed
         */
        function setAsset() {
            var asset = vm.asset;

            if (asset) {
                vm.previewUrl = urlLibrary.getImageUrl(asset, {
                    width: 220
                });
                vm.isRemovable = !vm.isRequired;

                vm.model = {
                    name: asset.fileName + '.' + asset.extension,
                    size: asset.fileSizeInBytes,
                    isCurrentFile: true
                };
            } else {
                vm.previewUrl = noImagePath;
                vm.isRemovable = false;

                if (vm.model) {
                    vm.model = undefined;
                }
            }

            setButtonText();
        }

        function onFileChanged($files) {

            if ($files && $files[0]) {
                vm.mainLoadState.on();

                // set the file if one is selected
                imageFileUtilities
                    .getFileInfoAndResizeIfRequired($files[0], vm.settings)
                    .then(onImageInfoLoaded)
                    .finally(vm.mainLoadState.off);

            } else if (!vm.ngModel || _.isUndefined($files)) {
                onNoFileSelected();
            }

            function onImageInfoLoaded(imgInfo) {
                if (!imgInfo) {
                    onNoFileSelected();
                }

                vm.model = imgInfo.file;
                validateSize(imgInfo);

                setPreviewImage(imgInfo.file);
                vm.isRemovable = !vm.isRequired;
                vm.isResized = imgInfo.isResized;
                vm.resizeSize = imgInfo.width + 'x' + imgInfo.height;
                onComplete();
            }

            function validateSize(imgInfo) {
                var filter = vm.filter;
                if (!filter) return;

                if (!isMinSize(filter.minWidth, imgInfo.width)
                    || !isMinSize(filter.minHeight, imgInfo.height)
                    || !isSize(filter.width, imgInfo.width)
                    || !isSize(filter.height, imgInfo.height)) {
                    addError('The image is ' + imgInfo.width + 'x' + imgInfo.height + ' which does not meet the size requirements.');
                }

                function isMinSize(size, value) {
                    return !(size > 1) || value >= size;
                }

                function isSize(size, value) {
                    return !(size > 1) || value == size;
                }
            }

            function addError(message) {
                // Run in next digest cycle otherwise it will
                // be overwirtten in the model change
                $timeout(function () {
                    validationErrorService.raise([{
                        message: message,
                        properties: [vm.modelName]
                    }]);
                });
            }

            function onNoFileSelected() {

                // if we don't have an image loaded already, remove the file.
                vm.model = undefined;
                vm.previewUrl = noImagePath;
                vm.isRemovable = false;
                vm.isResized = false;
                onComplete();
            }

            function onComplete() {
                setButtonText();

                // base onChange event
                if (vm.onChange) vm.onChange();
            }
        }

        /* Helpers */

        function setPreviewImage(file) {
            if (!isPreviewSupported(file)) {
                vm.previewUrl = noPreviewImagePath;
                return;
            }

            try {
                vm.previewUrl = URL.createObjectURL(file);
            }
            catch (err) {
                vm.previewUrl = noPreviewImagePath;
            }
        }

        function isPreviewSupported(file) {
            var unsupportedPreviewFormats = ['.tiff', '.tif'];

            return !_.find(unsupportedPreviewFormats, function (format) {
                return stringUtilities.endsWith(file.name, format);
            });
        }

        function setButtonText() {
            vm.buttonText = vm.model ? 'Change' : 'Upload';
        }
    }
}]);