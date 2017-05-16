import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'button-submit',
	templateUrl: 'button-submit.component.html',
	inputs: [
		'title',
		'disabled'
	]
})

export default class ButtonSubmit {
	title: string;
	disabled: boolean;
}
