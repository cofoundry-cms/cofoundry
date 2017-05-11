import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'button',
	templateUrl: 'button.component.html',
	inputs: [
		'title',
		'icon'
	]
})

export default class Button {
	title: string;
	icon: string;
}
