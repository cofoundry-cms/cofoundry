import * as _ from 'lodash';

export default class LoadState {
    isLoading = false;
    progress = this.isLoading ? 0 : 100;

    constructor(onByDefault? : boolean) {
        this.isLoading = onByDefault === true;
    };

    on() {
        this.isLoading = true;
        if (this.progress === 100) {
            this.progress = 0;
        }
    }

    off() {
        this.isLoading = false;
        this.progress = 100;
    }

    offWhen() {
        var promises = [],
            promise,
            args = Array.prototype.slice.call(arguments);

        _.each(args, function(arg) {
            promises.push(arg.promise ? arg.promise : arg);
        });

        Promise.all(promises).then(function () {
            this.off();
        });
    }

    /**
     * Update the progress of the loader. Loaded can be a a numerical value
     * or the eventArgs to a file upload progress event. Total is optional 
     * in which case total is assumed to be 100.
     */
    setProgress(loaded, total) {
        var progress;

        // If we pass in the eventArgs of a progress event
        if (_.isObject(loaded)) {
            total = loaded.total;
            loaded = loaded.loaded;
        }

        // If no total is provided assume 100
        if (_.isUndefined(total)) total = 100;
        this.progress = parseInt((100.0 * loaded / total).toString(), null);

        if (progress <= 0) progress = 0;

        if (progress >= 100) {
            progress = 100;
        } else {
            this.on();
        }

        this.progress = progress;
    }
};