/**
 * @ngdoc filter
 * @name groupBy
 * @kind function
 *
 * @description
 * Create an object composed of keys generated from the result of running each element of a collection,
 * each key is an array of the elements.
 * Adapted from https://github.com/a8m/angular-filter
 */

angular.module('cms.shared').filter('groupBy', [
    '$parse',
    '_',
    'filterWatcher',
function (
    $parse,
    _,
    filterWatcher) {

    return function (collection, property) {

        if (!_.isObject(collection) || _.isUndefined(property)) {
            return collection;
        }

        var getterFn = $parse(property);

        return filterWatcher.isMemoized('groupBy', arguments) ||
        filterWatcher.memoize('groupBy', arguments, this,
            _groupBy(collection, getterFn));

        /**
        * groupBy function
        * @param collection
        * @param getter
        * @returns {{}}
        */
        function _groupBy(collection, getter) {
            var result = {};
            var prop;

            _.each(collection, function (elm) {
                prop = getter(elm);

                if (!result[prop]) {
                    result[prop] = [];
                }
                result[prop].push(elm);
            });
            return result;
        }
    }
}]);