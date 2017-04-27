import { Pipe, PipeTransform } from '@angular/core';

/**
 * Taken from https://gist.github.com/thomseddon/3511330
 * Uber translated to ES6
 */

@Pipe({name: 'bytes'})
export class BytesPipe implements PipeTransform {
    transform(bytes, precision): any {
        let units = ['bytes', 'kb', 'mb', 'gb', 'tb', 'pb'],
            number;

        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (!bytes) return '0 ' + units[1];
        if (typeof precision === 'undefined') precision = 1;

        number = Math.floor(Math.log(bytes) / Math.log(1024));

        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    }
}