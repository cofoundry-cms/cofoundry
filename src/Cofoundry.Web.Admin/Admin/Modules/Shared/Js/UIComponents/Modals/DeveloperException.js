angular.module('cms.shared').controller('DeveloperExceptionController', [
    '$scope',
    '$sce',
    'shared.internalContentPath',
    'options',
    'close',
function (
    $scope,
    $sce,
    internalContentPath,
    options,
    close) {

    var html = options.response.data;

    var iframe = document.createElement('iframe');
    iframe.setAttribute('srcdoc', html);
    iframe.setAttribute('src', internalContentPath + 'developer-exception-not-supported.html');
    iframe.setAttribute('sandbox', 'allow-scripts');
    $scope.messageHtml = $sce.trustAsHtml(iframe.outerHTML);

    angular.extend($scope, options);
    $scope.close = close;

}]);