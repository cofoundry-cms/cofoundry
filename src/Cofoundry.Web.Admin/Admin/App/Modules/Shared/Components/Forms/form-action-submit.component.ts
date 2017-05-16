import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'form-action-submit',
	template: `<input class="btn main-cta" type="submit" value="{{title}}" [disabled]="disabled" />`,
	inputs: [
		'title',
		'disabled'
	]
})

export default class FormActionSubmitComponent {
	title: string;
	disabled: boolean;
}
