import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'button',
    templateUrl: 'button.component.html',
    styleUrls: ['Buttons.component.scss'],
    inputs: [
        'text',
        'icon'
    ]
})

export class Button {
    text: string;
    icon: string;
}
