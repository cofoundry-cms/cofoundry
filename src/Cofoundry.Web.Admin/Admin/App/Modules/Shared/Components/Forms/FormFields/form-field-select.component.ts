import { Component, Input, ViewChild, Optional, Inject } from '@angular/core';
import { SELECTOR_PREFIX } from '../../../constants/config.constants';
import {
	NgModel,
	NG_VALUE_ACCESSOR,
	NG_VALIDATORS,
	NG_ASYNC_VALIDATORS,
} from '@angular/forms';
import { ElementBase } from '../element-base';

@Component({
	selector: `${SELECTOR_PREFIX}form-field-select`,
	templateUrl: 'form-field-select.component.html',
	providers: [
		{ provide: NG_VALUE_ACCESSOR, useExisting: FormFieldSelectComponent, multi: true }
	]
})
export default class FormFieldSelectComponent extends ElementBase<string> {
	@Input() public label: string;
	@Input() public placeholder: string;

	@ViewChild(NgModel) model: NgModel;

	//public identifier = `form-select-${identifier++}`;

	constructor(
		@Optional() @Inject(NG_VALIDATORS) validators: Array<any>,
		@Optional() @Inject(NG_ASYNC_VALIDATORS) asyncValidators: Array<any>,
	) {
		super(validators, asyncValidators);
	}
}

//let identifier = 0;
