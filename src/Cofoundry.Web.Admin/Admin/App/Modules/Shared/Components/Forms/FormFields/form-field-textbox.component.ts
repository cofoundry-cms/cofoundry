import { Component, Input, ViewChild, Optional, Inject, forwardRef } from '@angular/core';
import { SELECTOR_PREFIX } from '../../../constants/config.constants';
import { ElementBase } from '../element-base';
import {
	NgModel,
	NG_VALUE_ACCESSOR,
	NG_VALIDATORS,
	NG_ASYNC_VALIDATORS,
} from '@angular/forms';

@Component({
	selector: `${SELECTOR_PREFIX}form-field-textbox`,
	templateUrl: 'form-field-textbox.component.html',
	providers: [
		{ provide: NG_VALUE_ACCESSOR, useExisting: FormFieldTextboxComponent, multi: true },
		{ provide: ElementBase, useExisting: forwardRef(() => FormFieldTextboxComponent) }
	]
})
export default class FormFieldTextboxComponent extends ElementBase<string> {
	@Input() public label: string;
	@Input() public placeholder: string;
	@ViewChild(NgModel) model: NgModel;

	constructor(
		@Optional() @Inject(NG_VALIDATORS) validators: Array<any>,
		@Optional() @Inject(NG_ASYNC_VALIDATORS) asyncValidators: Array<any>
	) {
		super(validators, asyncValidators);
	}
}
