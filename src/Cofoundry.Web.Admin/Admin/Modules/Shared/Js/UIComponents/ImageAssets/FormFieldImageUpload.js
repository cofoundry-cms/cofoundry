/**
 * File upload control for images. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsFormFieldImageUpload', [
            '_',
            'shared.internalModulePath',
            'baseFormFieldFactory',
        function (
            _,
            modulePath,
            baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/ImageAssets/FormFieldImageUpload.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState'
        }),
        passThroughAttributes: ['required', 'ngRequired'],
        getInputEl: getInputEl
    };

    return baseFormFieldFactory.create(config);

    function getInputEl(rootEl) {
        return rootEl.find('cms-image-upload');
    }
}]);