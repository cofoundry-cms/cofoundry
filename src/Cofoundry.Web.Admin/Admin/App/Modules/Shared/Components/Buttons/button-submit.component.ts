import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'button-submit',
    templateUrl: 'button-submit.component.html',
    styleUrls: ['Buttons.component.scss'],
    inputs: [
        'text'
    ]
})

export class ButtonSubmit {
    text: string;
}
