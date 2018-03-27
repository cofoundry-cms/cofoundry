/**
 * File upload control for images. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsImageUpload', [
            '_',
            'shared.internalModulePath',
            'shared.internalContentPath',
            'shared.stringUtilities',
            'shared.urlLibrary',
        function (
            _,
            modulePath,
            contentPath,
            stringUtilities,
            urlLibrary) {

    /* VARS */

    var assetReplacementPath = contentPath + 'img/AssetReplacement/',
        noPreviewImagePath = assetReplacementPath + 'preview-not-supported.png',
        noImagePath = assetReplacementPath + 'image-replacement.png';

    /* CONFIG */

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/ImageAssets/ImageUpload.html',
        scope: {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState',
            isEditMode: '=cmsIsEditMode',
            modelName: '=cmsModelName',
            ngModel: '=ngModel',
            onChange: '&cmsOnChange'
        },
        require: 'ngModel',
        controller: function () { },
        controllerAs: 'vm',
        bindToController: true,
        link: link
    };

    
    /* LINK */

    function link(scope, el, attributes, ngModelController) {
        var vm = scope.vm,
            isRequired = _.has(attributes, 'required');

        init();

        /* INIT */

        function init() {
            vm.remove = remove;
            vm.fileChanged = onFileChanged;
            vm.isRemovable = _.isObject(vm.ngModel) && !isRequired;
            scope.$watch("vm.asset", setAsset);
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
                vm.isRemovable = !isRequired;

                ngModelController.$setViewValue({
                    name: asset.fileName + '.' + asset.extension,
                    size: asset.fileSizeInBytes,
                    isCurrentFile: true
                });
            } else {
                vm.previewUrl = noImagePath;
                vm.isRemovable = false;

                if (ngModelController.$modelValue) {
                    ngModelController.$setViewValue(undefined);
                }
            }

            setButtonText();
        }

        function onFileChanged($files) {
            if ($files && $files[0]) {
                // set the file is one is selected
                ngModelController.$setViewValue($files[0]);
                setPreviewImage($files[0]);
                vm.isRemovable = !isRequired;

            } else if (!vm.ngModel || _.isUndefined($files)) {
                // if we don't have an image loaded already, remove the file.
                ngModelController.$setViewValue(undefined);
                vm.previewUrl = noImagePath;
                vm.isRemovable = false;
                //vm.asset = undefined;
            }

            setButtonText();

            // base onChange event
            if (vm.onChange) vm.onChange(vm.ngModel);
        }

        /* Helpers */

        function setPreviewImage(file) {
            if (!isPreviewSupported(file))
            {
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

            return !_.find(unsupportedPreviewFormats, function(format) {
                return stringUtilities.endsWith(file.name, format);
            });
        }

        function setButtonText() {
            vm.buttonText = ngModelController.$modelValue ? 'Change' : 'Upload';
        }
    }

}]);