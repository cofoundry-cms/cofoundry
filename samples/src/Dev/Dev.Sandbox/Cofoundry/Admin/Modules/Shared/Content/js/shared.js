angular.module('cms.shared').factory('shared.testService', [
    '$http',
    'shared.serviceBase',
function (
    $http,
    serviceBase) {

    var service = {},
        testServiceBase = serviceBase + 'test';

    /* QUERIES */

    service.getAll = function () {
        return $http.get(testServiceBase);
    }

    return service;

}]);
angular.module('cms.shared').directive('cmsFormFieldTestSelector', [
    '_',
    'shared.directiveUtilities',
    'shared.modulePath',
    'shared.testService',
    function (
        _,
        directiveUtilities,
        modulePath,
        testService
    ) {

        return {
            restrict: 'E',
            templateUrl: modulePath + 'UIComponents/FormFieldTestSelector.html',
            scope: {
                model: '=cmsModel',
                title: '@cmsTitle',
                onLoaded: '&cmsOnLoaded'
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
            vm.title = attrs.cmsTitle || 'Test';
            vm.description = attrs.cmsDescription;
            directiveUtilities.setModelName(vm, attrs);
        }

        /* CONTROLLER */

        function Controller() {
            var vm = this;

            testService.getAll().then(function (testData) {
                vm.testData = testData;

                if (vm.onLoaded) vm.onLoaded();
            });
        }
    }]);