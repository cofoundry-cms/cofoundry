angular.module('cms.shared').directive('cmsForm', [
    'shared.internalModulePath',
function (
    modulePath) {

    return {
        restrict: 'E',
        templateUrl: modulePath + 'UIComponents/Form/Form.html',
        replace: true,
        transclude: true,
        scope: {
            editMode: '=cmsEditMode',
            name: '@cmsName'
        },
        compile: compile,
        controller: ['$scope', FormController]
    };

    /* CONTROLLER/COMPILE */

    function FormController($scope) {
        $scope.getForm = function () {
            return $scope[$scope.name];
        }

        this.getFormScope = function () {

            return $scope;
        }
    };

    function compile(element, attrs) {
        // Default edit mode to true if not specified
        if (!angular.isDefined(attrs.cmsEditMode)) {
            attrs.cmsEditMode = 'true';
        }

        return link;
    }

    function link (scope, el, attrs, controllers) {
        // Do somethng similar to the behavior of NgForm and bind the form property a 
        // parent scope except in our case the root scope.
        var parentScope = findRootScopeModel(scope);
        parentScope[scope.name] = scope.getForm();
    }

    /* HELPERS */

    function findRootScopeModel(scope, vmScope) {
        var parent = scope.$parent;

        // We've reached the root, return a vm scope or the root scope
        if (!parent) return vmScope || scope;

        if (angular.isDefined(parent.vm)) {
            // we've found a parent with a controller as 'vm' scope
            vmScope = parent.vm;
        }

        // Keep searching up the tree recursively and return the last one found
        return findRootScopeModel(parent, vmScope);
    }
}]);