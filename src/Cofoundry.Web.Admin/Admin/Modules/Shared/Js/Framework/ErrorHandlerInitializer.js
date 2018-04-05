/**
 * The default error handler is initialized here
 */
angular.module('cms.shared').run([
    'shared.errorService',
    'shared.modalDialogService',
    'shared.internalModulePath',
    'shared.stringUtilities',
    'shared.showDevException',
function (
    errorService,
    modalDialogService,
    modulePath,
    stringUtilities,
    showDevException
) {
    errorService.addHandler(onError);
    
    function onError(error) {
        var response = error.response,
            config = response ? response.config : null;

        // here we try and show the full developer exception page if the request
        // is local and we're permitted to show it.
        if (showDevException
            && config
            && stringUtilities.startsWith(config.url, '/')
            && stringUtilities.startsWith(response.headers('Content-Type') , 'text/html')
            && stringUtilities.startsWith(response.data, '<!DOCTYPE html>')
        ) {
            onDeveloperException(error);
        } else {
            modalDialogService.alert(error);
        }
    }

    function onDeveloperException(error) {

        modalDialogService.show({
            templateUrl: modulePath + "UIComponents/Modals/DeveloperException.html",
            controller: "DeveloperExceptionController",
            options: error
        });
    }
}]);