angular.module('cms.shared').factory('shared.arrayUtilities', function () {
    var service = {};

    /* PUBLIC */

    /**
     * Moves an item in an array from one index to another.
     */
    service.move = function (array, fromIndex, toIndex) {
        array.splice(toIndex, 0, array.splice(fromIndex, 1)[0]);
    };

    /**
     * Moves an object in an array from its index to another. If the object isn't
     * the same as the object in the array, you can use the propertyComparer to look up
     * the object based on a property match.
     */
    service.moveObject = function (array, objectToMove, toIndex, propertyComparer) {
        var fromIndex;

        if (propertyComparer) {
            fromIndex = _.findIndex(array, function (item) {
                return item[propertyComparer] === objectToMove[propertyComparer];
            });
        } else {
            fromIndex = array.indexOf(objectToMove);
        }

        service.move(array, fromIndex, toIndex);
    };

    /**
     * Removes the specified object from an array.
     */
    service.removeObject = function (arr, item) {
        var index = arr.indexOf(item);

        if (index >= 0) {
            return arr.splice(index, 1);
        }
    }

    service.remove = function (arr, index) {

        if (index >= 0) {
            return arr.splice(index, 1);
        }
    }
    
    return service;
});