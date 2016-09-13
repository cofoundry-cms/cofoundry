/**
 * File upload control for images. Uses https://github.com/danialfarid/angular-file-upload
 */
angular.module('cms.shared').directive('cmsFormFieldDocumentUpload', [
    '_',
    'shared.internalModulePath',
    'baseFormFieldFactory',
function (
    _,
    modulePath,
    baseFormFieldFactory) {

    /* CONFIG */

    var config = {
        templateUrl: modulePath + 'UIComponents/DocumentAssets/FormFieldDocumentUpload.html',
        scope: _.extend(baseFormFieldFactory.defaultConfig.scope, {
            asset: '=cmsAsset',
            loadState: '=cmsLoadState'
        }),
        passThroughAttributes: ['required', 'ngRequired'],
        getInputEl: getInputEl
    };

    return baseFormFieldFactory.create(config);

    function getInputEl(rootEl) {
        return rootEl.find('cms-document-upload');
    }
}]);