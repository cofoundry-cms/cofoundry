/**
* Fix to angular 1.5 > 1.6 upgrade where the default hashPrefix has changed. Here 
* we remove it to be consistent with previous behavior.
*/
angular.module('cms.shared').config(['$locationProvider', function ($locationProvider) {
    $locationProvider.hashPrefix('');
}]);