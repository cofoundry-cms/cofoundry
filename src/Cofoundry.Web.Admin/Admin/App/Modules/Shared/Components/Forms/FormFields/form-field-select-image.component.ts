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
	selector: `${SELECTOR_PREFIX}form-field-select-image`,
	templateUrl: 'form-field-select-image.component.html',
	providers: [
		{ provide: NG_VALUE_ACCESSOR, useExisting: FormFieldSelectImageComponent, multi: true }
	]
})
export default class FormFieldSelectImageComponent extends ElementBase<string> {
	@Input() public label: string;
	@Input() public placeholder: string;
	@Input() public description: string;

	@ViewChild(NgModel) model: NgModel;

	constructor(
		@Optional() @Inject(NG_VALIDATORS) validators: Array<any>,
		@Optional() @Inject(NG_ASYNC_VALIDATORS) asyncValidators: Array<any>
	) {
		super(validators, asyncValidators);
	}
}
