/**
 * A status message that can appear in a form to notify the user of any errors or other messages. A scope is
 * attached to the parent form and can be accessed via [myFormName].formStatus
 */
angular.module('cms.shared').directive('cmsFormStatus', [
    '_',
    'shared.validationErrorService',
    'shared.internalModulePath',
function (
    _,
    validationErrorService,
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/FormStatus.html',
        require: ['^^cmsForm'],
        replace: true,
        scope: true,
        link: { post: link }
    };

    function link(scope, el, attr, controllers) {

        initScope(scope, controllers[0]);
        bindValidationHandler(scope, el);
    }

    function initScope(scope, formController) {
        var formScope = formController.getFormScope(),
            form = formScope.getForm();

        scope.success = success.bind(scope);
        scope.error = error.bind(scope);
        scope.errors = errors.bind(scope);
        scope.clear = clear.bind(scope);
        form.formStatus = scope;
    }

    function bindValidationHandler(scope, el) {

        validationErrorService.addHandler('', scope.errors);
        scope.$on('$destroy', function () {
            validationErrorService.removeHandler(scope.errors);
        });
    }

    function errors(errors, message) {

        var processedErrors = _.uniq(errors, function (error) {
            return error.message;
        });

        setScope(this, message, 'error', processedErrors);
    }

    function error(message) {
        setScope(this, message, 'error');
    }

    function success(message) {
        setScope(this, message, 'success');
    }

    function clear() {
        setScope(this);
    }

    function setScope(scope, message, cls, errors) {
        scope.message = message;
        scope.errors = errors;
        scope.cls = cls;
    }
}]);
