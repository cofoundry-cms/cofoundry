/**
 * File upload control for documents/files. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsDocumentUpload', [
            '_',
            'shared.internalModulePath',
            'shared.urlLibrary',
        function (
            _,
            modulePath,
            urlLibrary 
        ) {

    /* CONFIG */
    
    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/DocumentAssets/DocumentUpload.html',
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
                vm.previewUrl = urlLibrary.getDocumentUrl(asset);
                vm.isRemovable = !isRequired;

                ngModelController.$setViewValue({
                    name: asset.fileName + '.' + asset.fileExtension,
                    size: asset.fileSizeInBytes,
                    isCurrentFile: true
                });

            } else {
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
                vm.isRemovable = !isRequired;

            } else if (!vm.ngModel || _.isUndefined($files)) {
                // if we don't have a file loaded already, remove the file.
                ngModelController.$setViewValue(undefined);
                vm.previewUrl = null;
                vm.isRemovable = false;
                vm.asset = undefined;
            }

            setButtonText();

            // base onChange event
            if (vm.onChange) vm.onChange(vm.ngModel);
        }

        /* Helpers */

        function setButtonText() {
            vm.buttonText = ngModelController.$modelValue ? 'Change' : 'Upload';
        }
    }

}]);