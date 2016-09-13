(function ($, _) {

    /* Extensions */

    extendIfNotExists('offset', false, function (element, name, value) {
        var documentElem, box = { top: 0, left: 0 },
    	    doc = element && element.ownerDocument;

        if (!doc || !_.isUndefined(name)) {
            return;
        }

        documentElem = doc.documentElement;

        if (!_.isUndefined(element.getBoundingClientRect)) {
            box = element.getBoundingClientRect();
        }

        return {
            top: box.top + (window.pageYOffset || documentElem.scrollTop) - (documentElem.clientTop || 0),
            left: box.left + (window.pageXOffset || documentElem.scrollLeft) - (documentElem.clientLeft || 0)
        };
    });

    /* Helpers */

    /**
     * Taken from http://statelessprime.blogspot.co.uk/2013/09/extending-jqlite-with-new-functions.html 
     */
    function extendIfNotExists(name, wrapResult, fn) {
        if (!$.prototype[name]) {
            $.prototype[name] = function (arg1, arg2) {
                var value;

                for (var i = 0; i < this.length; i++) {
                    if (value == undefined) {
                        value = fn(this[i], arg1, arg2);

                        if (value !== undefined && wrapResult) {
                            // any function which returns a value needs to be wrapped
                            value = $(value);
                        }

                    } else {
                        jqLiteAddNodes(value, fn(this[i], arg1, arg2));
                    }

                }

                return value == undefined ? this : value;
            }
        }
    }

    /**
     * Copied from https://github.com/angular/angular.js/blob/master/src/jqLite.js
     */
    function jqLiteAddNodes(root, elements) {

        if (elements) {

            // if a Node (the most common case)
            if (elements.nodeType) {
                root[root.length++] = elements;
            } else {
                var length = elements.length;

                // if an Array or NodeList and not a Window
                if (typeof length === 'number' && elements.window !== elements) {
                    if (length) {
                        for (var i = 0; i < length; i++) {
                            root[root.length++] = elements[i];
                        }
                    }
                } else {
                    root[root.length++] = elements;
                }
            }
        }
    }

})(angular.element, _);