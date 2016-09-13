angular.module('cms.shared').factory('shared.LoadState', ['$q', '$rootScope', '_', function($q, $rootScope, _) {
    return LoaderState;

    function LoaderState(onByDefault) {
        var me = this;

        /* Properties */

        me.isLoading = onByDefault === true;
        me.progress = me.isLoading ? 0 : 100;

        /* Funcs */

        me.on = function () {
            me.isLoading = true;
            if (me.progress === 100) {
                me.progress = 0;
            }
        }

        me.off = function() {
            me.isLoading = false;
            me.progress = 100;
        }

        me.offWhen = function () {
            var promises = [],
                promise,
                args = Array.prototype.slice.call(arguments);

            _.each(args, function(arg) {
                promises.push(arg.promise ? arg.promise : arg);
            });

            return $q.all(promises).then(function () {
                me.off();
            });
        }

        /**
         * Update the progress of the loader. Loaded can be a a numerical value
         * or the eventArgs to a file upload progress event. Total is optional 
         * in which case total is assumed to be 100.
         */
        me.setProgress = function (loaded, total) {
            var progress;

            // If we pass in the eventArgs of a progress event
            if (_.isObject(loaded)) {
                total = loaded.total;
                loaded = loaded.loaded;
            }

            // If no total is provided assume 100
            if (_.isUndefined(total)) total = 100;
            progress = parseInt(100.0 * loaded / total);

            if (progress <= 0) progress = 0;

            if (progress >= 100) {
                progress = 100;
            } else {
                me.on();
            }

            me.progress = progress;
        }
    }
}]);