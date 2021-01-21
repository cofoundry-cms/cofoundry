angular.module('cms.shared').factory('shared.SearchQuery', ['$location', '_', function ($location, _) {
    var pagingDefaultConfig = {
        pageSize: 30,
        pageNumber: 1
    };

    return SearchQuery;

    /**
     * Represents a query for searching entities and returning a paged result, handling
     * the persistance of the query parameters in the query string.
     */
    function SearchQuery(options) {
        var me = this,
            useHistory = _.isUndefined(options.useHistory) || options.useHistory,
            defaultParams = _.defaults({}, options.defaultParams, pagingDefaultConfig),
            filters = options.filters || [],
            searchParams = _.defaults({}, parseSearchParams(), defaultParams);

        /* Public Funcs */

        /**
         * Gets an object containing all the query params including filters and 
         * paging parameters.
         */
        me.getParameters = function () {
            return searchParams;
        }

        /**
         * Gets an object containing the filter portion of the query params.
         */
        me.getFilters = function () {
            return _.omit(searchParams, Object.keys(pagingDefaultConfig));
        }

        /**
         * Updates the query parameters.
         */
        me.update = function (query) {
            var newParams = _.defaults({}, query, defaultParams, searchParams);
            setParams(newParams);
        }

        /**
         * Resets the query to the defaul parameters
         */
        me.clear = function () {
            setParams(defaultParams);
        }

        /* Private Funcs */

        function parseSearchParams() {
            if (!useHistory) return {};

            var params = $location.search();

            _.each(params, function (value, key) {
                // parse numbers
                if (!isNaN(value)) {
                    params[key] = parseInt(value);
                }
            });

            return params;
        }

        function setParams(params) {
            var qsParams = {};

            searchParams = params;

            if (useHistory) {
                // filter out default params so they dont appear in the query string
              
                _.each(params, function (value, key) {
                    if (defaultParams[key] !== value && value) {
                        if (_.isFunction(value.toJSON)) {
                            // Dates need converting to ISO otherwise they break when re-loading from the query string
                            qsParams[key] = value.toJSON();
                        } else {
                            qsParams[key] = value;
                        }
                    }
                });
                console.log(qsParams);
                $location.search(qsParams);
            }

            if (options.onChanged) options.onChanged(me);
        }
    }
}]);