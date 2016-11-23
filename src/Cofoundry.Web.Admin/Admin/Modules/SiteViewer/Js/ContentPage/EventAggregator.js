Cofoundry.events = {
    on: function (ev, callback) {
        var calls = this._callbacks || (this._callbacks = {});
        (this._callbacks[ev] || (this._callbacks[ev] = [])).push(callback);
    },
    off: function (ev, callback) {
        for (var i = 0, len = this._callbacks[ev].length; i < len; i++) {
            if (this._callbacks[ev][i] === callback) {
                this._callbacks[ev].splice(i, 1);
            }
        }
    },
    trigger: function () {
        var args = Array.prototype.slice.call(arguments, 0);
        var ev = args.shift();
        var list, calls, i, l;

        if (!(calls = this._callbacks)) return this;
        if (!(list = this._callbacks[ev])) return this;

        for (i = 0, l = list.length; i < l; i++)
            list[i].apply(this, args);

        return this;
    }
}