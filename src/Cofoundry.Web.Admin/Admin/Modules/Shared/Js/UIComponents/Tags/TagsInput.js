/**
 * Allows inputting of tags in a comma delimited format. Only
 * allows alpha-numeric and '()&-_ characters.
 */
angular.module('cms.shared').directive('cmsTagsInput', [
    '_',
    'shared.stringUtilities',
function (
    _,
    stringUtilities
    ) {

    /* CONSTANTS */

    var CHAR_BLOCKLIST = /[^,&\w\s'()-]+/g,
        TAG_DELIMITER = ', ';

    /* CONFIG */

    return {
        restrict: 'A',
        require: 'ngModel',
        link: link
    };

    /* LINK */
    
    function link(scope, el, attributes, ngModelController) {
        var controller = this,
            vm = scope.vm;

        init();

        /* Init */

        function init() {
            ngModelController.$formatters.push(formatTags);
            ngModelController.$parsers.push(parseTags);
            ngModelController.$render = function () {
                el.val(ngModelController.$viewValue || '');
            };

            el.on('keypress', onKeyPress);
            el.on('blur change keyup', onValueChanged);
        }

        /* Formatter/Parser */

        function formatTags(tagsArray) {
            if (!tagsArray || !tagsArray.length) return '';

            return _.map(tagsArray, function (tag) {
                return stringUtilities.capitaliseFirstLetter(tag.replace(getBadTagRegex(), '').trim());
            }).join(TAG_DELIMITER);
        }

        function parseTags(tagString) {
            var nonEmptyTags, allTags;

            allTags = tagString
                .replace(getBadTagRegex(), '')
                .split(',')
                .map(function (s) { return s.trim() });

            nonEmptyTags = _.filter(allTags, function (tag) {
                return tag && tag !== ',';
            });

            if (!nonEmptyTags.length) {
                return null;
            }

            return nonEmptyTags;
        }

        /* Events */

        function onValueChanged(e) {
            cleanInputValue();
            scope.$evalAsync(setModelValue);
        }

        function onKeyPress(e) {
            var charToTest = String.fromCharCode(e.which);

            if (getBadTagRegex().test(charToTest)) {
                e.preventDefault();
            }
        }

        /* Helpers */

        function cleanInputValue() {
            var value = el.val(),
                cleanedValue = value.replace(getBadTagRegex(), '');

            if (value != cleanedValue) {
                el.val(cleanedValue);
            }
        }

        function setModelValue() {
            var value = el.val();
            ngModelController.$setViewValue(value);
        }

        function getBadTagRegex() {
            CHAR_BLOCKLIST.lastIndex = 0;
            return CHAR_BLOCKLIST;
        }
    }
}]);