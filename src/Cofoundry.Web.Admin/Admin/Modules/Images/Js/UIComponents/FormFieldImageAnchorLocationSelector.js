angular.module('cms.shared').directive('cmsFormFieldImageAnchorLocationSelector', ['_', 'images.modulePath', function (_, modulePath) {
    return {
        restrict: 'E',
        templateUrl: modulePath + 'uicomponents/FormFieldImageAnchorLocationSelector.html',
        scope: {
            model: '=cmsModel'
        },
        controller: Controller,
        controllerAs: 'vm',
        bindToController: true
    };

    /* CONTROLLER */

    function Controller() {
        var vm = this;
        
        vm.options = [
            { name: 'Top Left', id: 'TopLeft' },
            { name: 'Top Center', id: 'TopCenter' },
            { name: 'Top Right', id: 'TopRight' },
            { name: 'Middle Left', id: 'MiddleLeft' },
            { name: 'Middle Center', id: 'MiddleCenter' },
            { name: 'Middle Right', id: 'MiddleRight' },
            { name: 'Bottom Left', id: 'BottomLeft' },
            { name: 'Bottom Center', id: 'BottomCenter' },
            { name: 'Bottom Right', id: 'BottomRight' }
        ];
    }
}]);