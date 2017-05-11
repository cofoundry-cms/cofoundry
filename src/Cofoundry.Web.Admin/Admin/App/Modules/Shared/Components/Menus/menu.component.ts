import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'menu',
	templateUrl: 'menu.component.html',
	styleUrls: ['menu.component.scss'],
	inputs: [
		'icon'
	]
})

export class Menu {
	icon: string;
}
