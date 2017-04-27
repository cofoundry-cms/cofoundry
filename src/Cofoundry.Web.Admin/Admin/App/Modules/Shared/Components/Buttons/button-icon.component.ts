import { Component, OnInit } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
    selector: SELECTOR_PREFIX + 'button-icon',
    templateUrl: 'button-icon.component.html',
    styleUrls: ['Buttons.component.scss'],
    inputs: [
        'title',
        'icon',
        'href',
        'external'
    ]
})

export class ButtonIcon implements OnInit {
    title: string;
    icon: string;
    iconCls: string;
    href: string;
    external: boolean;

    ngOnInit() {
        this.createIconCls();
    }

    createIconCls() {
        if (this.icon) {
            this.iconCls = 'fa-' + this.icon;
        }
    }
}
