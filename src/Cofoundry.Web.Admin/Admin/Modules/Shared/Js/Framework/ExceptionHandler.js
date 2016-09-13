//angular.module('cms.shared').factory('$exceptionHandler', [
//    '$injector',
//function (
//    $injector) {

//    var isErrorDisplayed;

//    return function (exception, cause) {
//        // Log out the error
//        console.error('error:', exception, cause);

//        // If we're not already displaying an error dialog, show one.
//        if (!isErrorDisplayed) {
//            isErrorDisplayed = true;
//            var modalDialogService = $injector.get('shared.modalDialogService');

//            modalDialogService
//                .alert({
//                    title: 'Error',
//                    message: 'An unexpected error has occured.'
//                })
//                .then(function () {
//                    isErrorDisplayed = false;
//                });
//        }
//    };
//}]);