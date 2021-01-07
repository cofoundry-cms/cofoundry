angular.module('cms.shared').factory('shared.timeUtilities', ['_', function (_) {
    var service = {};

    /* CONSTANTS */

    /* PUBLIC */

    service.toDate = function (timeAsString) {
        if (!timeAsString) return;

        var date = new Date(0);
        var match = /(\d{2}):(\d{2})(:(\d{2}))?/g.exec(timeAsString);
        if (!match) return null;
        date.setUTCHours(parseMatchGroup(1), parseMatchGroup(2), parseMatchGroup(4));

        return date;

        function parseMatchGroup(i) {
            var result = match[i];
            if (!result) return 0;
            return parseInt(result) || 0;
        }
    }

    service.format = function (date) {
        if (date && !_.isDate(date)) {
            date = service.toDate(date);
        }

        if (!date) return;

        var value = padTwoDigitNumber(date.getUTCHours())
            + ':' + padTwoDigitNumber(date.getUTCMinutes());

        var seconds = date.getUTCSeconds();
        if (seconds > 0) {
            value += ':' + padTwoDigitNumber(seconds);
        }

        return value;
    }

    function padTwoDigitNumber(number) {
        return ("00" + number).slice(-2);
    }

    return service;
}]);