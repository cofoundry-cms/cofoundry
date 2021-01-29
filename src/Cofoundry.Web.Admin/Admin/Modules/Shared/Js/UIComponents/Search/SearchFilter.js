angular.module('cms.shared').directive('cmsSearchFilter', ['shared.internalModulePath', function (modulePath) {
    return {
        restrict: 'E',
        scope: { 
            query: '=cmsQuery',
            filter: '=cmsFilter',
            ngShow: '='
        },
        templateUrl: modulePath + 'UIComponents/Search/SearchFilter.html',
        transclude: true,

        controller: angular.noop,
        controllerAs: 'vm',
        bindToController: true,
        link: link
    };

    /* Link */

    function link(scope, el, attributes) {
        var vm = scope.vm;
        init();

        /* Init */

        function init() {

            /* Properties */
            if (_.isUndefined(vm.ngShow)) vm.ngShow = true;

            /* Actions */
            vm.setFilter = setFilter;
            vm.clear = clear;
        }

        /* Actions */

        function setFilter() {
            vm.query.update(vm.filter);

            vm.ngShow = true;
        }

        function clear() {
            vm.ngShow = false;
            vm.query.clear();
            vm.filter = vm.query.getFilters();
        }
    }
}]);