import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'button-link',
    templateUrl: 'button-link.component.html',
    styleUrls: ['Buttons.component.scss'],
    inputs: [
        'text',
        'href',
        'iconCls'
    ]
})

export class ButtonLink {
    text: string;
    href: string;
    iconCls: string;
}
