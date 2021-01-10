angular.module('cms.shared').directive('cmsFormFieldLocaleSelector', [
    '_',
    'shared.internalModulePath',
    'shared.localeService',
    'shared.directiveUtilities',
function (
    _,
    modulePath,
    localeService,
    directiveUtilities
    ) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Locales/FormFieldLocaleSelector.html',
        scope: {
            model: '=cmsModel',
            onLoaded: '&cmsOnLoaded',
            readonly: '=cmsReadonly'
        },
        link: {
            pre: preLink
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* COMPILE */

    function preLink(scope, el, attrs) {
        var vm = scope.vm;

        if (angular.isDefined(attrs.required)) {
            vm.isRequired = true;
        } else {
            vm.isRequired = false;
            vm.defaultItemText = attrs.cmsDefaultItemText || 'None';
        }

        directiveUtilities.setModelName(vm, attrs);
    }

    /* CONTROLLER */

    function Controller() {
        var vm = this;

        localeService.getAll().then(function (locales) {

            vm.locales = _.map(locales, function (locale) {
                return {
                    name: locale.name + ' (' + locale.ietfLanguageTag + ')',
                    id: locale.localeId
                }
            });

            if (vm.onLoaded) vm.onLoaded();
        });
    }
}]);