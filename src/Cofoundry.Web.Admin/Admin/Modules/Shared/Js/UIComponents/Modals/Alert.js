angular.module('cms.shared').controller('AlertController', [
    '$scope',
    'options',
    'close', function (
        $scope,
        options,
        close) {

    angular.extend($scope, options);
    $scope.close = close;
}]);