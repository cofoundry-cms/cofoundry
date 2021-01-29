angular.module('cms.shared').filter('time', [
    'shared.timeUtilities',
function (
    timeUtilities
) {
    return function (date) {
        return timeUtilities.format(date);
    }
}]);