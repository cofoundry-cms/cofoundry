import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'button-link',
	templateUrl: 'button-link.component.html',
	inputs: [
		'title',
		'href',
		'iconCls'
	]
})

export default class ButtonLink {
	title: string;
	href: string;
	iconCls: string;
}
