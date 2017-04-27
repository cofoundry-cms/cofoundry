import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'menu',
    templateUrl: 'menu.component.html',
    styleUrls: ['Menu.component.scss'],
    inputs: [
        'icon'
    ]
})

export class Menu {
    icon: string;
}