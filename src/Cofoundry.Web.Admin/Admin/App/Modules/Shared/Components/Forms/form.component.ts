import { Component } from '@angular/core';
import { SELECTOR_PREFIX } from '../../constants/config.constants';

@Component({
	selector: SELECTOR_PREFIX + 'form',
	styleUrls: ['./form.component.scss'],
	templateUrl: 'form.component.html',
})

export default class Form {
	onSubmit(values, valid) {
		console.log(values);
	}
}
