﻿angular.module('cms.shared').directive('cmsPager', [
    'shared.internalModulePath',
function (
    modulePath
    ) {

    return {
        restrict: 'E',
        scope: { 
            result: '=cmsResult',
            query: '=cmsQuery'
        },
        templateUrl: modulePath + 'UIComponents/Search/Pager.html',
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
            vm.setPage = setPage;
            scope.$watch('vm.result', setResult);
        }

        /* Actions */

        function setPage(pageNumber) {
            if (!vm.query) return;

            vm.query.update({
                pageNumber: pageNumber
            });
        }

        /* Watches*/
        function setResult(newResult) {
            if (!newResult) {
                vm.isFirstPage = true;
                vm.isLastPage = true;
            }
            else {
                vm.isFirstPage = newResult.pageNumber <= 1
                vm.isLastPage = newResult.pageNumber === newResult.pageCount;
            }
        }
    }

}]);