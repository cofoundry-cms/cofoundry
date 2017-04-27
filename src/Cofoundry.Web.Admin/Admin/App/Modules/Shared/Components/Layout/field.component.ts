import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'field',
    templateUrl: 'field.component.html',
    styleUrls: ['Layout.component.scss']
})

export class Field {
    modelName: string;
}