angular.module('cms.shared').controller('ImageAssetEditorDialogController', [
    '$scope',
    'shared.LoadState',
    'shared.imageService',
    'shared.SearchQuery',
    'shared.urlLibrary',
    'options',
    'close',
function (
    $scope,
    LoadState,
    imageService,
    SearchQuery,
    urlLibrary,
    options,
    close) {
    
    var vm = $scope,
        isAssetInitialized;

    init();
    
    /* INIT */
    
    function init() {
        angular.extend($scope, options);

        vm.formLoadState = new LoadState();
        vm.saveLoadState = new LoadState();

        vm.onInsert = onInsert;
        vm.onCancel = onCancel;

        vm.onImageChanged = onImageChanged;
        vm.command = {};

        setCurrentImage();
    }

    /* ACTIONS */

    function setCurrentImage() {
        // If we have an existing image, we need to find the asset id to set the command image
        if (vm.imageAssetHtml && vm.imageAssetHtml.length) {
            vm.command.imageAssetId = vm.imageAssetHtml.attr('data-image-asset-id');
            vm.command.altTag = vm.imageAssetHtml.attr('alt');
            vm.command.style = vm.imageAssetHtml.attr('style');

            // If the image had any styles (mainly dimensions), pass them to the command so they are retained
            if (vm.command.style) {
                var styles = parseStyles(vm.command.style);
                vm.command.width = styles['width'];
                vm.command.height = styles['height'];

            // Else, look to see if the dimensions are stored as attibutes of the image
            } else {
                vm.command.width = vm.imageAssetHtml.attr('width');
                vm.command.height = vm.imageAssetHtml.attr('height');
            }

            // If we cannot find the asset id (could have removed the data attribute that this relies on),
            // we try to work this out based on the image path (this might change in future versions of cofoundry so less reliable)
            if (!vm.command.imageAssetId) {
                var src = vm.imageAssetHtml.attr('src');
                var lastIndex = src.lastIndexOf('/');
                var extractId = src.substr(lastIndex + 1, ((src.indexOf('_') - lastIndex) - 1));
                vm.command.imageAssetId = extractId;
            }
        }
    }
    
    /* EVENTS */

    function onCancel() {
        close();
    }

    function onImageChanged() {
        vm.command.altTag = vm.command.imageAsset.title || vm.command.imageAsset.fileName;
    }

    function onInsert() {

        // Parse and hold dimensions
        var dimensions = {
            width: parseUnits(vm.command.width),
            height: parseUnits(vm.command.height)
        };

        // If we have no sizes set, default to percentage respecting ratio
        if (!dimensions.width && !dimensions.height) {
            dimensions.width = '100%';
            dimensions.height = 'auto';
        }

        // Get the image path, including specific size options if nessessary
        var path = urlLibrary.getImageUrl(vm.command.imageAsset, parseImageRequestSize(dimensions));

        // Default the alt tag to an empty string if not specified
        var alt = vm.command.altTag || '';

        // Define an object thay holds formatted outputs, plus the model itself
        var output = {
            markdown: "![Alt " + alt + "](" + path + ")",
            html: "<img src='" + path + "' alt='" + alt + "' data-image-asset-id='" + vm.command.imageAssetId + "' />",
            model: vm.command
        };

        // Add css styles to output html
        output.html = insertCssStyles(output.html, dimensions);

        // Call callback with output
        vm.onSelected(output);

        // Close dialog
        close();
    }

    /* PUBLIC HELPERS */

    function insertCssStyles(html, styles) {
        return angular.element(html).css(styles)[0].outerHTML;
    }

    function parseImageRequestSize(dimensions) {
        // If unit type is percent, use original image size
        if ((dimensions.width || '').indexOf('%') > -1 || (dimensions.height || '').indexOf('%') > -1) return {};

        // Else, return raw pixel sizes
        return {
            width: dimensions.width.replace('px', ''),
            height: dimensions.height.replace('px', '')
        };
    }

    function parseUnits(value) {
        if (!value) return '';

        // Default to pixels if not unit type specified
        if (value.indexOf('px') == -1 && value.indexOf('%') == -1 && value.indexOf('auto') == -1) return value + 'px';

        // Return original value if we get here
        return value;
    }

    function parseStyles(cssText) {
        var regex = /([\w-]*)\s*:\s*([^;]*)/g;
        var match, properties = {};
        while (match = regex.exec(cssText)) properties[match[1]] = match[2];
        return properties;
    }

}]);
