/**
 * Taken from https://gist.github.com/thomseddon/3511330
 */
angular.module('cms.shared').filter('bytes', function () {
    return function (bytes, precision) {
        var units = ['bytes', 'kb', 'mb', 'gb', 'tb', 'pb'],
            number;

        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (!bytes) return '0 ' + units[1];
        if (typeof precision === 'undefined') precision = 1;

        number = Math.floor(Math.log(bytes) / Math.log(1024));

        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    }
});