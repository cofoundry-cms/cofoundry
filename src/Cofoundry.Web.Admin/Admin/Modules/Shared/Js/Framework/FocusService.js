angular.module('cms.shared').factory('shared.focusService', ['$document', function ($document) {
    var service = {},
        doc = $document[0];

    /* PUBLIC */

    /**
    * Focus's a dom element with the specified id.
    */
    service.focusById = function (id) {
        var el = doc.getElementById(id);
        if (el) {
            el.focus();
        }
    }

    return service;
}]);