import { Component, Input, ViewChild, Optional, OnInit, Inject, forwardRef } from '@angular/core';
import { SELECTOR_PREFIX } from '../../../constants/config.constants';
import {
	NgModel,
	NG_VALUE_ACCESSOR,
	NG_VALIDATORS,
	NG_ASYNC_VALIDATORS,
} from '@angular/forms';
import { ElementBase } from '../element-base';
import { LocaleService } from '../../../services';

@Component({
	selector: `${SELECTOR_PREFIX}form-field-select-locale`,
	templateUrl: 'form-field-select-locale.component.html',
	providers: [
		LocaleService,
		{ provide: NG_VALUE_ACCESSOR, useExisting: FormFieldSelectLocaleComponent, multi: true },
		{ provide: ElementBase, useExisting: forwardRef(() => FormFieldSelectLocaleComponent) }
	]
})
export default class FormFieldSelectLocaleComponent extends ElementBase<string> implements OnInit {
	@Input() public label: string;
	@Input() public placeholder: string;
	@Input() public description: string;
	@ViewChild(NgModel) model: NgModel;
	locales: any[];

	constructor(
		@Optional() @Inject(NG_VALIDATORS) validators: Array<any>,
		@Optional() @Inject(NG_ASYNC_VALIDATORS) asyncValidators: Array<any>,
		private _localeService: LocaleService
	) {
		super(validators, asyncValidators);
	}

	ngOnInit() {
		this._localeService
			.getAll()
			.subscribe(response => {
				let data: any[] = response.json().data;
				data.map(locale => {
					return {
						name: `${locale.name} (${locale.ietfLanguageTag})`,
						id: locale.localeId
					};
				});
				this.locales = data;
			});
	}
}
