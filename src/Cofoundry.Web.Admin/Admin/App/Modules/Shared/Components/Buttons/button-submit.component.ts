import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'button-submit',
	templateUrl: 'button-submit.component.html',
	inputs: [
		'text'
	]
})

export class ButtonSubmit {
	text: string;
}
